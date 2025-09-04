using UnityEngine;

public class AIManager : MonoBehaviour
{
    // 아아아아아악ㅇㅇㅇㅇㅇ
    public DefaultDatable data;
    private int currentAIId;
    private Transform player;
    private EnemyAnimation enemyAnimation; // EnemyAnimation 스크립트를 가져오기

    void Start()
    {
        if (data != null)
        {
            currentAIId = data.id;
        }
        else
        {
            Debug.LogError("No DefaultDatable assigned to AIManager!");
        }

        player = GameObject.FindWithTag("Player")?.transform;
        enemyAnimation = GetComponent<EnemyAnimation>(); // EnemyAnimation 컴포넌트 찾기
    }

    void Update()
    {
        ExecuteAIBehavior(currentAIId);
    }

    private void ExecuteAIBehavior(int aiId)
    {
        switch (aiId)
        {
            case 1:
                FollowPlayerAI();  // ID 1: 플레이어 추적
                break;

            case 2:
                AggressiveAI();
                break;

            case 3:
                DefensiveAI();
                break;

            default:
                Debug.LogWarning("Unknown AI id");
                break;
        }
    }

    private void FollowPlayerAI()
    {
        if (player == null || data == null) return;

        Vector2 moveDir = Vector2.zero;
        Vector2 toPlayer = player.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        // 플레이어를 향해 이동
        if (distanceToPlayer > 0.8f) // 일정 거리 이상일 때만 이동
        {
            moveDir += toPlayer.normalized;
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
        }
        else
        {
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);
        }

        // 방향에 맞게 스프라이트 반전 (플레이어가 왼쪽이면 반전)
        if (toPlayer.x < 0)
        {
            enemyAnimation.FlipSprite(false); // 왼쪽을 볼 때
        }
        else
        {
            enemyAnimation.FlipSprite(true); // 오른쪽을 볼 때
        }

        // 이동 처리
        transform.Translate(moveDir.normalized * data.spd * Time.deltaTime);
    }

    private void AggressiveAI()
    {
        if (player == null || data == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * Time.deltaTime * data.spd * 1.2f);
        enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
    }

    private void DefensiveAI()
    {
        if (player == null) return;

        Vector2 direction = (transform.position - player.position).normalized;
        transform.Translate(direction * Time.deltaTime * 1.5f);
        enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
    }
}
