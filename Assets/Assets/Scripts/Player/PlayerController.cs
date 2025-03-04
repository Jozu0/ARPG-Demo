
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Références pour les composants
    public Rigidbody2D rb;
    public Animator anim;

    private bool isFacingRight = true;
    public float speed = 4f;

    // Variables pour gérer le mouvement
    private Vector3 movement;
    private Vector2 lastMovementDirection;
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

        // Gestion de l'orientation du sprite
        if (direction.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    // Update est appelé une fois par frame
    void Update()
    {
        ProcessInputs();
        Animate();  
        if (movement.x > 0 && !isFacingRight || movement.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * speed;  // Utilisez rb.velocity au lieu de rb.linearVelocity
    }

    void Flip() // Flip character when direction changes
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        isFacingRight = !isFacingRight; // Corrige le nom de la variable ici
    }

    void ProcessInputs()
    {
        // Récupérer les axes d'entrée (Horizontal et Vertical)
        int moveX = (int)Input.GetAxisRaw("Horizontal");
        int moveY = (int)Input.GetAxisRaw("Vertical");

        // Mémoriser la dernière direction si le joueur est inactif
        if (moveX == 0 && moveY == 0 && (movement.x != 0 || movement.y != 0))
        {
            lastMovementDirection = movement;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize(); // Normaliser le vecteur de mouvement pour éviter les vitesses diagonales plus rapides
    }

    void Animate()
    {
        anim.SetFloat("MoveX", movement.x);
        anim.SetFloat("MoveY", movement.y);
        anim.SetFloat("LastX", lastMovementDirection.x);
        anim.SetFloat("LastY", lastMovementDirection.y);
        anim.SetFloat("MoveMagnitude", movement.magnitude);

        // Gestion de la vitesse de déplacement en fonction de la touche Shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 8;
            anim.SetBool("ShiftPressed", true);
        }
        else
        {
            anim.SetBool("ShiftPressed", false);
            speed = 4;
        }
    }
}
