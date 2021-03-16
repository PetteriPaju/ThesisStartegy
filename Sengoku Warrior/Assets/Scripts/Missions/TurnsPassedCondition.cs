using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SengokuWarrior
{
    public class TurnsPassedCondition : MissionCondition
    {

        public int Turn = 1;

        public override bool Check()
        {
            return Turn <= TurnManager.TurnNumber;
        }

    }
}
