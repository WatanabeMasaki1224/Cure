using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyData data;
    Rigidbody2D rb;
    Transform player;
    RepairPoint targetRepair;
    int currentHP;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
        currentHP = data.maxHP;
    }


    void FixedUpdate()
    {
        Vector2 targetPos = GetTargetPosition();
        Move(targetPos);
    }

    // ターゲット決定
    Vector2 GetTargetPosition()
    {
        if (data.type == EnemyType.TargetPlayer)
        {
            return player.position;
        }
        else // Repair狙い
        {
            // ターゲット更新
            if (targetRepair == null || targetRepair.state == RepairPoint.RepairState.Broken)
            {
                targetRepair = RepairManager.Instance.GetClosestTarget(transform.position);
            }

            if (targetRepair != null)
            {
                return targetRepair.transform.position;
            }

            // 無ければプレイヤー
            return player.position;
        }
    }

    // 移動
    void Move(Vector2 target)
    {
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * data.moveSpeed;
    }

    // プレイヤーに接触
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit: " + other.name + " Tag:" + other.tag);
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(data.attackPower);
            }
            Destroy(gameObject);
        }

        if (data.type != EnemyType.TargetRepair) return;

        if (other.CompareTag("Repair"))
        {
            RepairPoint rp = other.GetComponent<RepairPoint>();

            if (rp != null && rp.state == RepairPoint.RepairState.Repaired)
            {
                rp.TakeDamage();
            }
            Destroy(gameObject) ;
        }

    }

    // ダメージ処理
    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

}
