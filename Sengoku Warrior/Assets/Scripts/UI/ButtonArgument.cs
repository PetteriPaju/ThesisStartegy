using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
namespace SengokuWarrior
{
    
    public class ButtonArgument
    {
        public List<UnityAction> onClickCallback;
        public string Title = "Button";
        public bool Interactable = true;

        public ButtonArgument(List<UnityAction> callbacks, string title)
        {
            this.onClickCallback = callbacks;
            this.Title = title;
        }
        public ButtonArgument(string title)
        {
            this.onClickCallback = new List<UnityAction>();
            this.Title = title;
        }



    }
}
