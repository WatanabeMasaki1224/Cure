using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerContoroller : MonoBehaviour
{
    public float _moveSpeed = 1;
    public float _jumpPower = 1;
    public int _maxJumpCount = 1;
    private int _jumpCount = 0;
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;
    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;
    private bool isFacingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
}
