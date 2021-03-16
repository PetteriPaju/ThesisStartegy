using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using System.Collections.Generic;
using UnityEngine;

namespace SengokuWarrior
{
    [Serializable]
    public class Save
    {
        public CharacterInfoStorage characterStorage = new CharacterInfoStorage();
        public int PlayerMoney = 9000;
        public int LoadableId = -1;
        public int MissionTurn = -1;
        public string date = "Unknown Date";
        public List<bool> eventsDone = new List<bool>();
        public List<SavedTeam> teams = new List<SavedTeam>();
        public int CurrentTeamIndex = 0;

        public Save()
        {
           
        }

        public void SaveAllCharacters()
        {
            foreach (Character chara in GameManager._intance.CharacterDatabase.uniqueCharacters)
            {
                characterStorage.RegisterData(chara);
            }
        }

        public void SaveMission(Mission currentMission)
        {
            LoadableId = Loadable.CurrentlyLoaded.ID;
            MissionTurn = TurnManager.TurnNumber;
            CurrentTeamIndex = TurnManager.currentTeam;
            eventsDone.Clear();
            teams.Clear();
            for (int i =0; i<currentMission.events.Count; i++)
            {
                eventsDone.Add(currentMission.events[i].DoneOnce);
            }
            for (int q=0; q<currentMission.Teams.Count; q++)
            {
                teams.Add(new SavedTeam(currentMission.Teams[q]));        
            }
        }


        [Serializable]
        public class SavedTeam
        {
            public List<SavedCharacterGamePlay> characters = new List<SavedCharacterGamePlay>();
            public SavedTeam(Team org)
            {    
                for (int q = 0; q < org.members.Count;q++)
                {
                    characters.Add(new SavedCharacterGamePlay(org.members[q]));
                }
                
            }
        }

        [Serializable]
        public class SavedCharacterGamePlay
        {
            public bool isUnique = false;
            public bool isSpawned = false;
            public int id = -1;
            public string Name = string.Empty;
            
            public Stats SavedStats = null;
            public float currentHp = 0;
            public float currentMP = 0;
            public Inventory SavedInventory = null;

            public bool[] TurnActionsPreformed = new bool[] {false,false};
            public List<GridPosition> onStartPath = new List<GridPosition>();
            public GridPosition currentPosition = new GridPosition();
            public int SpawnDirection = -1;

            public bool afterMissionSave = false;

            public SavedCharacterGamePlay(Character chara)
            {
                Character newchara = ObjectCopier.Clone<Character>(chara);
                this.Name = chara._Name;
                this.isUnique = chara.isunique;          
                this.id = chara.id;
               
                this.SavedStats = chara.stats;
                this.currentHp = this.SavedStats.CurrentHP;
                this.currentMP = this.SavedStats.CurrentMP;

                this.SavedInventory = chara.inventroy;

                this.isSpawned = chara.isSpawned;
                this.SpawnDirection = chara.Faceddirection-1;
                this.TurnActionsPreformed = chara.TurnActionsPreformed;
                this.onStartPath = chara.onStartPath;
                this.currentPosition = chara.getGridPosition().Clone();


            }

        }

        [Serializable]
        public class SavedCharacter
        {
            public bool isUnique = false;
            public int id = -1;
            public string Name = string.Empty;

            public Stats SavedStats = null;
            public Inventory SavedInventory = null;

            public SavedCharacter(Character chara)
            {
                Character tempChara = ObjectCopier.Clone<Character>(chara);
                this.Name = chara._Name;
                this.isUnique = tempChara.isunique;
                this.id = tempChara.id;
                this.SavedStats = tempChara.stats;
                this.SavedInventory = tempChara.inventroy;

            }

        }
    }

    public static class ObjectCopier
    {
        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }



            return JsonUtility.FromJson<T>(JsonUtility.ToJson(source));
        }
    }

}
