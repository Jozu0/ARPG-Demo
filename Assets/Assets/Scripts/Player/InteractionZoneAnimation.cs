using UnityEngine;

public class InteractionZoneAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private MovementInputHandler playerMovementInputHandler;
    [SerializeField] private Animator playerAnimator;


    private Vector2 lastMovementDirection;


    void Start()
    {
        lastMovementDirection = playerMovementInputHandler.moveInput;
        playerAnimator.SetFloat("LastX", 0);
        playerAnimator.SetFloat("LastY", 0);
 
    }

    void Update()
    {
        Animate();
    }
    void Animate()
    {
        Vector2 moveDirection = playerMovementInputHandler.moveInput;
        if ((moveDirection.x != 0 || moveDirection.y != 0))
        {
            lastMovementDirection = moveDirection;
        }
        playerAnimator.SetFloat("LastX", lastMovementDirection.x);
        playerAnimator.SetFloat("LastY", lastMovementDirection.y);
 
    }
}

