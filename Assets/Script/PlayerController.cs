using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Normal,
        Attack,
        Repair
    }

    
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePosition;
    [SerializeField] float attackDuration = 0.5f;
    [SerializeField] float jumpPower = 5f;
    [SerializeField] float repairRange = 1f;
    [SerializeField] int maxHP = 5;
    private int currentHP;
    float attackTimer = 0f;
    private Rigidbody2D rb;
    Vector2 moveInput;
    PlayerState state;
    private Vector2 lastDirection = Vector2.right;
    // 地面判定
    [SerializeField] Transform grounndCheck;
    [SerializeField] float checkRadius = 0.1f;
    [SerializeField] LayerMask groundLayer;
    private bool isGround;
    RepairPoint currentRepairPoint;
    [SerializeField] float knockbackForce = 5f;
    [SerializeField] int desPenaluty = 50;
    bool isInvincible = false;
    [SerializeField] float respawnTime = 3f;
    [SerializeField] Transform respawnPoint;

    bool isDead = false;
    SpriteRenderer sr;
    Collider2D col;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // 横方向の入力があるときだけ lastFacing を更新
        if (moveInput.x != 0)
        {
            lastDirection = new Vector2(Mathf.Sign(moveInput.x), 0);
        }

        if (state == PlayerState.Attack)
        {
            attackTimer -= Time.deltaTime;
            if(attackTimer < 0f)
            {
                state = PlayerState.Normal;
            }
            return;
        }

        // 毎フレーム リペア対象を取得
        currentRepairPoint = null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, repairRange);

        foreach (var h in hits)
        {
            if (h.CompareTag("Repair"))
            {
                currentRepairPoint = h.GetComponentInParent<RepairPoint>();
                break;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Attack();
        }

        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            rb.linearVelocity  = new Vector2(rb.linearVelocity.x, jumpPower);
        }

        // Repair開始
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerStartRepair();
        }

        // Repair終了
        if (Input.GetKeyUp(KeyCode.E))
        {
            PlayerStopRepair();
        }

        // Repair中の進行
        if (state == PlayerState.Repair)
        {
            PlayerUpdateRepair();
        }
    }

    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(grounndCheck.position,checkRadius,groundLayer);

        if (state != PlayerState.Normal)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed , rb.linearVelocity.y);
    }

    void Attack()
    {
        state = PlayerState.Attack;
        attackTimer = attackDuration;
        Vector2 dir = lastDirection;
        // 入力がない時は右向き（仮）
        if (dir == Vector2.zero)
            dir = Vector2.right;
        GameObject bullet = Instantiate(bulletPrefab, firePosition.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Init(dir);
    }

    void PlayerStartRepair()
    {
        Debug.Log("StartRepair呼ばれた");
        if (state == PlayerState.Attack) return;
        if (currentRepairPoint == null)
        {
            Debug.Log("RepairPointがnull");
            return;
        }

        state = PlayerState.Repair;
        currentRepairPoint.StartRepair();
    }

    void PlayerStopRepair()
    {
        if (state != PlayerState.Repair) return;

        currentRepairPoint?.StopRepair();
        state = PlayerState.Normal;
    }

    void PlayerUpdateRepair()
    {
        if(currentRepairPoint == null) return;
        bool completed = currentRepairPoint.UpdateRepair(Time.deltaTime);

        if (completed)
        {
            state = PlayerState.Normal;
            currentRepairPoint = null;

          　//修復成功スコア
        }
    }

    public void TakeDamage(int damage)
    {
        if(isInvincible)  return;

        currentHP -= damage;
        if(currentHP <= 0)
        {
            Die();
        }

        StartCoroutine(InvincibleCoroutine());
        Vector2 knockDir = (transform.position - Camera.main.transform.position).normalized;
        rb.linearVelocity = knockDir * knockbackForce;
    }

    IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        for (int i = 0; i < 5; i++)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(0.05f);

            sr.enabled = true;
            yield return new WaitForSeconds(0.05f);
        }
        isInvincible = false;
    }

    void Die()
    {
        if (isDead) return;
  
        isDead = true;
        ScoreManager.Instance.Sub(desPenaluty);
        sr.color = new Color(1f, 1f, 1f, 0f);
        col.enabled = false;
        rb.linearVelocity = Vector2.zero;
        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnTime);

        // 位置リセット
        transform.position = respawnPoint.position;

        // HP回復
        currentHP = maxHP;

        // 復活
        sr.color = new Color(1f, 1f, 1f, 1f);
        col.enabled = true;
        isDead = false;
    }

    public bool IsDead()
    {
        return isDead;
    }
}
