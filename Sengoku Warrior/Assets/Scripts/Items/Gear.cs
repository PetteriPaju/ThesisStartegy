using System;
using System.Collections.Generic;
using UnityEngine;


namespace SengokuWarrior
{
    [Serializable]
    public class Gear
    {
        [SerializeField]                          //Helmet,Plate,Leg,LeftA,RightA,Ring
        protected NewItem[] gear = new NewItem[6] { null, null, null, null, null, null };
        [SerializeField]
        protected NewItem[] gearNames = new NewItem[6] { null, null, null, null, null, null };


        public NewItem[] items
        {
            get
            {
                return gear;
            }
            set
            {
                gear = value;
            }
        }


        public NewItem[] Names
        {
            get
            {
                return gearNames;
            }
            set
            {
                gearNames = value;
            }
        }


        public void Equip(NewItem itm, Inventory inv)
        {
            int firstEquip = -1;
            int firstFreeEquip = -1;
            GearSlotAttribute attr = itm.GetAttribute<GearSlotAttribute>();

            //Look for thew first open slot and unequip all future slots
            for (int i = 0; i < attr.PossibleSlots.Length; i++)
            {
                if (attr.PossibleSlots[i])
                {
                    if (firstEquip == -1) firstEquip = i;
                    if (items[i] == null && firstFreeEquip == -1) firstFreeEquip = i;
                }
            }
            
            int slot = firstFreeEquip == -1 ? firstEquip : firstFreeEquip;

            Equip(itm, slot, inv);
        }

        public void Equip(NewItem itm, int slot, Inventory inv)
        {   
                GearSlotAttribute attr = itm.GetAttribute<GearSlotAttribute>();

                if (attr)
                {
                    //Unequip item slots
                    for (int i = 0; i < attr.UnequipLots.Length; i++)
                    {
                        if (attr.UnequipLots[i])if (gear[i])Unequip(i, inv); 
                    }
                }

            //Unequip item that is currently in the slot
            if (gear[slot])
            {
                NewItem lastItem = gear[slot];
                GearSlotAttribute attrLast = lastItem.GetAttribute<GearSlotAttribute>();
                for (int q = 0; q < attrLast.UnequipLots.Length; q++)
                {
                    if (attr.UnequipLots[q])
                    {
                        gear[q] = null;
                        gearNames[q] = null;
                    }
                }

                gear[slot] = null;
            }

            gear[slot] = itm;

            //Fill other slots
            for (int i = 0; i < attr.UnequipLots.Length; i++)
            {
                if (attr.UnequipLots[i])
                    gearNames[i] = itm;
            }

            if (Application.isPlaying && inv != null)inv.DepleteItem(itm);
         

        }
        public void Unequip(int slot, Inventory inv)
        {
            NewItem lastItem = items[slot];
            GearSlotAttribute attrLast = lastItem.GetAttribute<GearSlotAttribute>();
            for (int q = 0; q < attrLast.UnequipLots.Length; q++)
            {
                if (attrLast.UnequipLots[q]) gearNames[q] = null;
                
            }

            if (Application.isPlaying && inv != null)inv.AddItem(lastItem);
            

            gear[slot] = null;
        }
        public Gear Clone()
        {
            Gear newGear = ObjectCopier.Clone<Gear>(this);

            return newGear;
        }

    }
}
