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
            if (targetRepair == null || targetRepair.state == RepairPoint.RepairState.Repaired)
            {
                targetRepair = RepairManager.Instance.GetClosestBroken(transform.position);
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
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // ダメージ処理（あとで実装）
            Debug.Log("Player Damage");
        }
    }

    //修復ポイントに接触
    void OnTriggerStay2D(Collider2D other)
    {
        if (data.type != EnemyType.TargetRepair) return;

        if (other.CompareTag("Repair"))
        {
            RepairPoint rp = other.GetComponent<RepairPoint>();

            if (rp != null && rp.state == RepairPoint.RepairState.Repaired)
            {
                // 壊す
                rp.state = RepairPoint.RepairState.Broken;

                // スコアマイナスとかここ
                Debug.Log("Repair Destroyed");
            }
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
