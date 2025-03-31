using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;


public class InventorySlotUI : MonoBehaviour , IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI quantityText;

    public Image slotHovering;

    public GameObject heldItem;
    public Image slotBackground;


    public InventorySlot draggedObject;

    private int slotIndex;
    private InventorySlot slotData;

    private bool isDragging = false;
    private Vector2 dragOffset;
    public Canvas parentCanvas;



    void Awake()
    {
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas canvas in canvases)
        {
            if (canvas.name == "InventoryCanvas")
            {
                parentCanvas = canvas;
                break;
            }
        }
    }
    void Update()
    {
        if(isDragging && heldItem != null)
    {
        // For Screen Space - Overlay canvas
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            heldItem.transform.position = Input.mousePosition;
        }
        // For Screen Space - Camera or World Space canvas
        else
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                Input.mousePosition,
                parentCanvas.worldCamera,
                out pos);
            heldItem.transform.position = parentCanvas.transform.TransformPoint(pos);
        }
    }
    }


    public void SetupSlot(int index, InventorySlot data)
    {
        slotIndex = index;
        slotData = data;


        // Mettre à jour l'UI en fonction des données du slot
        if (data.IsEmpty())
        {
            itemIcon.enabled = false;
            quantityText.gameObject.SetActive(false);
        }
        else
        {
            itemIcon.sprite = data.item.icon;
            itemIcon.enabled = true;


            if (data.quantity > 1)
            {
                quantityText.text = data.quantity.ToString();
                quantityText.gameObject.SetActive(true);
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
        }
    }

    public void SetupSlotBar(int index, InventorySlot data, int indexHover)
    {
        slotIndex = index;
        slotData = data;
        if (index == indexHover)
        {
            slotHovering.enabled = true;
        }
        else
        {
            slotHovering.enabled = false;
        }


        // Mettre à jour l'UI en fonction des données du slot
        if (data.IsEmpty())
        {
            itemIcon.enabled = false;
            quantityText.gameObject.SetActive(false);
        }
        else
        {
            itemIcon.sprite = data.item.icon;
            itemIcon.enabled = true;


            if (data.quantity > 1)
            {
                quantityText.text = data.quantity.ToString();
                quantityText.gameObject.SetActive(true);
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Clic droit - Jeter l'item ou afficher des options
            if (!slotData.IsEmpty())
            {
                // Pour l'instant, simplement utiliser l'item
                InventoryManager.Instance.UseItem(slotIndex);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Only start drag if we have an item
            if (!slotData.IsEmpty() && eventData.pointerCurrentRaycast.gameObject != null)
            {
                GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
                
                // Check if we clicked on a valid item icon
                if (clickedObject.name == "ItemIconImage")
                {
                    // Create a draggable copy of the item
                    GameObject draggedItemObj = Instantiate(clickedObject.transform.parent.gameObject, parentCanvas.transform);
                    draggedItemObj.name = "HeldItem";
                    
                    // Setup the dragged item
                    heldItem = draggedItemObj;
                    isDragging = true;
                    
                    // Make the dragged item appear above other UI elements
                    heldItem.transform.SetAsLastSibling();
                    
                    // Optional: make original item semi-transparent to indicate it's being dragged
                    clickedObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging && heldItem != null)
        {
            // Find all UI elements under the pointer
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            // Look for an InventorySlotUI component
            foreach (RaycastResult result in results)
            {
                InventorySlotUI targetSlot = result.gameObject.GetComponentInParent<InventorySlotUI>();
                if (targetSlot != null && targetSlot != this)  // Make sure we don't drop on ourselves
                {
                    InventoryManager.Instance.MoveItem(slotIndex, targetSlot.slotIndex);
                    break;
                }
            }
            
            // Clean up
            Destroy(heldItem);
            heldItem = null;
            isDragging = false;
            
            if (itemIcon != null)
            {
                itemIcon.color = Color.white;
            }
        }
    }
}
