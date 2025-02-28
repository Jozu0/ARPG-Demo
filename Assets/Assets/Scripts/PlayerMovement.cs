using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public Rigidbody2D rb;

    public float speed = 1f;
    private float horizontal;
    private float vertical;
    private Vector3 movement;

    void Start()
    {
        rb.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame

    void Update() {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }
    void FixedUpdate()
    {
        movement = new Vector3(horizontal, vertical, 0);
        rb.linearVelocity = new Vector3(movement.x * speed, movement.y * speed);
    }
}
