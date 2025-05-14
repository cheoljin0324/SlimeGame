using UnityEngine;

public class Damageable : MonoBehaviour
{
    public DefaultDatable data;
    public int currentHp;

    void Start()
    {
        // HP �ʱ�ȭ
        currentHp = data.Hp;

        // Rigidbody2D�� ���� ���, isKinematic�� true�� �����Ͽ� ���� ������ �߷� ������ ���� �ʵ��� ��
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.CompareTag("Enemy"))
        {
            // ���� "Attack" �±׿� ���� ������
            if (other.CompareTag("Attack"))
            {
                TakeDamage(other);
            }
        }
        else
        {
            // �÷��̾ ��Ÿ ��ü�� "Enemy" �Ǵ� "EnemyAttack" �±׿� ���� ������
            if (other.CompareTag("Enemy") || other.CompareTag("EnemyAttack"))
            {
                TakeDamage(other);
            }
        }
    }

    private void TakeDamage(Collider2D source)
    {
        // ������ ��� ����: source���� Damageable ������ atk ���
        int damage = 1;

        Damageable attacker = source.GetComponent<Damageable>();
        if (attacker != null && attacker.data != null)
        {
            damage = attacker.data.atk;
        }

        currentHp -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining HP: {currentHp}");

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        // �״� ���� �� �߰� ����
        Destroy(gameObject);
    }
}
