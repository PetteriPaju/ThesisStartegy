using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SengokuWarrior
{
    public class PlayerBody : CharacterBody
    {
        public Player playerData = new Player();

        void Start()
        {
            GetData().OnStart(this);
        }

        public override Character GetData()
        {
            return this.playerData;
        }

    }
}
