    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.UI;
    using System.Linq;

    public class InventoryBarManager : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public InputActionReference inventoryBarScrollAction;

    public Image prefabOveringImage;

    public InventoryManager mainInventoryManager;


    Vector2 scrollDirection;
    int actualSlot = 0;
    private float scrollCooldown = 0.05f; 
    private float lastScrollTime = 0f;

    [Header("Inventory Bar Settings")]
    public int inventoryBarSize = 8;
    public List<InventorySlot> slots;
    Dictionary<string, InventorySlot> itemsById = new Dictionary<string, InventorySlot>();
    
    [Header("UI References")]
    public Animator playerAnimator;
    public GameObject inventoryBarPanel;
    public Transform slotsGrid; // Le parent où les slots seront instanciés
    public GameObject slotPrefab; // Le prefab pour un emplacement d'inventaire

    public static InventoryBarManager Instance { get; private set; }
    private void Awake()
    {
        
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        } 
        Instance = this;

        // Initialiser les slots à partir de mainInventoryManager
        if (mainInventoryManager != null)
        {
            slots = mainInventoryManager.slots.Take(inventoryBarSize).ToList();
        }
        else
        {
            slots = new List<InventorySlot>();
        }

        if (inventoryBarScrollAction != null)
        {
            inventoryBarScrollAction.action.started += OnInventoryOveringChangeInput;
        }

        prefabOveringImage = slotPrefab.GetComponent<Image>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerAnimator = player.GetComponent<Animator>();
    }

    private void Start()
    {
        // Créer les slots UI
        RefreshInventoryBarUI();
    }


    private void OnEnable()
    {
        // Activer l'action d'input
        if (inventoryBarScrollAction != null)
        {
            inventoryBarScrollAction.action.Enable();
        }
    }


    private void OnDisable()
    {
        // Désactiver l'action d'input
        if (inventoryBarScrollAction != null)
        {
            inventoryBarScrollAction.action.Disable();
        }
    }
    private void OnDestroy()
    {
        // Nettoyer les abonnements d'événements
        if (inventoryBarScrollAction != null)
        {
            inventoryBarScrollAction.action.started -= OnInventoryOveringChangeInput;
        }
    }
    private void OnInventoryOveringChangeInput(InputAction.CallbackContext context){
        if (Time.time - lastScrollTime >= scrollCooldown)
        {
            scrollDirection = context.ReadValue<Vector2>();
            OveringSlot(scrollDirection);
            RefreshInventoryBarUI();
            lastScrollTime = Time.time;
        }
    }
    public void UseItem(int slotIndex)
    {
    if (slotIndex >= 0 && slotIndex < slots.Count)
       {
        if (!slots[slotIndex].IsEmpty())
            {
                slots[slotIndex].item.Use();
                RefreshInventoryBarUI();
            }
        }
    }
    
    public void RefreshInventoryBarUI()
    {
        // Si l'UI n'est pas encore initialisée, retourner
        if (slotsGrid == null || slotPrefab == null)
        {
            return;
        }

        slots = mainInventoryManager.slots.Take(inventoryBarSize).ToList();
            
        // Supprimer tous les slots actuels pour les recréer
        foreach (Transform child in slotsGrid)
        {
            Destroy(child.gameObject);
        }
        
        // Créer de nouveaux slots
        for (int i = 0; i < slots.Count; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotsGrid);
            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
            
            if (slotUI != null)
            {
                slotUI.SetupSlotBar(i, slots[i], actualSlot);
            }
            ChangeItemState(actualSlot);
        }
    }


    
    private void OveringSlot(Vector2 scrolldirection)
    {
        scrollDirection = scrolldirection;

        int previousSlot = actualSlot;
        
        // Add the scroll direction and handle wrapping
        actualSlot = (actualSlot + (int)scrollDirection.y) % inventoryBarSize;
        
        // Handle negative numbers (C# modulo can return negative)
        if (actualSlot < 0)
            actualSlot += inventoryBarSize;
        
        // Only change item state if the slot actually changed
        if (previousSlot != actualSlot)
        {
            ChangeItemState(actualSlot);
        }
            
    }
    private void ChangeItemState(int hoveredIndex)
    {
        if(slots[hoveredIndex].item){
            Item itemInHand = slots[hoveredIndex].item;
            switch(itemInHand.typeString)
            {
                case "Sword":
                    playerAnimator.SetBool("SwordEquipped",true);
                    playerAnimator.SetBool("PickaxeEquipped",false);
                    break;
                case "Pickaxe":
                    playerAnimator.SetBool("SwordEquipped",false);
                    playerAnimator.SetBool("PickaxeEquipped",true);
                    break;
                default:
                    playerAnimator.SetBool("SwordEquipped",false);
                    playerAnimator.SetBool("PickaxeEquipped",false);
                    break;
            }

        }else{
            playerAnimator.SetBool("SwordEquipped",false);
            playerAnimator.SetBool("PickaxeEquipped",false);
        }
    }
    
    
}


