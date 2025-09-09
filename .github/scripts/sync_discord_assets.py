import os, json, time, hashlib, datetime, pathlib
import requests

DISCORD_TOKEN = os.environ["DISCORD_TOKEN"]
CHANNEL_IDS   = [c.strip() for c in os.environ.get("CHANNEL_IDS","").split(",") if c.strip()]
SAVE_ROOT     = os.environ.get("SAVE_ROOT","assets/discord")
STATE_FILE    = os.environ.get("STATE_FILE",".sync/discord_state.json")
MAX_LOOKBACK  = int(os.environ.get("MAX_LOOKBACK","500"))
RECENT_WINDOW = int(os.environ.get("RECENT_WINDOW","80"))
ALLOW_EXT     = set([e.strip().lower() for e in os.environ.get("ALLOW_EXT","png,jpg,jpeg,gif,webp").split(",") if e.strip()])
ENABLE_HASH   = os.environ.get("ENABLE_HASH","0") == "1"

HDRS = {
    "Authorization": f"Bot {DISCORD_TOKEN}",
    "User-Agent": "discord-sync-bot (github-actions)"
}
BASE = "https://discord.com/api/v10"

def load_state():
    if not os.path.exists(STATE_FILE):
        return {}
    with open(STATE_FILE,"r",encoding="utf-8") as f:
        try:
            return json.load(f)
        except:
            return {}

def save_state(state):
    os.makedirs(os.path.dirname(STATE_FILE), exist_ok=True)
    with open(STATE_FILE,"w",encoding="utf-8") as f:
        json.dump(state, f, ensure_ascii=False, indent=2)

def get_channel_name(cid):
    r = requests.get(f"{BASE}/channels/{cid}", headers=HDRS, timeout=30)
    r.raise_for_status()
    data = r.json()
    return data.get("name") or str(cid)

def list_messages(cid, params):
    r = requests.get(f"{BASE}/channels/{cid}/messages", headers=HDRS, params=params, timeout=30)
    if r.status_code == 403:
        raise RuntimeError(f"No permission to read channel {cid}.")
    r.raise_for_status()
    return r.json(), r.headers

def sha256_url(url):
    h = hashlib.sha256()
    with requests.get(url, stream=True, timeout=120) as r:
        r.raise_for_status()
        for chunk in r.iter_content(1024*1024):
            if chunk:
                h.update(chunk)
    return h.hexdigest()

def ext_ok(filename):
    ext = filename.rsplit(".",1)[-1].lower() if "." in filename else ""
    return (ext in ALLOW_EXT) if ALLOW_EXT else True

def ymd_from_ts(ts_iso):
    # Discord message timestamp is ISO8601 in message['timestamp']
    try:
        dt = datetime.datetime.fromisoformat(ts_iso.replace("Z","+00:00"))
    except Exception:
        dt = datetime.datetime.utcnow()
    return dt.strftime("%Y"), dt.strftime("%m"), dt.strftime("%d")

def process_messages(cid, ch_name, msgs, state):
    """
    msgs는 오래된 것부터 처리(커밋 순서 안정)
    """
    ch_key = str(cid)
    st = state.setdefault(ch_key, {"last_message_id":"0", "seen":{}})
    seen = st["seen"]

    new_max = int(st["last_message_id"])
    changed = False

    for m in msgs:
        mid   = int(m["id"])
        ts    = m.get("timestamp","")
        year,month,day = ymd_from_ts(ts)
        atts  = m.get("attachments",[]) or []

        if mid > new_max:
            new_max = mid

        # 첨부만 대상
        if not atts: 
            continue

        # 저장 디렉토리
        base_dir = pathlib.Path(SAVE_ROOT)/ch_name/year/month/day/str(mid)
        base_dir.mkdir(parents=True, exist_ok=True)

        for a in atts:
            aid = a["id"]
            key = f"{m['id']}#{aid}"
            if key in seen:
                continue  # 이미 처리

            fn  = a["filename"]
            url = a["url"]
            if not ext_ok(fn):
                continue

            # 다운로드
            dst = base_dir/fn
            with requests.get(url, stream=True, timeout=180) as r:
                r.raise_for_status()
                with open(dst, "wb") as f:
                    for chunk in r.iter_content(1024*1024):
                        if chunk:
                            f.write(chunk)

            file_hash = ""
            if ENABLE_HASH:
                file_hash = sha256_url(url)

            # 메타 기록
            meta = {
                "message_id": str(m["id"]),
                "attachment_id": str(aid),
                "filename": fn,
                "size": a.get("size"),
                "content_type": a.get("content_type"),
                "url": url,
                "timestamp": ts,
                "sha256": file_hash
            }
            with open(base_dir/"_meta.json","a",encoding="utf-8") as f:
                f.write(json.dumps(meta, ensure_ascii=False)+"\n")

            seen[key] = {"sha256": file_hash} if file_hash else {}
            changed = True

    # last_message_id 갱신
    if new_max > int(st["last_message_id"]):
        st["last_message_id"] = str(new_max)
        changed = True

    return changed

def backoff_sleep(headers):
    # 간단 레이트리밋 대응
    rl = headers.get("X-RateLimit-Remaining")
    if rl is not None and rl == "0":
        reset = headers.get("X-RateLimit-Reset-After")
        try:
            t = float(reset)
        except:
            t = 2.0
        time.sleep(t)
    else:
        time.sleep(0.3)

def main():
    if not CHANNEL_IDS:
        raise SystemExit("CHANNEL_IDS env is empty. Set repo variable DISCORD_CHANNEL_IDS.")

    state = load_state()

    for cid in CHANNEL_IDS:
        ch_name = ""
        try:
            ch_name = get_channel_name(cid)
        except Exception as e:
            print(f"[warn] get_channel_name({cid}) failed:", e)
            ch_name = str(cid)

        print(f"\n== Channel {cid} ({ch_name}) ==")
        st = state.setdefault(str(cid), {"last_message_id":"0","seen":{}})
        last_id = int(st.get("last_message_id","0"))

        # 1) last_message_id 이후(증분) 긁기 (oldest_first 되도록 반복 수집 후 역정렬)
        collected = []
        after = last_id if last_id>0 else None
        fetched = 0
        while True:
            params = {"limit": 100}
            if after:
                params["after"] = after
            msgs, hdr = list_messages(cid, params=params)
            if not msgs:
                break
            fetched += len(msgs)
            # 디스코드는 최신→오래된 순 반환. 우리는 오래된→최신으로 처리하려고 모아뒀다가 나중에 reverse
            collected.extend(msgs)
            after = int(msgs[0]["id"])  # 가장 큰 ID 뒤이어 계속
            backoff_sleep(hdr)
            if fetched >= MAX_LOOKBACK:
                break
        collected.sort(key=lambda m: int(m["id"]))  # 오래된→최신

        changed1 = process_messages(cid, ch_name, collected, state)

        # 2) 최근 윈도우 재검사(편집/교체 보완)
        recent_params = {"limit": min(100, RECENT_WINDOW)}
        recent_msgs, hdr = list_messages(cid, params=recent_params)
        backoff_sleep(hdr)
        recent_msgs.sort(key=lambda m: int(m["id"]))
        changed2 = process_messages(cid, ch_name, recent_msgs, state)

        if changed1 or changed2:
            save_state(state)
            print(f"[update] state saved for channel {cid}")
        else:
            print("[ok] no new/changed attachments")

if __name__ == "__main__":
    main()
