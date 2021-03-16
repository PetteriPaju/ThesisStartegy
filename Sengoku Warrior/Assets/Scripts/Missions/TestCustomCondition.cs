using System;
using UnityEditor;
using UnityEngine;

namespace SengokuWarrior
{
    [CreateAssetMenu(menuName = "Tactics/Custom Condition", fileName = "Condition")]
    [System.Serializable]
    public class TestCustomCondition : MissionCondition
    {

        public int testInteger=5;

        public override bool Check()
        {
            return true;
        }

    }
}
