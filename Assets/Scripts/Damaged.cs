using UnityEngine;
using System.Collections;

public class Damaged : MonoBehaviour
{
    public DefaultDatable data;
    public int currentHp;

    private bool isInvincible = false;
    private Coroutine damageCoroutine;
    private SpriteRenderer spriteRenderer;
    public Spawner spawner;

    void Start()
    {
        spawner = FindAnyObjectByType<Spawner>();
        currentHp = data.Hp;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer not found on " + gameObject.name);
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f; // �߷� ����
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (ShouldTakeDamage(collision))
        {
            damageCoroutine = StartCoroutine(ApplyDamageOverTime(collision));
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private IEnumerator ApplyDamageOverTime(Collision2D collision)
    {
        while (true)
        {
            if (CompareTag("Enemy"))
            {
                if (collision.collider.CompareTag("Attack"))
                {
                    TakeDamage(collision.collider);
                }
            }
            else if (CompareTag("Player"))
            {
                if ((collision.collider.CompareTag("Enemy") || collision.collider.CompareTag("EnemyAttack")) && !isInvincible)
                {
                    TakeDamage(collision.collider);
                    StartCoroutine(InvincibilityCoroutine(3f));
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private bool ShouldTakeDamage(Collision2D collision)
    {
        if (CompareTag("Enemy") && collision.collider.CompareTag("Attack")) return true;
        if (CompareTag("Player") && (collision.collider.CompareTag("Enemy") || collision.collider.CompareTag("EnemyAttack"))) return true;
        return false;
    }

    private void TakeDamage(Collider2D source)
    {
        int damage = 1;
        Damaged attacker = source.GetComponent<Damaged>();
        if (attacker != null && attacker.data != null)
        {
            damage = attacker.data.atk;
        }

        currentHp -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining HP: {currentHp}");

        StartCoroutine(DamageFlashEffect());

        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void ReceiveDamage(int damage)
    {
        if (!CompareTag("Enemy")) return;

        currentHp -= damage;
        Debug.Log($"{gameObject.name} received {damage} direct damage. Remaining HP: {currentHp}");

        StartCoroutine(DamageFlashEffect());

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private IEnumerator InvincibilityCoroutine(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }

    private IEnumerator DamageFlashEffect()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = originalColor;
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        Destroy(gameObject);
        if (gameObject.CompareTag("Enemy"))
        {
            spawner.enemyCnt -= 1;
            GameManager.Instance.StateCheck();
        }
    }

    private IEnumerator FlashRed()
    {
        // SpriteRenderer ������Ʈ�� ������
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // ���� ������ ����
        Color originalColor = spriteRenderer.color;

        // ������ ȿ���� ���� (���� ���� 0.5�� ����)
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);

        // ��� ��� (������ ���·� ��� ���)
        yield return new WaitForSeconds(0.1f);

        // ���� �������� ����
        spriteRenderer.color = originalColor;
    }


    public void ReceiveAreaDamage(int damage)
    {
        currentHp -= damage;
        Debug.Log($"{gameObject.name} took {damage} area damage. Remaining HP: {currentHp}");

        StartCoroutine(FlashRed());

        if (currentHp <= 0)
        {
            Die();
        }
    }
}
