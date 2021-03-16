using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace SengokuWarrior
{
    public class PopUpMenu: MonoBehaviour
    {
        public static PopUpMenu OpenMenu;
        public Button overlay;
        public LayoutGroup ButtonContainer;
        public Button ButtonPrefab;
        public UnityEngine.Events.UnityEvent OnClode;

        void Start()
        {
            overlay.transform.localScale = new Vector3(9999, 9999, 9999);
        }

        void Update()
        {
            if (Input.GetButtonUp("Cancel"))
            {
                this.Hide();           
            }
        }
        public virtual void Initialize(List<ButtonArgument> buttons, UnityEngine.Events.UnityAction closeCallback)
        {
            if(closeCallback != null)
            this.OnClode.AddListener(closeCallback);
            DestroyChildren();
            CreateButtons(buttons);
        }

        public virtual void CreateButtons(List<ButtonArgument> buttons)
        {
            foreach (ButtonArgument arg in buttons)
            {
                Button btn = Button.Instantiate<Button>(ButtonPrefab);
                btn.transform.SetParent(ButtonContainer.transform);
                btn.transform.localScale = new Vector3(1, 1, 1);
                btn.onClick.RemoveAllListeners();

                foreach(UnityEngine.Events.UnityAction act in arg.onClickCallback)
                {
                    btn.onClick.AddListener(act);
                }
                
                btn.onClick.AddListener(() => this.Hide());
                btn.interactable = arg.Interactable;

                Text txt = btn.GetComponentInChildren<Text>();
                if (txt) txt.text = arg.Title;

            }
        }

        public virtual void Show(Vector2 pos)
        {
            if (OpenMenu != this && OpenMenu != null) OpenMenu.Hide();
            gameObject.SetActive(true);

            pos.x += ButtonContainer.GetComponent<RectTransform>().sizeDelta.x;

            transform.position = pos;

        }

        public virtual void Hide()
        {
            if (OpenMenu == this) OpenMenu.Hide();

            if (this.OnClode != null) this.OnClode.Invoke();
            gameObject.SetActive(false);
            DestroyChildren();

        }
        
        private void DestroyChildren()
        {
            foreach (Transform t in ButtonContainer.transform)
            {
                GameObject.Destroy(t.gameObject);
            }
        }
    }
}
