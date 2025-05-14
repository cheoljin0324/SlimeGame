using UnityEngine;

public class AIManager : MonoBehaviour
{
    public DefaultDatable data;  // AI�� ������ ScriptableObject
    private int currentAIId;
    private Transform player;  // �÷��̾� ��ġ

    void Start()
    {
        if (data != null)
        {
            currentAIId = data.id;  // ScriptableObject���� id���� �о����
        }
        else
        {
            Debug.LogError("No DefaultDatable assigned to AIManager!");
        }

        // �÷��̾ ã��
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        // AI ������ �����ϴ� �޼��带 ȣ��
        ExecuteAIBehavior(currentAIId);
    }

    private void ExecuteAIBehavior(int aiId)
    {
        switch (aiId)
        {
            case 1:
                FollowPlayerAI();  // �÷��̾ �����ϴ� AI
                break;

            case 2:
                AggressiveAI();  // �������� AI
                break;

            case 3:
                DefensiveAI();  // ������� AI
                break;

            default:
                Debug.LogWarning("Unknown AI id");
                break;
        }
    }

    // ID�� 1�� ��, �÷��̾ �����ϴ� AI
    private void FollowPlayerAI()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;  // �÷��̾���� ���� ���� ���
            transform.Translate(direction * Time.deltaTime * 3f);  // �÷��̾� ������ �̵� (�ӵ� 3f)
        }
    }

    private void AggressiveAI()
    {
        // �������� AI ���� (�÷��̾ �����ϴ� ����)
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * Time.deltaTime * 3f);  // �÷��̾� ������ �̵�
        }
    }

    private void DefensiveAI()
    {
        // ������� AI ���� (���ϴ� ����)
        if (player != null)
        {
            Vector2 direction = (transform.position - player.position).normalized;
            transform.Translate(direction * Time.deltaTime * 1.5f);  // �÷��̾� �ݴ� �������� �̵�
        }
    }
}
