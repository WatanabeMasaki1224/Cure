using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;


public class PlayerContoroller : MonoBehaviour
{
    [Header("ステータス")]
    public float _moveSpeed = 1;
    public float _jumpPower = 1;
    public int _maxJumpCount = 1;
    private int _jumpCount = 0;
    public int maxHP = 10;
    private int currentHP;
    [Header("接地判定")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isGrounded;
    private float moveInput;
    private bool isFacingRight = true;
    [Header("魔法")]
    public GameObject magicBulletPrefub;
    public GameObject rebirthMagicPrefab;    // 再生魔法のプレハブ
    public Transform magicSpawnPoint;        // 再生魔法を出す位置（円形）
    public Transform firePoint;             // 魔法のだす位置
    public int maxMagicUse = 5;
    private int magicUse;
    public float magicCooldown;
    private float lastMagicTime = -Mathf.Infinity;
    public float reviveTime = 5;   //復活魔法の長押し時間
    private bool revive = false;
    private float reviveHoldStartTime = 0f;
    private float reviveTimer = 0;
    [Header("リスポーン")]
    public Transform respawnPoint;
    public float respownTime;
    private bool isDead = false;
    private RebirthMagic rebirthMagic;
    private GameObject currentRebirthMagic;
    public Image hpBarImage;  // ← Unityで紐づける


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        currentHP = maxHP;
        rebirthMagic = GetComponent<RebirthMagic>();
        HPBar();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if(Input.GetKeyDown(KeyCode.Space) && _jumpCount < _maxJumpCount)
        {
            rb.velocity = new Vector2(rb.velocity.x,_jumpPower);
            _jumpCount++;
        }

        if(moveInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if(moveInput < 0 && isFacingRight)
        {
            Flip();
        }

        //通常魔法
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (Time.time - lastMagicTime >= magicCooldown)
            {
                GameObject bullet = Instantiate(magicBulletPrefub, firePoint.position, Quaternion.identity);
                Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;
                bullet.GetComponent<MagicBullet>().SetDirection(dir);
                lastMagicTime = Time.time;
                Debug.Log("魔法発射");
            }
        }
        //再生魔法
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (currentRebirthMagic == null)
            {
                currentRebirthMagic =Instantiate(rebirthMagicPrefab, magicSpawnPoint.position, Quaternion.identity);
                RebirthMagic magic = currentRebirthMagic.GetComponent<RebirthMagic>();
                  if(magic != null)
                {
                    magic.StartHealing();
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.V))
        {
            if (currentRebirthMagic != null)
            {
                RebirthMagic magic = currentRebirthMagic.GetComponent<RebirthMagic>();
                if (magic != null)
                {
                    magic.StopHealing();
                }
                Destroy(currentRebirthMagic);
                currentRebirthMagic=null;
            }
        }

        //復活魔法
        if (Input.GetKeyDown(KeyCode.B))
        {
            reviveHoldStartTime = Time.time;  // 押し始めた時刻を記録
            revive = false;
            Debug.Log("Bキー");
        }

        if (Input.GetKey(KeyCode.B))
        {
            float heldTime = Time.time - reviveHoldStartTime;
            if (!revive && heldTime >= reviveTime)  // reviveTimeは「必要長押し秒数」
            {
                revive = true;
                ReviveNPC();
                Debug.Log("復活");
            }
        }

        if (Input.GetKeyUp(KeyCode.B))
        {
            revive = false;
        }

        //NPCとの会話ボタン
        if (Input.GetKeyDown(KeyCode.M))
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.5f);
            foreach(var hit in hits)
            {
                NPC npc = hit.GetComponent<NPC>();
                if (npc != null && npc.IsRevived()) 
                {
                    string magic = npc.GetMagicName();
                    GetComponent<SpecialMagicController>().SetSpecialMagic(magic);
                    Debug.Log("会話");
                    break;
                   
                }
            }
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius,groundLayer);
        rb.velocity = new Vector2(moveInput * _moveSpeed, rb.velocity.y);
        if (isGrounded &&  rb.velocity.y <= 0.01f)
        {
            _jumpCount = 0;
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scare = transform.localScale;
        scare.x *= -1;
        transform.localScale = scare;
    }

    //プレイヤーのダメージ処理  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                TakeDmage(enemy.damageToPlayer);
            }
        }
    }

    public void TakeDmage (int damage)
    {
        currentHP -= damage;
        Debug.Log(currentHP);
        HPBar();

        if (isDead)
        {
            return;
        }
       

        if (currentHP <= 0)
        {
            Die();
        }
    }

   void Die()
    {
        isDead = true;
        //動きを止める
        rb.velocity = Vector2.zero;
        rb.isKinematic = false;
        col.enabled = false;

        StartCoroutine(RespawnCoroutine());
    }
     
    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respownTime);
        Respown();
    }

    void Respown()
    {
        transform.position = respawnPoint.position;
        currentHP = maxHP;
        HPBar();
        //挙動復活
        rb.isKinematic = false ;
        col.enabled = true ;
        isDead = false ;
    }

    private void HPBar()
    {
        if(hpBarImage != null)
        {
            hpBarImage.fillAmount = (float)currentHP / maxHP;
        }
    }

    void ReviveNPC()
    {
        Debug.Log("ReviveNPCが呼ばれました");
        float revive = 1.5f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, revive);
        foreach(var hit in hits)
        {
            NPC npc = hit.GetComponent<NPC>();
            if(npc != null && !npc.IsRevived())
            {
                npc.Revive();
                Debug.Log("NPCを復活させました");
                break;
            }
        }
        

    }
}
