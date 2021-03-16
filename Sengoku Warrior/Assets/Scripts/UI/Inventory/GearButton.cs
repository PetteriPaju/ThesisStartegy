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
    public class GearButton : Button, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
    {

        public Text text;
        public Image InventorySprite;


        private NewItem stack;

        private UnityEvent onPointerEvent = new UnityEvent();

        public void Clear()
        {
            stack = null;
        }

        public void SetData(NewItem stack, ButtonArgument arg = null)
        {
            this.stack = stack;
            onPointerEvent.RemoveAllListeners();
            CommonAttributes atr = stack.GetAttribute<CommonAttributes>();
            if (atr)
            {
                this.image.sprite = atr.GetSprite();
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
            if(stack)
            InventoryUI._instance.itemInfo.BuildUI(stack);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            if (stack)
                InventoryUI._instance.itemInfo.BuildUI(stack);
        }

        public bool Refresh()
        {
            CommonAttributes atr = stack.GetAttribute<CommonAttributes>();
            if (atr)
            {
                this.image.sprite = atr.GetSprite();
            }

            return false;
        }
    }



#if UNITY_EDITOR
    [CustomEditor(typeof(GearButton))]
    public class GearButton_ButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif

}
