    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.Rendering;


    [CreateAssetMenu(fileName = "New Chest", menuName = "Inventory/Chest")]
    public class Chest : ScriptableObject    
    {
        [Header("Chest Basic Info")]
        public string chestName;
        [TextArea(3, 10)]
        public string description;
    
        [Header("Chest content")]
        public List<Item> chestItems = new List<Item>();    

    }