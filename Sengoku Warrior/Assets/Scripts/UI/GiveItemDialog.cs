using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SengokuWarrior
{
    public class GiveItemDialog : UI_element
    {

        public Image Sprite;
        public Text MainText;
        public Text Multiplier;

        private Animator anim;

        void Awake()
        {
            anim = GetComponent<Animator>();
        }

        void OnEnabled()
        {

        }

        public void SetData(string CharacterName,string ItemName,int amount, Sprite sprite)
        {
            string bodyText = CharacterName;
            bodyText += System.Environment.NewLine;
            bodyText += "obtained";
            bodyText += System.Environment.NewLine;
            bodyText += ItemName;

            MainText.text = bodyText;

            Multiplier.text = "x" + amount.ToString();

            Sprite.sprite = sprite;

        }

        

        // Update is called once per frame
        public override void Update()
        {

            if (Input.GetButtonUp("Submit") || Input.GetMouseButtonUp(0))
            {
                this.Hide();
            }


        }
    }
}
