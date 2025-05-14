using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour
{

    public float attackRadius = 1.5f;      // ���� �ݰ�
    public int damage = 1;                 // �� ������
    public float attackInterval = 2f;      // ���� ����
    public ParticleSystem attackEffect;

    private void Start()
    {
        InvokeRepeating(nameof(AreaAttack), 0f, attackInterval);
        GetComponent<ParticleSystem>().Stop();
    }

    void AreaAttack()
    {
        // �÷��̾� �ֺ� �ݰ� ���� �ִ� ��� �ݶ��̴� ��������
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRadius);

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                Damaged enemy = hit.GetComponent<Damaged>();
                if (enemy != null)
                {
                    enemy.ReceiveAreaDamage(damage);
                }
            }
        }

        // ����Ʈ ó���� ���⿡ �߰� ���� (��: ���� ���� ����Ʈ)
        attackEffect.Play();
    }

    // �ð������� ���� ���� Ȯ��
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
