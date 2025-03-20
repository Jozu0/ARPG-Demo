        using UnityEngine;


        public enum ItemRarity { Common, Rare, Epic, Legendary }
        public enum ItemType { Weapon, Armor, Potion, Misc, Food }

        
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

        public ItemRarity itemRarity = ItemRarity.Common;
        public int rarity;
        public ItemType itemType =  ItemType.Food;
        public int type;
        public string typeString;

        public bool isStackable = true;
        public int maxStackSize = 99;        
        [Header("Item Value")]
        public int buyPrice;
        public int sellPrice;

        public int damage;
        
        void Start(){
            rarity = (int)itemRarity;
            type = (int)itemType;

        }
        public virtual void Use()
        {
            // La méthode de base que les classes dérivées surchargeront
            Debug.Log("Using item: " + itemName);
        }
        }