using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace SengokuWarrior {

    public class ShopItemSlot : Button,IPointerClickHandler, IPointerEnterHandler,IPointerExitHandler, ISelectHandler {
        public static ShopItemSlot current;

        public Text AmountText;
        public Text Price;
        private ItemStack stack;
        private float price = 0;
        private bool SellMode = false;
        [HideInInspector]
        public void SetData(ItemStack stack, float price, bool SellMode)
        {
            this.SellMode = SellMode;
            this.stack = stack;
            this.price = (int)price;

            CommonAttributes atr = stack.item.GetAttribute<CommonAttributes>();
            if (atr)
            {
                this.image.sprite = atr.GetSprite();
                this.AmountText.text = this.stack.amount.ToString();
                this.Price.text = this.price.ToString();
            }
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            List<ButtonArgument> buttons = new List<ButtonArgument>();
            ButtonArgument arg = new ButtonArgument("Buy");
            if (!SellMode)
            {
              
                arg.onClickCallback.Add(() => ShopUI.current.BuyItem(stack));
                ShopUI.current.TextBox.Type("Is this what you want?");
            }
            else
            {
                arg.Title = "Sell";
                arg.onClickCallback.Add(() => ShopUI.current.SellItem(stack));
                ShopUI.current.TextBox.Type("I can buy this from you");
            }

            arg.onClickCallback.Add(() => ShopUI.current.Refresh());

            if (stack.amount == 1) arg.onClickCallback.Add(() => ShopUI.current.infoDisplayer.Clear());

            buttons.Add(arg);
        
            ShopUI.current.menu.Initialize(buttons, () => ShopUI.current.TextBox.Type("Anything else?"));
            ShopUI.current.menu.Show(eventData.pressPosition);

           
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            
            if (current != this)
            {
                current = this;
            }

            ShopUI.current.infoDisplayer.BuildUI(stack.item);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            if (current != this)
            {
                current = this;
            }

            ShopUI.current.infoDisplayer.BuildUI(stack.item);
        }

        public bool Refresh()
        {
            CommonAttributes atr = this.stack.item.GetAttribute<CommonAttributes>();
            if (atr)
            {
                this.image.sprite = atr.GetSprite();
                this.AmountText.text = this.stack.amount.ToString();
                this.Price.text = this.price.ToString();
            }

            return stack.amount > 0;
        }
    



}


#if UNITY_EDITOR
    [CustomEditor(typeof(ShopItemSlot))]
    public class ShopItemSlotEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

        }
    }
#endif
}