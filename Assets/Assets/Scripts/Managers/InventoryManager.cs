using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InventoryManager : MonoBehaviour
{

    [Header("Input Settings")]
    public InputActionReference inventoryAction;

    [Header("Inventory Settings")]
    public int inventorySize = 32;
    public List<InventorySlot> slots = new List<InventorySlot>();
    Dictionary<string, InventorySlot> itemsById = new Dictionary<string, InventorySlot>();
    [Header("UI References")]
    public GameObject inventoryPanel;


    public InventoryBarManager inventoryBarManager;
    public Transform slotsGrid; // Le parent où les slots seront instanciés
    public GameObject slotPrefab; // Le prefab pour un emplacement d'inventaire

    private bool isInventoryOpen = false;
    public GameObject inventoryBar;
    public GameObject lifeBar;

    public static InventoryManager Instance { get; private set; }
    private void Awake()
    {

        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;


        // Initialiser les slots d'inventaire
        for (int i = 0; i < inventorySize; i++)
        {
            slots.Add(new InventorySlot());
        }

        // Fermer l'inventaire au démarrage
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        // Configurer l'action d'input pour ouvrir l'inventaire
        if (inventoryAction != null)
        {
            inventoryAction.action.started += OnInventoryInput;
        }
    }


    private void Start()
    {
        // Créer les slots UI
        RefreshInventoryUI();
    }

    private void Update()
    {
    }

    private void OnEnable()
    {
        // Activer l'action d'input
        if (inventoryAction != null)
        {
            inventoryAction.action.Enable();
        }
    }


    private void OnDisable()
    {
        // Désactiver l'action d'input
        if (inventoryAction != null)
        {
            inventoryAction.action.Disable();
        }
    }
    private void OnDestroy()
    {
        // Nettoyer les abonnements d'événements
        if (inventoryAction != null)
        {
            inventoryAction.action.started -= OnInventoryInput;
        }
    }
    private void OnInventoryInput(InputAction.CallbackContext obj)
    {
        ToggleInventory();
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;


        if (inventoryPanel)
        {
            inventoryPanel.SetActive(isInventoryOpen);
            inventoryBar.SetActive(!isInventoryOpen);
           lifeBar.SetActive(!isInventoryOpen);
        }
    }

    public bool AddItem(Item item, int quantity = 1)
    {
        // Vérifier si l'item est empilable
        if (item.isStackable)
        {
            // Chercher un slot existant avec le même item
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].item == item && slots[i].quantity < item.maxStackSize)
                {
                    quantity = slots[i].AddItem(item, quantity);

                    if (quantity <= 0)
                    {
                        RefreshInventoryUI();
                        if (InventoryBarManager.Instance != null)
                        {
                            InventoryBarManager.Instance.RefreshInventoryBarUI();
                        }
                        return true; // Tout a été ajouté
                    }
                }
            }
        }

        // Chercher un slot vide
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty())
            {
                quantity = slots[i].AddItem(item, quantity);

                if (quantity <= 0)
                {
                    RefreshInventoryUI();
                    if (InventoryBarManager.Instance != null)
                    {
                        InventoryBarManager.Instance.RefreshInventoryBarUI();
                    }
                    return true; // Tout a été ajouté
                }
            }
        }

        // Si on arrive ici, c'est que l'inventaire est plein
        Debug.Log("Inventaire plein, impossible d'ajouter " + item.itemName);
        RefreshInventoryUI();
        return false;
    }

    public void RemoveItem(Item item, int quantity = 1)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == item)
            {
                slots[i].RemoveItem(quantity);
                RefreshInventoryUI();
                if (InventoryBarManager.Instance != null)
                {
                    InventoryBarManager.Instance.RefreshInventoryBarUI();
                }
                return;
            }
        }
    }

    public void UseItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Count)
        {
            if (!slots[slotIndex].IsEmpty())
            {
                slots[slotIndex].item.Use();
                if (InventoryBarManager.Instance != null)
                {
                    InventoryBarManager.Instance.RefreshInventoryBarUI();
                }
                RefreshInventoryUI();
            }
        }
    }

    public void GrabItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Count)
        {
            if (!slots[slotIndex].IsEmpty())
            {
            }
        }
    }


    private void RefreshInventoryUI()
    {
        // Si l'UI n'est pas encore initialisée, retourner
        if (slotsGrid == null || slotPrefab == null)
        {

            return;
        }

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
                slotUI.SetupSlot(i, slots[i]);
            }
            else
            {

            }
        }
    }

    public void MoveItem(int sourceSlotIndex, int targetSlotIndex)
    {
        // Ensure valid indices
        if (sourceSlotIndex < 0 || sourceSlotIndex >= slots.Count || 
            targetSlotIndex < 0 || targetSlotIndex >= slots.Count || 
            sourceSlotIndex == targetSlotIndex)
        {
            return;
        }
        
        InventorySlot sourceSlot = slots[sourceSlotIndex];
        InventorySlot targetSlot = slots[targetSlotIndex];
        
        // If source slot is empty, nothing to move
        if (sourceSlot.IsEmpty())
        {
            return;
        }
        
        // If target slot is empty, simply move the item
        if (targetSlot.IsEmpty())
        {
            // Move the entire stack
            targetSlot.item = sourceSlot.item;
            targetSlot.quantity = sourceSlot.quantity;
            
            // Clear source slot
            sourceSlot.item = null;
            sourceSlot.quantity = 0;
        }
        // If target has the same item and is stackable
        else if (targetSlot.item == sourceSlot.item && sourceSlot.item.isStackable)
        {
            // Calculate how many items can be moved
            int spaceInTarget = sourceSlot.item.maxStackSize - targetSlot.quantity;
            int amountToMove = Mathf.Min(spaceInTarget, sourceSlot.quantity);
            
            if (amountToMove > 0)
            {
                targetSlot.quantity += amountToMove;
                sourceSlot.quantity -= amountToMove;
                
                // If source is now empty, clear the item reference
                if (sourceSlot.quantity <= 0)
                {
                    sourceSlot.item = null;
                    sourceSlot.quantity = 0;
                }
            }
        }
        // If target has a different item, swap them
        else
        {
            // Swap items
            Item tempItem = targetSlot.item;
            int tempQuantity = targetSlot.quantity;
            
            targetSlot.item = sourceSlot.item;
            targetSlot.quantity = sourceSlot.quantity;
            
            sourceSlot.item = tempItem;
            sourceSlot.quantity = tempQuantity;
        }
        
        // Refresh UI after moving items
        RefreshInventoryUI();
        if (InventoryBarManager.Instance != null)
        {
            InventoryBarManager.Instance.RefreshInventoryBarUI();
        }
    }
}

