using UnityEngine;

namespace SengokuWarrior
{
    [System.Serializable]
    public class ItemStack
    {
        public NewItem item;
        public int amount = 1;


        public bool DepleteAmount()
        {
            amount--;
            return amount <= 0;
        }


        public ItemStack(NewItem itm)
        {
            item = itm;
            amount = 1;
        }


        public ItemStack Duplicate()
        {
            ItemStack newStack = new ItemStack(item);
            newStack.amount = this.amount;

            return newStack;
        }

    }
}

