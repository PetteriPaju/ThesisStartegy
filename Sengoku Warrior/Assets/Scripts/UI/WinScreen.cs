﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior
{
    public class WinScreen : UI_element
    {
        private bool buttonClicked = false;
        void OnEnable()
        {
            buttonClicked = false;
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        public override void Update()
        {
            if (buttonClicked) return;
            if (Input.anyKeyDown)
            {
                Mission.currentMission.SaveData();
                GameManager.LoadScene(Mission.currentMission.NextLoadable);
                buttonClicked = true;


            }

        }

    }
}
