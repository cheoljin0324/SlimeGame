using UnityEngine;

public class AIManager : MonoBehaviour
{
    // �ƾƾƾƾƾǤ���������
    public DefaultDatable data;
    private int currentAIId;
    private Transform player;
    private EnemyAnimation enemyAnimation; // EnemyAnimation ��ũ��Ʈ�� ��������

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
        enemyAnimation = GetComponent<EnemyAnimation>(); // EnemyAnimation ������Ʈ ã��
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
                FollowPlayerAI();  // ID 1: �÷��̾� ����
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

        // �÷��̾ ���� �̵�
        if (distanceToPlayer > 0.8f) // ���� �Ÿ� �̻��� ���� �̵�
        {
            moveDir += toPlayer.normalized;
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
        }
        else
        {
            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle);
        }

        // ���⿡ �°� ��������Ʈ ���� (�÷��̾ �����̸� ����)
        if (toPlayer.x < 0)
        {
            enemyAnimation.FlipSprite(false); // ������ �� ��
        }
        else
        {
            enemyAnimation.FlipSprite(true); // �������� �� ��
        }

        // �̵� ó��
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
