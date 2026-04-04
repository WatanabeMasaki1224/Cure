using UnityEngine;

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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   
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

        if(Input.GetKeyDown(KeyCode.F))
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
        if (state == PlayerState.Attack) return;
        if (currentRepairPoint == null) return;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Repair"))
        {
            currentRepairPoint = other.GetComponent<RepairPoint>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Repair"))
        {
            currentRepairPoint = null;
            PlayerStopRepair();
        }
    }
}
