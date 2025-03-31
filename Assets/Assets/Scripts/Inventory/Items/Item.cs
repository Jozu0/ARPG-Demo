using UnityEngine;




[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Item Basic Info")]
    public string itemName;
    public Sprite icon;
    [TextArea(3, 10)]
    public string description;
    public int id;

    [Header("Item Properties")]

    //public ItemRarity itemRarity = ItemRarity.Common;
    [Tooltip("Rarity : Common : 0, Rare : 1, Epic = 2, Legendary = 3")] public int rarity;
    //public ItemType itemType =  ItemType.Food;
    [Tooltip("Money = 0, Weapon = 1, Armor = 2, Potion = 3, Misc = 4, Food = 5")] public int type;
    public string typeString;
    public bool consommable;
    public int lifeGainQuantity;
    public bool isStackable = true;
    public int maxStackSize = 99;

    [Header("Item Value")]
    public int buyPrice;
    public int sellPrice;

    public int damage;

    void Start()
    {
    }
    public virtual void Use()
    {
        // La méthode de base que les classes dérivées surchargeront
        Debug.Log("Using item: " + itemName);
    }
}