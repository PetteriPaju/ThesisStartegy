using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//COntaisn all items that can be seen at InventoryPanel
namespace SengokuWarrior
{
    [System.Serializable]
    public class Inventory
    {
        [SerializeField]
        public List<ItemStack> items = new List<ItemStack>();


        public Inventory() { }
        public Inventory(List<ItemStack> items)
        {
            this.items = items;
        }
        public List<ItemStack> Items
        {
            get
            {
                return items;
            }

            set
            {
                items = value;
            }
        }

        public void Init()
        {

            List<ItemStack> newlist = new List<ItemStack>();
            for (int i=0; i<items.Count; i++)
            {
                CommonAttributes attr = items[i].item.GetAttribute<CommonAttributes>();

                if (attr)
                {
                    if (!attr.isStackable)
                    {
                        for (int q = 0; q < items[i].amount; q++)
                        {
                            newlist.Add(new ItemStack(items[i].item));
                        }
                    }
                    else
                    {
                        newlist.Add(items[i]);
                    }

                }
            }
            items = newlist;
        }

        public void AddItem(NewItem itm)
        {
            CommonAttributes attr = itm.GetAttribute<CommonAttributes>();
            if (attr && attr.isStackable)
            {
                int index = Items.FindIndex(a => a.item == itm);
                if (index != -1)
                {
                    items[index].amount++;
                }
                if (items[index].amount <= 0)
                {
                    items.Add(new ItemStack(itm));
                }
            }
            else
            {
                items.Add(new ItemStack(itm));
            }
        
        }

        public void DepleteItem(NewItem itm)
        {
            int index = Items.FindIndex(a => a.item == itm);
            if (index != -1)
            {
                items[index].amount--;
            }
            if (items[index].amount <= 0)
            {
                items.RemoveAt(index);
            }
        }



    }
}
