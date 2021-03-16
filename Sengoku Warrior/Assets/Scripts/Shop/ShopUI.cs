using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SengokuWarrior {

    public class ShopUI : MonoBehaviour {

        public AutoWriteText TextBox;
        public UnityEngine.UI.Text header;
        public ShopItemSlot itemSlotPrefab;
        public UnityEngine.UI.GridLayoutGroup itemContainer;
        public Shop CurrentShopContent;

        public static ShopUI current;

        public PopUpMenu menu;
        public ItemInfo infoDisplayer;
        public Shop_CharacterSelector characterSelector;
        public UnityEngine.UI.Text moneyCounter;

        public bool SellMode = false;

        public void Init(Shop content)
        {
            header.text = "Buy";
               CurrentShopContent = content;
            if (CurrentShopContent.original == null)
            {
                Shop copy = CurrentShopContent.Clone() as Shop;
                copy.original = CurrentShopContent;
                CurrentShopContent = copy;
            }
            foreach (Transform t in itemContainer.transform)
            {
                GameObject.Destroy(t.gameObject);
            }
            characterSelector.Initialize(CurrentShopContent.getCharacters(), () => Refresh());

            for (int i =0; i< CurrentShopContent.Items.Count; i++)
            {
                ShopItemSlot slot = GameObject.Instantiate<ShopItemSlot>(itemSlotPrefab);
                slot.SetData(CurrentShopContent.Items[i], CurrentShopContent.valueOverrides[i],SellMode);
                slot.transform.SetParent(itemContainer.transform, false);
                slot.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }
            moneyCounter.text = GameManager.saveData.PlayerMoney.ToString();
           

        }

        public void ToggleMode()
        {
            this.SellMode = !this.SellMode;

            header.text = this.SellMode ? "Sell" : "Buy";

            TextBox.Type(this.SellMode ? "I buy anything" : "Have a look at my wares");


            Refresh();
        }

        public void BuyItem(ItemStack i)
        {
            characterSelector.SelectedCharacter.inventroy.AddItem(i.item);
            GameManager.saveData.PlayerMoney -= (int)(CurrentShopContent.GetPrice(i));
            moneyCounter.text = GameManager.saveData.PlayerMoney.ToString();
            CurrentShopContent.BuyItem(i);
        }

        public void SellItem(ItemStack i)
        {
            characterSelector.SelectedCharacter.inventroy.DepleteItem(i.item);
            GameManager.saveData.PlayerMoney += (int)(i.item.Value);
            moneyCounter.text = GameManager.saveData.PlayerMoney.ToString();
            CurrentShopContent.AddItem(i.item);
            Refresh();

        }

        public void Refresh()
        {
            current = this;
            foreach (Transform t in itemContainer.transform)
            {
                GameObject.Destroy(t.gameObject);
            }


            if (!SellMode)
            {

                for (int i = 0; i < CurrentShopContent.Items.Count; i++)
                {
                    ShopItemSlot slot = GameObject.Instantiate<ShopItemSlot>(itemSlotPrefab);
                    slot.SetData(CurrentShopContent.Items[i], CurrentShopContent.valueOverrides[i],SellMode);
                    slot.transform.SetParent(itemContainer.transform, false);
                    slot.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                }
            }
            else
            {
                for (int i = 0; i < characterSelector.SelectedCharacter.inventroy.items.Count; i++)
                {
                    ShopItemSlot slot = GameObject.Instantiate<ShopItemSlot>(itemSlotPrefab);
                    slot.SetData(characterSelector.SelectedCharacter.inventroy.items[i], characterSelector.SelectedCharacter.inventroy.items[i].item.Value, SellMode);
                    slot.transform.SetParent(itemContainer.transform, false);
                    slot.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                }
            }
            moneyCounter.text = GameManager.saveData.PlayerMoney.ToString();
        }

        public void End()
        {
            GameManager.LoadScene(CurrentShopContent.NextLoadable);
        }

        public void Awake()
        {
            current = this;
            menu.Hide();
            TextBox.Type("Welcome to MY shop..");

        }
   

    }
}
