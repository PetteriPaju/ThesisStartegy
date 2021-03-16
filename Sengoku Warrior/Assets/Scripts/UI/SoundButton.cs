using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SengokuWarrior { 
public class SoundButton : Button, ISelectHandler, ISubmitHandler, IPointerClickHandler {


    public AudioClip PlayOnSelect;
    public AudioClip PlayOnClick;


        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            AudioManager._instance.Play(PlayOnClick);
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            AudioManager._instance.Play(PlayOnClick);
        }
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            AudioManager._instance.Play(PlayOnSelect);
           
        }


    }
   
}