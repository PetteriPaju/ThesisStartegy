using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SengokuWarrior {
    public class UnitList : MonoBehaviour {


        private List<UnitListButton> buttons = new List<UnitListButton>();
        public ScrollRect scrollArea;
        public UnitListButton prefab;

        public void SetData(List<Character> charas)
        {

            Clear();

            buttons = new List<UnitListButton>();
            foreach (Character chara in charas) {
                UnitListButton newButton = GameObject.Instantiate<UnitListButton>(prefab);
                newButton.SetData(chara);
                newButton.transform.SetParent(scrollArea.content.transform);
                buttons.Add(newButton);


                newButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                    }


        }

        public void Clear()
        {
            foreach (UnitListButton btn in buttons)
            {
                GameObject.Destroy(btn.gameObject);
            }
            buttons = new List<UnitListButton>();
        }



    }
}