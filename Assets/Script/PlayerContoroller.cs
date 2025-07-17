using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;  // UI�\���p

public class PlayerContoroller : MonoBehaviour
{
    public float _moveSpeed = 1;
    public float _jumpPower = 1;
    public int _maxJumpCount = 1;
    private int _jumpCount = 0;
    public int maxHP = 10;
    private int currentHP;
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isGrounded;
    private float moveInput;
    private bool isFacingRight = true;
    public GameObject magicBulletPrefub;
    public Transform firePoint;
    public Transform respawnPoint;
    public float respownTime;
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        currentHP = maxHP;
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
        
        if(Input.GetKeyDown(KeyCode.Z))
        {
            GameObject bullet = Instantiate(magicBulletPrefub,firePoint.position,Quaternion.identity);
            Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;
            bullet.GetComponent<MagicBullet>().SetDirection(dir);
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

    //�v���C���[�̃_���[�W����  
    private void OnCollisionEnter2D(Collision2D collision)
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
        //�������~�߂�
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
        //��������
        rb.isKinematic = false ;
        col.enabled = true ;
        isDead = false ;
    }
}
