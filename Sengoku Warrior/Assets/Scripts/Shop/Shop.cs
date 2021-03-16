using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SengokuWarrior
{
    [CreateAssetMenu(menuName = "Tactics/Shop", fileName = "Shop")]
    public class Shop : Loadable
    {
        public List<ItemStack> Items = new List<ItemStack>();

        public float ItemSalePriceModifier = 0.5f;
        public float ItemBuyPriceModifier = 0.5f;
        private List<Character> playerCharacters = new List<Character>();

        [HideInInspector]
        public List<float> valueOverrides = new List<float> {};

        public void BuyItem(int i)
        {
           
            Items[i].amount--;
            if (Items[i].amount <= 0)
            {
                Items.RemoveAt(i);
                valueOverrides.RemoveAt(i);
            }
        }

        public float GetPrice(ItemStack i)
        {
            int index = Items.IndexOf(i);

            if (index != -1)
            {
                return valueOverrides[index];
            }

            return i.item.Value;
        }

        public void BuyItem(ItemStack i)
        {
            i.amount--;
            if (i.amount <= 0)
            {
                int q = Items.IndexOf(i);
                Items.Remove(i);
                valueOverrides.RemoveAt(q);
            }
        }
        public void AddItem(NewItem i)
        {
            int index = Items.FindIndex(a => a.item == i);
            if (index == -1)
            {
                Items.Add(new ItemStack(i));
                valueOverrides.Add(i.Value);
            }

            else
            {
                Items[index].amount++;
            }
        }

        public List<Character> getCharacters()
        {
            return playerCharacters;
        }

        public override void Init()
        {
            ShopUI.current.Init(this);
        }
 
        public override void Begin()
        {
           
        }

        public override void End()
        {
            SaveData();
        }

        public override Loadable Clone()
        {

            Shop clone = Instantiate<Shop>(this);
            clone.Items = new List<ItemStack>();
            clone.valueOverrides = new List<float>();
            clone.Items = Items.Select(item => (ItemStack)item.Duplicate()).ToList();
            clone.valueOverrides = valueOverrides.Select(item => item).ToList();

            return clone;

        }

        public override void LoadData(Save data)
        {
         //   Debug.Log("Shop Load Data");
           for (int i = 0; i < data.characterStorage.SavedCharacters.Count; i++)
            {
                Save.SavedCharacter charaData = data.characterStorage.SavedCharacters[i];
                Character chara = GameManager._intance.CharacterDatabase.GetCharacterWithID(charaData.id);
                chara.LoadSaveData(charaData);
                playerCharacters.Add(chara);
            }
        }

        public override void LoadDefaultData()
        {
          //  Debug.Log("Shop Load Default");
            for (int i = 0; i < GameManager._intance.CharacterDatabase.uniqueCharacters.Count; i++)
            {
                Save.SavedCharacter chara = GameManager.saveData.characterStorage.SavedCharacters.Find(item => item.id == GameManager._intance.CharacterDatabase.uniqueCharacters[i].id);
                Character newChara;
                if (chara != null)
                {
                 //   Debug.Log("Shop Character Load from Save");
                    newChara = GameManager._intance.CharacterDatabase.uniqueCharacters.Find(item => item.id == chara.id).Clone(true);
                    newChara.LoadSaveData(chara);
                //    Debug.Log(newChara.inventroy.items.Count);
                    newChara.stats.CalculateStats();
                    newChara.stats.ResetHPandMP();
                }
                else
                {
              //      Debug.Log("Shop Character Load from Default");
                    newChara = GameManager._intance.CharacterDatabase.uniqueCharacters.Find(item => item.id == chara.id).Clone(true);
                    newChara.stats.CalculateStats();
                    newChara.stats.ResetHPandMP();
                }
                playerCharacters.Add(newChara);
            }
        }

        public override void SaveData()
        {
            foreach (Character chara in playerCharacters)
            {
                if (chara.isunique)
                {
                    GameManager.saveData.characterStorage.RegisterData(chara);
                }
            }
        }

    }
}
