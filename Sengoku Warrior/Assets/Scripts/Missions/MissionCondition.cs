using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SengokuWarrior
{
    [System.Serializable]
    public abstract class MissionCondition : ScriptableObject
    {
        public enum ConditionType
        {
            UnitsDead,
            UnitsDeadAmount,
            TeamsDead,
            PointReached,
            TurnsPassed,
            Custom
        }
        [HideInInspector]
        public ConditionType conditionType = ConditionType.Custom;

        public virtual bool Check()
        {
            return false;
        }

    }
}