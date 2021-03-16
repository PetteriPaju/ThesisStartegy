using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace SengokuWarrior
{
    [CreateAssetMenu(menuName = "Tactics/Test", fileName = "PointReached")]
    public class PointReachedCondition : MissionCondition
    {
        public List<GridPosition> Positions = new List<GridPosition>();
        public int Team = 0;

        public override bool Check()
        {
            if (Positions.Count == 0) return false;
            if (Mission.currentMission.Teams.Count>=Team && Team >= 0)
            {
                List<Character> characters = Mission.currentMission.Teams[Team].GetAliveMembers();
                for (int i=0; i< characters.Count; i++)
                {
                    if (Positions.Find(item => item.Equals(characters[i].Position)) != null)
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

    }
}
