using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior
{
    [System.Serializable]
    public class UnitsAliveCondition : MissionCondition
    {

        public List<TeamUnitWrap> elements = new List<TeamUnitWrap>();
        public override bool Check()
        {
            for (int i = 0; i <elements.Count; i++)
            {
                if (elements[i].TeamIndex < Mission.currentMission.Teams.Count)
                {
                    if (Mission.currentMission.Teams[elements[i].TeamIndex].Members.Count - Mission.currentMission.Teams[elements[i].TeamIndex].MembersAlive > elements[i].amount)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        [System.Serializable]
        public class TeamUnitWrap
        {
            public int amount = 0;
            public int TeamIndex = 0;
        }
    }
}
