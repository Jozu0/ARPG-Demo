using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ChestInteraction : MonoBehaviour, IInteractable
{
    public Chest chest;
    public ItemManager itemManager;
    SpriteRenderer spriteRenderer;
    BoxCollider2D bc2D;
    System.Random rand;
    public Sprite openedChest;
    public Sprite closedChest;
    
    public void Start()
    
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = closedChest;
        bc2D = this.gameObject.GetComponent<BoxCollider2D>();
        rand = new System.Random();
    } 
  
    public string GetInteractionPrompt()
    {
        return "Appuyez sur E pour ouvrir " + chest.chestName;
    }
    public void Interact()
    {
        Open();
    }
    
    private void Open()
    {
        int rarity = getRarity();
        List<Item> itemListInChest = itemManager.GetItemsByRarity(rarity);
        
        if (itemListInChest.Count == 0)
        {
            Debug.LogWarning($"Aucun item trouvé pour la rareté {rarity} !");
            return;
        }

        int itemIndex = rand.Next(0, itemListInChest.Count-1); 
        Item itemInChest = itemListInChest[itemIndex];


        bool wasPickedUp = InventoryManager.Instance.AddItem(itemInChest);
        Debug.Log(itemInChest.itemName);
        
        if (wasPickedUp)
        {
            // Jouer un son ou un effet de particule ici
            
            // Détruire l'objet dans le monde
            spriteRenderer.sprite = openedChest;
            bc2D.enabled = false;
    

        }
        else
        {
            Debug.Log("Votre inventaire est plein !");
        }
    }

    private int getRarity()
    {   // Pas opti donc nécessaire de mettre 
        // au moins un item de chaque rareté dans un coffre
        int[] weightedNumbers = {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 60%
            1, 1, 1, 1, 1, // 25%
            2, 2, // 10%
            3  // 5%
        };
        return weightedNumbers[rand.Next(weightedNumbers.Length)];
    }
}
