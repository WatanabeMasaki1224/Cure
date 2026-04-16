using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
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
    [SerializeField] int _score = 10;
    [SerializeField] float chaseRange = 5f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckDistance = 0.2f; //前にどれくらいずらしてチェックするか
    [SerializeField] float groundCheckOffset = 0.5f;//どのくらい下を見るか
    [SerializeField] float patrolRange = 3f; // ウロウロ範囲
    float startX;
    float patrolDir = 1f;
    Transform currentTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
        currentHP = data.maxHP;
        startX = transform.position.x;
    }


    void FixedUpdate()
    {
        if(data.type == EnemyType.TargetPlayer)
        {
            float distX = Mathf.Abs(player.position.x - transform.position.x);
            float distY = Mathf.Abs(player.position.y - transform.position.y);

            if (distX <= chaseRange && distY <= 1f)
            {
                Move(player.position);
            }

            else
            {
                Patrol();
            }
            return;
        }

        currentTarget = FindTarget();
        if (currentTarget != null)
        {
            float distX = Mathf.Abs(currentTarget.position.x - transform.position.x);

            if (distX <= chaseRange)
            {
                Move(currentTarget.position);
            }
            else
            {
                Patrol();
            }
        }
        else
        {
            Patrol();
        }

    }


    /// <summary>
    /// 移動
    /// </summary>
    /// <param name="target"></param>
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

    Transform FindTarget()
    {
        //範囲内のリペアpointを探す
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(chaseRange*2f,0.1f),0);
        float minDist = Mathf.Infinity;
        RepairPoint nearest = null;

        foreach(var hit in hits)
        {
            if(hit.CompareTag("Repair"))
            {
                RepairPoint rp = hit.GetComponentInParent<RepairPoint>();

                if(rp != null && rp.state == RepairPoint.RepairState.Repaired)
                {
                    float distX = Mathf.Abs(rp.transform.position.x - transform.position.x);
                    if(distX < minDist)
                    {
                        minDist = distX;
                        nearest = rp;
                    }
                }
            }
        }

        if(nearest != null)
            return nearest.transform;

        //プレイヤー探索
        PlayerController pc = player.GetComponent<PlayerController>();
        bool playerAlive = pc != null && !pc.IsDead();

        if(playerAlive)
        {
            float distX = Mathf.Abs(player.position.x - transform.position.x);
            float distY = Mathf.Abs(player.position.y - transform.position.y);

            if (distX <= chaseRange && distY <= 1f)
                return player;
        }

        return null;
    }
}
