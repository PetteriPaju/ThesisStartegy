using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SengokuWarrior {
    public class UnitListButton : MonoBehaviour, ISelectHandler
    {

        public Text CharacterText;
        public Text HPText;

        private Character selectCharacter;



        public void SetData(Character chara)
        {
            CharacterText.text = chara._Name;
            HPText.text = chara.stats.CurrentHP +"/"+ chara.stats.CalculatedHP;
            selectCharacter = chara;
        }


        public void OnSelect(BaseEventData eventData)
        {
            if (selectCharacter != null)
            Target.SetTarget(selectCharacter);
        }

    }
}
