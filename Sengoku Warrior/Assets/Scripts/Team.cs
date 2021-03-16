using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
namespace SengokuWarrior
{
    [Serializable]
   public  class Team
    {
        [SerializeField]
        public List<Character> members = new List<Character>();


        public static List<Team> allTeams = new List<Team>();

        public int MaxUnits = 1;
        public string TeamName = "Team 1";

        public Color TeamColor = Color.white;
        public void Init()
        {
            if (allTeams.Contains(this)) Debug.LogWarning("Same team was added twice!");
            else allTeams.Add(this);

            foreach (Character chara in members)
            {
                chara.SetTeam(allTeams.IndexOf(this));
              
                GameManager._intance.CreateCharacterBody(chara, TeamColor);
            }
        }

        public void Destroy()
        {
            allTeams.Remove(this);

            foreach (Character chara in members)
            {
                if (chara.body != null && chara.body.gameObject != null)
                GameObject.Destroy(chara.body.gameObject);
            }


        }


        public void TurnEndedUpdate()
        {

            foreach (Character chara in members)
            {
                if (!chara.isSpawned && chara.SpawnCondition != null)
                {
                    if (chara.SpawnCondition.Check())
                    {
                        chara.Spawn();
                    }
                }
            }

        }

        public List<Character> Members
        {
            get { return members; }
            set { members = value; }
        }

        public void AddToTeam(Character chara) {

            if (Members.Contains(chara)) Debug.LogWarning("Same character was added twice!");
            else Members.Add(chara);

        }

        public List<Character> GetAliveMembers()
        {
            return Members.Where(s => s.isAlive).ToList<Character>();
        }
        public List<Character> GetAliveMembersWithTurnPending()
        {
            return Members.Where(s => s.isAlive &&( !s.TurnActionsPreformed[0] || !s.TurnActionsPreformed[1])).ToList<Character>();
        }
        public bool hasAliveMember() { return MembersAlive != 0; }
        public int MembersAlive { get { return Members.Where(s => s.isAlive).Count(); } }
        public int MembersTotal { get { return members.Count; } }
        
    }
}
