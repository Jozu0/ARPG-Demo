using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private MovementInputHandler playerMovementInputHandler;
    [SerializeField] private Animator playerAnimator;


    private Vector2 lastMovementDirection;


    void Start()
    {
        lastMovementDirection = playerMovementInputHandler.moveInput;
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
    
        playerAnimator.SetFloat("MoveX", moveDirection.x);
        playerAnimator.SetFloat("MoveY", moveDirection.y);
        playerAnimator.SetFloat("LastX", lastMovementDirection.x);
        playerAnimator.SetFloat("LastY", lastMovementDirection.y);
        playerAnimator.SetFloat("MoveMagnitude", moveDirection.magnitude);
        
 
    }

    public void SpeedOn(bool run)
    {
        if(playerAnimator){
            playerAnimator.SetBool("Run", run);

        }
        else
        {
           Debug.LogError("Animator is missing on PlayerController");
        }
    }
    
}
