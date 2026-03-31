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
    private Rigidbody2D rb;
    Vector2 moveInput;
    PlayerState state;

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
    }

    private void FixedUpdate()
    {
        if (state != PlayerState.Normal)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = moveInput * moveSpeed;
    }
}
