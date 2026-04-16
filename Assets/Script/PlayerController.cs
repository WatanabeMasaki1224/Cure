using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

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
    [SerializeField] Text hpText;
    Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        currentHP = maxHP;
        HPUI();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;
        anim.SetFloat("Speed", Mathf.Abs(moveInput.x));
        anim.SetBool("IsGround", isGround);

        
        if (moveInput.x != 0)
        {
            lastDirection = new Vector2(Mathf.Sign(moveInput.x), 0);
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(moveInput.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
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
            anim.SetTrigger("Attack");
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
        if (isDead) return;

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
        }
    }

    public void TakeDamage(int damage)
    {
        if(isInvincible)  return;

        currentHP -= damage;
        HPUI();
        anim.SetTrigger("Hit");

        if (currentHP <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibleCoroutine());
    }

    IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(1.0f); // 無敵時間
        isInvincible = false;
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        ScoreManager.Instance.Sub(desPenaluty);

        anim.SetTrigger("Dead");

        col.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false; 

        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnTime);

        transform.position = respawnPoint.position;
        currentHP = maxHP;

        anim.Rebind();
        anim.Update(0f);
        rb.simulated = true;
        col.enabled = true;
        isDead = false;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void HPUI()
    {
        hpText.text = "HP: " + currentHP;
    }
}
