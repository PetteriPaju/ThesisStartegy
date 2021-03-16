using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SengokuWarrior
{
    [CreateAssetMenu(fileName = "DummyAI", menuName = "AI/CreateDummytAI", order = 1)]
    public class DummyAI : AIBehaviour
    {

        public override void Think(AIHandlerer handlerer, GameGrid currentGrid)
        {
            GameManager._intance.turnManager.EndTurnWait();
        }

    }
}
