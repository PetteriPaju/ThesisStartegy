using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
namespace SengokuWarrior
{
    [CreateAssetMenu(menuName = "Tactics/Items/CreateDatabase", fileName = "Itemdatabase")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField]
        public List<NewItem> items = new List<NewItem>();
        private Dictionary<string, NewItem> d_items = new Dictionary<string, NewItem>();

        private string[] itemNames = null;

        public string[] ItemNames
        {
            get
            {
                if (itemNames==null || itemNames.Length != items.Count)
                {
                    itemNames = items.Select(x => x.ItemName).ToArray();
                }

                return itemNames;
            }
        }


        public NewItem[] GetItemsWithSlot(int i)
        {

            NewItem[] itms = items.Where(x => x.GetAttribute<GearSlotAttribute>()).ToArray();
            NewItem[] slotItems = itms.Where(x => x.GetAttribute<GearSlotAttribute>().PossibleSlots[i]).ToArray();


            return slotItems;
        }

        public void AddItem(NewItem itm)
        {
            itm.GenereateNewId();
            items.Add(itm);
            RefreshDictionary();

        }

        public void Destroy(NewItem itm)
        {
            items.Remove(itm);
            GameObject.DestroyImmediate(itm,true);
            RefreshDictionary();
        }

        private void Sort()
        {
            items.Sort((a, b) => (a.ItemName.CompareTo(b.ItemName)));
        }
        private Dictionary<string, NewItem> D_items
        {
            get
            {
                if (items.Count != d_items.Count)
                {
                    RefreshDictionary();
                }
                return d_items;
            }
        }
        public void RefreshDictionary()
        {
            d_items.Clear();
            for (int i = 0; i < d_items.Count; i++)
            {
                d_items.Add(items[i].ItemName, items[i]);
            }
               itemNames = items.Select(x => x.ItemName).ToArray();
        }


    }
}

 

