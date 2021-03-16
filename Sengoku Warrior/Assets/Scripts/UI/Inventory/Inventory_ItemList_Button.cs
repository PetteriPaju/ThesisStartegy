using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SengokuWarrior
{
    public class Inventory_ItemList_Button : Button, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
    {
      
        public Text text;
        public Text Amounttext;
        public Image InventorySprite;


        private ItemStack stack;

        private UnityEvent onPointerEvent = new UnityEvent();


        public void SetData(ItemStack stack, ButtonArgument arg = null)
        {
            this.stack = stack;
            onPointerEvent.RemoveAllListeners();
            CommonAttributes atr = stack.item.GetAttribute<CommonAttributes>();
            if (atr)
            {
                this.InventorySprite.sprite = atr.GetSprite();
                this.Amounttext.text = this.stack.amount.ToString();
                if (this.Amounttext.text == "1") this.Amounttext.enabled = false;
                else this.Amounttext.enabled = true;
            }


            if (arg != null)
            {
                foreach (UnityAction argument in arg.onClickCallback)
                {
                    onPointerEvent.AddListener(argument);
                }
            }
                
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

           if (onPointerEvent != null)
            {
                onPointerEvent.Invoke();
                InventoryUI._instance.DropDownMenu.Show(eventData.position);
            }


        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            InventoryUI._instance.itemInfo.BuildUI(stack.item);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            InventoryUI._instance.itemInfo.BuildUI(stack.item);
        }

        public bool Refresh()
        {
            CommonAttributes atr = this.stack.item.GetAttribute<CommonAttributes>();
            if (atr)
            {
                this.InventorySprite.sprite = atr.GetSprite();
                this.Amounttext.text = this.stack.amount.ToString();
            }

            return stack.amount > 0;
        }
    }



#if UNITY_EDITOR
    [CustomEditor(typeof(Inventory_ItemList_Button))]
    public class Inventory_ItemList_ButtonEditor: Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif

}
