using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior {
    public class UnitsDeadCondition : MissionCondition  {

        public List<int> DeadCharacters = new List<int>();
        public override bool Check()
        {

           foreach(int id in DeadCharacters)
            {
                Character chara = Mission.currentMission.FindCharacter(id);
                if (chara == null) 
                {
                    Debug.Log("No Chracter with id " + id + " was found!");
                    return false;
                }
                else
                {
                    if (chara.isAlive) return false;
                }
            }


            return true;
        }
    }
}
