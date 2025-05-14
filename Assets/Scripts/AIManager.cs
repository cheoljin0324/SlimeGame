using UnityEngine;

public class AIManager : MonoBehaviour
{
    public DefaultDatable data;  // AI가 참조할 ScriptableObject
    private int currentAIId;
    private Transform player;  // 플레이어 위치

    void Start()
    {
        if (data != null)
        {
            currentAIId = data.id;  // ScriptableObject에서 id값을 읽어오기
        }
        else
        {
            Debug.LogError("No DefaultDatable assigned to AIManager!");
        }

        // 플레이어를 찾기
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        // AI 동작을 실행하는 메서드를 호출
        ExecuteAIBehavior(currentAIId);
    }

    private void ExecuteAIBehavior(int aiId)
    {
        switch (aiId)
        {
            case 1:
                FollowPlayerAI();  // 플레이어를 추적하는 AI
                break;

            case 2:
                AggressiveAI();  // 공격적인 AI
                break;

            case 3:
                DefensiveAI();  // 방어적인 AI
                break;

            default:
                Debug.LogWarning("Unknown AI id");
                break;
        }
    }

    // ID가 1일 때, 플레이어를 추적하는 AI
    private void FollowPlayerAI()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;  // 플레이어와의 방향 벡터 계산
            transform.Translate(direction * Time.deltaTime * 3f);  // 플레이어 쪽으로 이동 (속도 3f)
        }
    }

    private void AggressiveAI()
    {
        // 공격적인 AI 로직 (플레이어를 추적하는 예시)
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * Time.deltaTime * 3f);  // 플레이어 쪽으로 이동
        }
    }

    private void DefensiveAI()
    {
        // 방어적인 AI 로직 (피하는 예시)
        if (player != null)
        {
            Vector2 direction = (transform.position - player.position).normalized;
            transform.Translate(direction * Time.deltaTime * 1.5f);  // 플레이어 반대 방향으로 이동
        }
    }
}
