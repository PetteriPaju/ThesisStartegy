using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SengokuWarrior
{
    public class Flicker : MonoBehaviour
    {
        public float time = 0.62f;
        public float alpha = 0.33f;
        // Use this for initialization
        void Start()
        {
            iTween.FadeTo(gameObject, iTween.Hash("alpha", alpha, "time", time, "looptype", "pingpong", "easetype", "easeInQuad"));
        }
    }
}
