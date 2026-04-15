using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    enum EnemyState
    {
        Patrol, // うろうろ
        Chase   // 追跡
    }

    [SerializeField] EnemyData data;
    Rigidbody2D rb;
    Transform player;
    RepairPoint targetRepair;
    int currentHP;
    Spawner spawner;
    EnemyData myData;
    [SerializeField] int _score = 10;
    [SerializeField] float chaseRange = 5f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckDistance = 0.2f; //前にどれくらいずらしてチェックするか
    [SerializeField] float groundCheckOffset = 0.5f;//どのくらい下を見るか
    EnemyState state = EnemyState.Patrol;
    [SerializeField] float patrolRange = 3f; // ウロウロ範囲
    float startX;
    float patrolDir = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
        currentHP = data.maxHP;
        startX = transform.position.x;
    }


    void FixedUpdate()
    {
        float distX = Mathf.Abs(player.position.x - transform.position.x);

        // 状態切り替え
        if (distX <= chaseRange)
        {
            state = EnemyState.Chase;
        }
        else
        {
            state = EnemyState.Patrol;
        }

        if (state == EnemyState.Chase)
        {
            Vector2 targetPos = GetTargetPosition();
            Move(targetPos);
        }
        else
        {
            Patrol();
        }
    }

    // ターゲット決定
    Vector2 GetTargetPosition()
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        bool playerAlive = pc != null && !pc.IsDead();

        if (data.type == EnemyType.TargetPlayer)
        {
            if (playerAlive)
                return player.position;

            // 死んでたらその場待機
            return transform.position;
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
            if (playerAlive)
            {
                return player.position;
            }

            // どっちも無いなら停止
            return transform.position;
        }
    }

    // 移動
    void Move(Vector2 target)
    {
        // X距離だけで判定
        float distX = Mathf.Abs(target.x - transform.position.x);

        // 範囲外なら停止
        if (distX > chaseRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // 進行方向（Xのみ）
        float dir = Mathf.Sign(target.x - transform.position.x);

        // 足場チェック（前方の足元）
        Vector2 checkPos = (Vector2)transform.position + new Vector2(dir * groundCheckOffset, -0.5f);

        RaycastHit2D groundHit = Physics2D.Raycast(checkPos, Vector2.down, groundCheckDistance, groundLayer);

        // 足場がない → 落ちるの防ぐ
        if (groundHit.collider == null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // 移動（Yはそのまま）
        rb.linearVelocity = new Vector2(dir * data.moveSpeed, rb.linearVelocity.y);
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

        if (data.type == EnemyType.TargetRepair && other.CompareTag("Repair"))
        {
            RepairPoint rp = other.GetComponentInParent<RepairPoint>();

            if (rp != null && rp.state == RepairPoint.RepairState.Repaired)
            {
                rp.TakeDamage();
                Die(other.gameObject);
            }
        }

    }

    // ダメージ処理
    public void TakeDamage(int damage, GameObject attacker)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die(attacker);
        }
    }


    public void Init(Spawner spawner, EnemyData data)
    {
        this.spawner = spawner;
        this.myData = data;
    }

    void Die(GameObject attaacker)
    {
        if(attaacker.CompareTag("Bullet"))
        {
            ScoreManager.Instance.Add(data.score);
        }

        spawner?.OnEnemyDead(myData);
        Destroy(gameObject);
    }

    void Patrol()
    {
        float left = startX - patrolRange;
        float right = startX + patrolRange;

        // 端で反転
        if (transform.position.x <= left)
            patrolDir = 1f;

        if (transform.position.x >= right)
            patrolDir = -1f;

        // 足場チェック
        Vector2 checkPos = (Vector2)transform.position + new Vector2(patrolDir * groundCheckOffset, -0.5f);
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, groundCheckDistance, groundLayer);

        if (hit.collider == null)
        {
            patrolDir *= -1f;
            return;
        }

        rb.linearVelocity = new Vector2(patrolDir * data.moveSpeed, rb.linearVelocity.y);
    }
}
