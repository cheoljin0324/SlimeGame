using UnityEngine;

public class Damageable : MonoBehaviour
{
    public DefaultDatable data;
    public int currentHp;

    void Start()
    {
        // HP 초기화
        currentHp = data.Hp;

        // Rigidbody2D가 있을 경우, isKinematic을 true로 설정하여 물리 엔진의 중력 영향을 받지 않도록 함
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
            // 적은 "Attack" 태그에 의해 데미지
            if (other.CompareTag("Attack"))
            {
                TakeDamage(other);
            }
        }
        else
        {
            // 플레이어나 기타 객체는 "Enemy" 또는 "EnemyAttack" 태그에 의해 데미지
            if (other.CompareTag("Enemy") || other.CompareTag("EnemyAttack"))
            {
                TakeDamage(other);
            }
        }
    }

    private void TakeDamage(Collider2D source)
    {
        // 데미지 계산 예시: source에서 Damageable 가져와 atk 사용
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
        // 죽는 연출 등 추가 가능
        Destroy(gameObject);
    }
}
