using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public Rigidbody2D rb;
    public Animator anim;

    private bool facingRight = true;
    public float speed = 4f;
    private Vector3 movement;
    private Vector2 lastMovementDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame

    void Update() {
        ProcessInputs();
        Animate();
        if(movement.x>0 && !facingRight ||movement.x<0 && facingRight){
            Flip();
        }
    }
    void FixedUpdate()
    {
        rb.linearVelocity = movement*speed;
    }

    void Flip(){ //Flip character when direction change
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }

    void ProcessInputs(){
        int moveX = (int)Input.GetAxisRaw("Horizontal");
        int moveY = (int)Input.GetAxisRaw("Vertical");
        if((moveX == 0 && moveY == 0) &&  (movement.x !=0 || movement.y != 0)){
            lastMovementDirection = movement;
        }
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();
    }

    void Animate(){
        anim.SetFloat("MoveX",movement.x);
        anim.SetFloat("MoveY",movement.y);
        anim.SetFloat("LastX",lastMovementDirection.x);
        anim.SetFloat("LastY",lastMovementDirection.y);
        anim.SetFloat("MoveMagnitude",movement.magnitude);
        if(Input.GetKey(KeyCode.LeftShift)){
            speed=8;
            anim.SetBool("ShiftPressed",true);
            Debug.Log("run");
        }else{
            anim.SetBool("ShiftPressed",false);
            speed=4;

        }
    }
}
