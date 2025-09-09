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

# ✅ 알림 메시지 관련
POST_CONFIRM   = os.environ.get("POST_CONFIRM","0") == "1"     # 1이면 안내 메시지 전송
POST_CHANNELID = os.environ.get("POST_CHANNEL_ID","")          # 공지 채널 ID, 비우면 원본 채널에 보냄
MSG_TEMPLATE = os.environ.get(
    "POST_MESSAGE_TEMPLATE",
    "✅이미지 너무 쌓아뒀잖아요 선생님! ({count}개)\n{files}"
)

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

def list_messages(cid, params):
    r = requests.get(f"{BASE}/channels/{cid}/messages", headers=HDRS, params=params, timeout=30)
    r.raise_for_status()
    return r.json(), r.headers

def post_message(channel_id, content):
    """디스코드 채널에 채팅 전송"""
    url = f"{BASE}/channels/{channel_id}/messages"
    r = requests.post(url, headers=HDRS, json={"content": content}, timeout=30)
    if r.status_code not in (200, 201):
        print("[warn] post_message:", r.status_code, r.text)

def ext_ok(filename):
    ext = filename.rsplit(".",1)[-1].lower() if "." in filename else ""
    return (ext in ALLOW_EXT) if ALLOW_EXT else True

def ymd_from_ts(ts_iso):
    try:
        dt = datetime.datetime.fromisoformat(ts_iso.replace("Z","+00:00"))
    except Exception:
        dt = datetime.datetime.utcnow()
    return dt.strftime("%Y"), dt.strftime("%m"), dt.strftime("%d")

def process_messages(cid, ch_name, msgs, state):
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

        if not atts: 
            continue

        base_dir = pathlib.Path(SAVE_ROOT)/ch_name/year/month/day/str(mid)
        base_dir.mkdir(parents=True, exist_ok=True)

        new_files = []

        for a in atts:
            aid = a["id"]
            key = f"{m['id']}#{aid}"
            if key in seen:
                continue

            fn  = a["filename"]
            url = a["url"]
            if not ext_ok(fn):
                continue

            dst = base_dir/fn
            with requests.get(url, stream=True, timeout=180) as r:
                r.raise_for_status()
                with open(dst, "wb") as f:
                    for chunk in r.iter_content(1024*1024):
                        if chunk:
                            f.write(chunk)

            seen[key] = {}
            new_files.append(fn)
            changed = True

        if new_files and POST_CONFIRM:
            files_line = "\n".join(f"- {name}" for name in new_files)
            content = MSG_TEMPLATE.format(
                count=len(new_files),
                files=files_line,
                save_dir=str(base_dir)
            )
            target = POST_CHANNELID if POST_CHANNELID else cid
            post_message(target, content)

    if new_max > int(st["last_message_id"]):
        st["last_message_id"] = str(new_max)
        changed = True
    return changed

def backoff_sleep(headers):
    rl = headers.get("X-RateLimit-Remaining")
    if rl is not None and rl == "0":
        reset = headers.get("X-RateLimit-Reset-After")
        try: t = float(reset)
        except: t = 2.0
        time.sleep(t)
    else:
        time.sleep(0.3)

def main():
    state = load_state()

    for cid in CHANNEL_IDS:
        try:
            st = state.setdefault(str(cid), {"last_message_id":"0","seen":{}})
            last_id = int(st.get("last_message_id","0"))
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
                collected.extend(msgs)
                after = int(msgs[0]["id"])
                backoff_sleep(hdr)
                if fetched >= MAX_LOOKBACK:
                    break
            collected.sort(key=lambda m: int(m["id"]))
            changed1 = process_messages(cid, str(cid), collected, state)

            recent_params = {"limit": min(100, RECENT_WINDOW)}
            recent_msgs, hdr = list_messages(cid, params=recent_params)
            backoff_sleep(hdr)
            recent_msgs.sort(key=lambda m: int(m["id"]))
            changed2 = process_messages(cid, str(cid), recent_msgs, state)

            if changed1 or changed2:
                save_state(state)
                print(f"[update] state saved for channel {cid}")
            else:
                print(f"[ok] no new files in channel {cid}")
        except Exception as e:
            print("error channel", cid, ":", e)

if __name__ == "__main__":
    main()
