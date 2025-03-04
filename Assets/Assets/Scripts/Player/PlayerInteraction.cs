
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;


public class PlayerInteraction  : InputHandler
{
   [Header("UI References")]
   public GameObject interactionPromptPanel;
   public TextMeshProUGUI interactionPromptText;
  
   // L'interactable actuellement à portée
   private IInteractable currentInteractable;
  
   private void Awake()
   {
       // S'assurer que le prompt est désactivé au démarrage
       if (interactionPromptPanel != null)
       {
           interactionPromptPanel.SetActive(false);
       }
      
   }


   protected override void RegisterInputActions()
   {
       PlayerInput playerInput = GetPlayerInput();
       if (playerInput != null)
       {
           playerInput.actions["Interact"].started += OnInteract;
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
           playerInput.actions["Interact"].started -= OnInteract;
       }
   }


   private void OnInteract(InputAction.CallbackContext context)
   {
       if (currentInteractable != null)
       {
           currentInteractable.Interact();
       }
   }
  
   private void OnTriggerEnter2D(Collider2D other)
   {
       IInteractable interactable = other.GetComponent<IInteractable>();
          
       currentInteractable = interactable;
      
       if (currentInteractable != null && interactionPromptPanel != null)
       {
           interactionPromptPanel.SetActive(true);
           interactionPromptText.text = currentInteractable.GetInteractionPrompt();
       }
   }
      
   private void OnTriggerExit2D(Collider2D other)
   {
       IInteractable interactable = other.GetComponent<IInteractable>();
          
       if (currentInteractable == interactable)
       {
           currentInteractable = null;
          
           if (interactionPromptPanel != null)
           {
                 interactionPromptPanel.SetActive(false);
           }
       }
   }
  
}
