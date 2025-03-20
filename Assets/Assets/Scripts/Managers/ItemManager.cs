using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ItemManager : MonoBehaviour 
{
    public Chest chest;
    
    // Nested dictionary for combined type/rarity/id lookups
    private Dictionary<int, Dictionary<int, Dictionary<int, Item>>> itemsByTypeRarityId = 
        new Dictionary<int, Dictionary<int, Dictionary<int, Item>>>();
        
    // Direct lookup dictionaries for single-parameter searches
    private Dictionary<int, Item> itemsById = new Dictionary<int, Item>();
    private Dictionary<int, List<Item>> itemsByType = new Dictionary<int, List<Item>>();
    private Dictionary<int, List<Item>> itemsByRarity = new Dictionary<int, List<Item>>();
    
    // Dictionary for combined type/rarity lookups
    private Dictionary<(int, int), List<Item>> itemsByTypeAndRarity = new Dictionary<(int, int), List<Item>>();
    
    void Start()
    {
        InitializeItemDictionary();
    }
    
    void InitializeItemDictionary()
    {
        List<Item> sourceItems = chest.chestItems;
        
        if (sourceItems == null || sourceItems.Count == 0)
        {
            Debug.LogWarning("No items found in chest to initialize dictionary!");
            return;
        }
        
        foreach (Item item in sourceItems)
        {
            // Add to the nested dictionary structure
            if (!itemsByTypeRarityId.ContainsKey(item.type))
            {
                itemsByTypeRarityId[item.type] = new Dictionary<int, Dictionary<int, Item>>();
            }
            
            if (!itemsByTypeRarityId[item.type].ContainsKey(item.rarity))
            {
                itemsByTypeRarityId[item.type][item.rarity] = new Dictionary<int, Item>();
            }
            
            itemsByTypeRarityId[item.type][item.rarity][item.id] = item;
            
            // Add to ID-only dictionary
            itemsById[item.id] = item;
            
            // Add to type-only dictionary
            if (!itemsByType.ContainsKey(item.type))
            {
                itemsByType[item.type] = new List<Item>();
            }
            itemsByType[item.type].Add(item);
            
            // Add to rarity-only dictionary
            if (!itemsByRarity.ContainsKey(item.rarity))
            {
                itemsByRarity[item.rarity] = new List<Item>();
            }
            itemsByRarity[item.rarity].Add(item);
            
            // Add to type/rarity dictionary
            var typeRarityKey = (item.type, item.rarity);
            if (!itemsByTypeAndRarity.ContainsKey(typeRarityKey))
            {
                itemsByTypeAndRarity[typeRarityKey] = new List<Item>();
            }
            itemsByTypeAndRarity[typeRarityKey].Add(item);
        }
    }

    
    // Get by ID 
    public Item GetItem(int id)
    {
        return itemsById.ContainsKey(id) ? itemsById[id] : null;
    }
    
    // Get by type, rarity, and ID 
    public Item GetItem(int type, int rarity, int id)
    {
        if (itemsByTypeRarityId.ContainsKey(type) && 
            itemsByTypeRarityId[type].ContainsKey(rarity) && 
            itemsByTypeRarityId[type][rarity].ContainsKey(id))
        {
            return itemsByTypeRarityId[type][rarity][id];
        }
        
        return null;
    }
    
    // Get by type and rarity
    public List<Item> GetItems(int type, int rarity)
    {
        var key = (type, rarity);
        return itemsByTypeAndRarity.ContainsKey(key) ? 
            new List<Item>(itemsByTypeAndRarity[key]) : new List<Item>();
    }
    
    // Get by type only
    public List<Item> GetItemsByType(int type)
    {
        return itemsByType.ContainsKey(type) ? 
            new List<Item>(itemsByType[type]) : new List<Item>();
    }
    
    // Get by rarity only 
    public List<Item> GetItemsByRarity(int rarity)
    {
        return itemsByRarity.ContainsKey(rarity) ? 
            new List<Item>(itemsByRarity[rarity]) : new List<Item>();
    }
}




[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> allItems = new List<Item>();
}