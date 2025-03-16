
using UnityEngine;
using UnityEngine.InputSystem;


public class MovementInputHandler : InputHandler
{
   [SerializeField] private PlayerController playerController;
   [SerializeField] private PlayerAnimation playerAnimator;


   public Vector2 moveInput;



   protected override void RegisterInputActions()
   {
       PlayerInput playerInput = GetPlayerInput();
       if (playerInput != null)
       {
            playerInput.actions["Move"].performed += OnMovePerformed;
            playerInput.actions["Move"].canceled += OnMoveCanceled;
            playerInput.actions["Sprint"].performed += OnSpeedPerformed;
            playerInput.actions["Sprint"].canceled += OnSpeedCanceled;


       }
       else
       {
           Debug.LogError("PlayerInput is null in MovementInputHandler");
       }
   }
  
   protected override void UnregisterInputActions()
   {
       PlayerInput playerInput = GetPlayerInput();
       if (playerInput != null)
       {
            playerInput.actions["Move"].performed -= OnMovePerformed;
            playerInput.actions["Move"].canceled -= OnMoveCanceled;
            playerInput.actions["Sprint"].performed -= OnSpeedPerformed;
            playerInput.actions["Sprint"].canceled -= OnSpeedCanceled;
       }
   }
  
   // Utilisez des noms différents pour éviter les conflits potentiels
   private void OnMovePerformed(InputAction.CallbackContext context)
   {

       moveInput = context.ReadValue<Vector2>();
       if (playerController != null)
       {
           playerController.SetMoveDirection(moveInput);
       }
       else
       {
           Debug.LogError("PlayerController non assigné dans MovementInputHandler");
       }
   }
  
   private void OnMoveCanceled(InputAction.CallbackContext context)
   {
       moveInput = Vector2.zero;
       if (playerController != null)
       {
           playerController.SetMoveDirection(moveInput);
       }
   }

   private void OnSpeedPerformed(InputAction.CallbackContext context)
   {
        playerController.SpeedMove(8f);
        playerAnimator.SpeedOn(true);
   }

   private void OnSpeedCanceled(InputAction.CallbackContext context)
   {
        playerController.SpeedMove(4f);
        playerAnimator.SpeedOn(false);
   }
}


