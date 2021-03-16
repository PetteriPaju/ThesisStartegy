using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior
{
    [System.Serializable]
    public class TeamsDeadCondition : MissionCondition
    {

        public List<bool> DeadTeams = new List<bool>();


        public override bool Check()
        {

            for (int i =0; i < DeadTeams.Count; i++)
            {
                if (i > Mission.currentMission.Teams.Count - 1)
                {
                    Debug.LogError("There is no Team at index " + DeadTeams[i] + " !");
                    return false;
                }
                if (Mission.currentMission.Teams[i].hasAliveMember() == DeadTeams[i] && DeadTeams[i] == true) return false;
               
            }


            return true;
        }

    }
}
