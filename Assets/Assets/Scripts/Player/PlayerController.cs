
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Références pour les composants
    public Rigidbody2D rb;

    private bool isFacingRight = true;
    public float moveSpeed = 4f;

    // Variables pour gérer le mouvement
    private Vector2 moveDirection; // Ajoutez cette variable pour stocker la direction du mouvement

    private void Start()
    {
        // Récupérer le PlayerInput et l'enregistrer auprès de l'InputManager
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.SetPlayerInput(playerInput);
            }
            else
            {
                Debug.LogError("InputManager is not in the scene");
            }
        }
        else
        {
            Debug.LogError("Missing PlayerInput on GameObject");
        }
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;

        if (direction.y != 0)
        {
            ResetFlip();
        }
        // Gestion de l'orientation du sprite
        if (direction.x > 0 && !isFacingRight && direction.y==0)
        {
            Flip();
        }
        else if (direction.x < 0 && isFacingRight && direction.y==0)
        {
            Flip();
        }
        if(direction.y!=0){
            
        }
    }

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    void Update(){
    }

    private void FixedUpdate()
   {
       // On utilise FixedUpdate pour le mouvement physique
       Move();
   }
  
   // ReSharper disable Unity.PerformanceAnalysis
   private void Move()
   {
       // Mouvement avec Rigidbody2D pour une meilleure physique
        if (rb)
        {   
            rb.linearVelocity = moveDirection* moveSpeed;
        }
        else
        {
            Debug.LogError("Rigidbody2D is missing on PlayerController");
        }
   }

    public void SpeedMove(float speed)
    {
        if(rb){
            moveSpeed = speed ;

        }
        else
        {
           Debug.LogError("Rigidbody2D is missing on PlayerController");
        }
    }

    void Flip() // Flip character when direction changes
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        isFacingRight = !isFacingRight;
    }

    // New function to reset the scale to the original value
    void ResetFlip()
    {
        Vector3 scale = transform.localScale;
        scale.x = 1f; // Original scale value
        transform.localScale = scale;
        isFacingRight = true; // Assuming right is the default facing direction
    }
        

    }
