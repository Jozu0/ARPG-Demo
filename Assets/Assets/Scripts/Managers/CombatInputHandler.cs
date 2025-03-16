using UnityEngine;
using UnityEngine.InputSystem;


public class CombatInputHandler : InputHandler
{
   private PlayerCombatSystem playerCombatSystem;
  
   private void Awake()
   {
       playerCombatSystem = GetComponent<PlayerCombatSystem>();  
       if (playerCombatSystem == null)
       {
           Debug.LogError("PlayerCombatSystem component not found on CombatInputHandler!");
       }
   }
  
   protected override void RegisterInputActions()
   {
       PlayerInput playerInput = GetPlayerInput();
       if (playerInput != null)
       {
           playerInput.actions["Attack1"].started += OnAttack1;
           playerInput.actions["Attack2"].started += OnAttack2;

       }
       else
       {
           Debug.LogError("PlayerInput is null in CombatInputHandler");
       }
   }
  
   protected override void UnregisterInputActions()
   {
       PlayerInput playerInput = GetPlayerInput();
       if (playerInput != null)
       {
           playerInput.actions["Attack1"].started -= OnAttack1;
           playerInput.actions["Attack2"].started -= OnAttack2;
       }
   }
  
   private void OnAttack1(InputAction.CallbackContext context)
   {
       if (playerCombatSystem != null)
       {
           playerCombatSystem.Attack1();
       }
   }

   private void OnAttack2(InputAction.CallbackContext context)
   {
    if(playerCombatSystem != null)
    {
        playerCombatSystem.Attack2();
    }
   }
}