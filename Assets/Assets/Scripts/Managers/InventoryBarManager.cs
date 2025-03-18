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

    [Header("Inventory Bar Settings")]
    public int inventoryBarSize = 8;
    public List<InventorySlot> slots;
    Dictionary<string, InventorySlot> itemsById = new Dictionary<string, InventorySlot>();
    
    [Header("UI References")]
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
            Debug.Log("Slots Created");
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
            Debug.Log("inventoryScrollactionEnabled");
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
        scrollDirection = context.ReadValue<Vector2>();
        OveringSlot(scrollDirection);
        RefreshInventoryBarUI();
    }
    //    public bool AddItem(Item item, int quantity = 1)
    //    {
    //        // Vérifier si l'item est empilable
    //        if (item.isStackable)
    //        {
    //            // Chercher un slot existant avec le même item
    //            for (int i = 0; i < slots.Count; i++)
    //            {
    //                if (slots[i].item == item && slots[i].quantity < item.maxStackSize)
    //                {
    //                    quantity = slots[i].AddItem(item, quantity);
                    
    //                    if (quantity <= 0)
    //                    {
    //                        RefreshInventoryUI();
    //                        return true; // Tout a été ajouté
    //                    }
    //                }
    //            }
    //        }
        
    //        // Chercher un slot vide
    //        for (int i = 0; i < slots.Count; i++)
    //        {
    //            if (slots[i].IsEmpty())
    //            {
    //                quantity = slots[i].AddItem(item, quantity);
                
    //                if (quantity <= 0)
    //                {
    //                    RefreshInventoryUI();
    //                    return true; // Tout a été ajouté
    //                }
    //            }
    //        }
        
    //        // Si on arrive ici, c'est que l'inventaire est plein
    //        Debug.Log("Inventaire plein, impossible d'ajouter " + item.itemName);
    //        RefreshInventoryUI();
    //        return false;
    //    }
    
    //    public void RemoveItem(Item item, int quantity = 1)
    //    {
    //        for (int i = 0; i < slots.Count; i++)
    //        {
    //            if (slots[i].item == item)
    //            {
    //                slots[i].RemoveItem(quantity);
    //                RefreshInventoryUI();
    //                return;
    //            }
    //        }
    //    }
    
    //    public void UseItem(int slotIndex)
    //    {
    //        if (slotIndex >= 0 && slotIndex < slots.Count)
    //        {
    //            if (!slots[slotIndex].IsEmpty())
    //            {
    //                slots[slotIndex].item.Use();
    //                RefreshInventoryUI();
    //            }
    //        }
    //    }
    
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
        }
    }


    
    private void OveringSlot(Vector2 scrolldirection)
    {
        scrollDirection = scrolldirection;
        if(actualSlot+scrollDirection.y>=0f && (actualSlot+scrollDirection.y)<=7){
            actualSlot+=(int)scrollDirection.y;
        }
        
    }
    
}


