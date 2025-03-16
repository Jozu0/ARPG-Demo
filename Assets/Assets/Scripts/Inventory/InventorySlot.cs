using UnityEngine;




[System.Serializable]
public class InventorySlot
{
   public Item item;
   public int quantity;
  
   public InventorySlot()
   {
       item = null;
       quantity = 0;
   }
  
   public InventorySlot(Item item, int quantity)
   {
       this.item = item;
       this.quantity = quantity;
   }
  
   public bool IsEmpty()
   {
       return item == null || quantity <= 0;
   }
  
   public void Clear()
   {
       item = null;
       quantity = 0;
   }
  
   public bool CanAddItem(Item itemToAdd)
   {
       return item == null || (item == itemToAdd && itemToAdd.isStackable && quantity < itemToAdd.maxStackSize);
   }
  
   public int AddItem(Item itemToAdd, int quantityToAdd)
   {
       if (item == null)
       {
           item = itemToAdd;
           quantity = quantityToAdd;
           return 0; // Tout a été ajouté
       }
      
       if (item == itemToAdd && itemToAdd.isStackable)
       {
           int spaceLeft = itemToAdd.maxStackSize - quantity;
           int added = Mathf.Min(quantityToAdd, spaceLeft);
          
           quantity += added;
           return quantityToAdd - added; // Retourne la quantité restante
       }
      
       return quantityToAdd; // Rien n'a été ajouté
   }
  
   public void RemoveItem(int quantityToRemove)
   {
       quantity -= quantityToRemove;
      
       if (quantity <= 0)
      {
           Clear();
      }
   }
}
