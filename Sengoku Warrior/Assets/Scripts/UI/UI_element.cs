using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior
{
    public class UI_element : MonoBehaviour
    {
        [System.NonSerialized]
        [HideInInspector]
        public UI_element parent;

        public virtual void Show(UI_element _parent)
        {
            if (_parent)
            {
                this.parent =_parent;
            }
            UIManager.currentWindow = this;
            if (_parent)parent.gameObject.SetActive(false);
            Init();
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);

            UIManager.currentWindow = this.parent; 
            if (this.parent != null)
            {
                parent.gameObject.SetActive(true);
                this.parent = null;
            }

        }


        public virtual void Update()
        {
            if (Input.GetButtonUp("Cancel"))
            {
                this.Hide();
            }
        }
        public virtual void Init()
        {

        }
      


    }
}
