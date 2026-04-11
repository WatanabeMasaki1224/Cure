using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyData data;
    Rigidbody2D rb;
    Transform player;
    RepairPoint targetRepair;
    int currentHP;
    Spawner spawner;
    EnemyData myData;

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
            if (targetRepair == null || targetRepair.state == RepairPoint.RepairState.Broken)
            {
                RepairPoint[] all = GameObject.FindObjectsByType<RepairPoint>(FindObjectsSortMode.None);
                float minDist = Mathf.Infinity;
                targetRepair = null;

                foreach (var rp in all)
                {
                    if (rp.state != RepairPoint.RepairState.Broken)
                    {
                        float dist = Vector2.Distance(transform.position, rp.transform.position);

                        if (dist < minDist)
                        {
                            minDist = dist;
                            targetRepair = rp;
                        }
                    }
                }
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
            RepairPoint rp = other.GetComponentInParent<RepairPoint>();

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


    public void Init(Spawner spawner, EnemyData data)
    {
        this.spawner = spawner;
        this.myData = data;
    }
}
