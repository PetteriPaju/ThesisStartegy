using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior
{
    public class Shop_CharacterSelector : MonoBehaviour
    {
        public Character SelectedCharacter;
        public UnityEngine.UI.Image[] Images = new UnityEngine.UI.Image[3];
        public UnityEngine.UI.Text NameTag;
        private List<Character> charas;
        private int index = 0;
        public UnityEngine.Events.UnityEvent OnChangeCallbak;
        public void Initialize(List<Character> list, UnityEngine.Events.UnityAction callback)
        {
            OnChangeCallbak.RemoveAllListeners();
            OnChangeCallbak.AddListener(callback);
            index = 0;
            charas = list;
            SetData();

        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                Previous();
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                Next();
            }
        }

        public void Next()
        {
            index++;
            if (index > charas.Count - 1)
            {
                index = 0;
            }
            SetData();
            if (OnChangeCallbak != null) OnChangeCallbak.Invoke();
        }
        public void Previous()
        {
            index--;
            if (index < 0)
            {
                index = charas.Count - 1;
            }
            SetData();
            if (OnChangeCallbak != null) OnChangeCallbak.Invoke();
        }


        public void SetData()
        {
            if (charas.Count > 0)
            {
                SelectedCharacter = charas[index];
                Images[0].sprite = charas[index].UIIcon;
                NameTag.text = charas[index]._Name;
                if (charas.Count > 1)
                {
                    int tmpIndex = index + 1;
                    if (tmpIndex > charas.Count-1)
                    {
                        tmpIndex = 0;
                    }

                    Images[1].sprite = charas[tmpIndex].UIIcon;
             
                    if (charas.Count > 2)
                    {

                        tmpIndex = index - 1;
                        if (tmpIndex< 0)
                        {
                            tmpIndex = charas.Count-1;
                        }


                        Images[2].sprite = charas[tmpIndex].UIIcon;
                    }

                }
               
            }
        }     
    }
}