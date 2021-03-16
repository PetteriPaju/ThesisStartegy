using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior
{
    public class CharacterBody : MonoBehaviour
    {



        public Animator animator;
        public SpriteRenderer sRenderer;
        public SpriteRenderer shadowRenderer;
        public AIHandlerer aiHandelerer;
       // [System.NonSerialized]
        public Character charadata;
        [HideInInspector]public HPbar hpBar;

        public GameObject HelathBarAnchor;

        public virtual Character GetData()
        {
            return charadata;
        }

        // Update is called once per frame
        void Update()
        {

            if (hpBar != null)
            {
                hpBar.transform.position = HelathBarAnchor.transform.position;
            }

            if (charadata.OnUpdate != null) GetData().OnUpdate.Invoke();
        }
    }
}
