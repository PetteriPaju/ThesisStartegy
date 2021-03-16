using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace SengokuWarrior
{

    [System.Serializable]
    public class Mission:Loadable
    {
        
   
        public string Mission_Name= "00. New Mission";
        public string MissionIntroText = "";
        public static Mission currentMission;
      
        public MissionCondition WinDondition;
        public MissionCondition LoseCondition;

        private bool saveLoaded = false;

        public List<Team> Teams = new List<Team>();
        public List<MissionEventContainer> events = new List<MissionEventContainer>();

        public UnityEngine.Events.UnityEvent DoAtLoad = new UnityEngine.Events.UnityEvent();
        private UnityEngine.Events.UnityEvent DoAtLoadEnded = new UnityEngine.Events.UnityEvent();

        [HideInInspector] public List<Object> attatchedObjects = new List<Object>();

        [System.NonSerialized]
        private Dictionary<int,Character> characterDictionary= new Dictionary<int,Character>();


        public override Loadable Clone()
        {

            return Instantiate<Mission>(this);
        }
        public int GetTeamIndex(Team team)
        {
            return Teams.IndexOf(team);
        }

        public List<Character> GetAllCharacters()
        {
            List<Character> allcharas = new List<Character>();
            foreach (Team tm in Teams)
            {
                foreach (Character chara in tm.members)
                {
                    characterDictionary.Add(chara.id, chara);
                }
            }
            return allcharas;
        }
        public Character FindCharacter(int id)
        {
           
            if (characterDictionary.Count == 0)
            {
                RefreshCharacterDictionary();
            }

            if (characterDictionary.ContainsKey(id))
            {
                return characterDictionary[id];
            }
            return null;
        }

        public void RefreshCharacterDictionary()
        {
            characterDictionary.Clear();
            foreach (Team tm in Teams)
            {
                foreach (Character chara in tm.members)
                {
                    characterDictionary.Add(chara.id, chara);
                }
            }
        }

        public override void LoadData(Save data)
        {
            for (int i = 0; i < this.events.Count; i++)
            {
                this.events[i].DoneOnce = data.eventsDone[i];
            }

            for (int i = 0; i < this.Teams.Count; i++)
            {
                
                for (int q = 0; q < this.Teams[i].members.Count; q++)
                {
                   Save.SavedCharacterGamePlay savedChara = data.teams[i].characters.Find(item => item.id == this.Teams[i].members[q].id);
                    if (savedChara != null)
                    {
                        this.Teams[i].members[q].LoadSaveData(savedChara);
                        this.Teams[i].members[q].stats.CalculateStats();
                    }
                }

            }
            TurnManager.TurnNumber = data.MissionTurn;
            TurnManager.currentTeam = data.CurrentTeamIndex;
        }

        public override void LoadDefaultData()
        {
            for (int i = 0; i < this.Teams.Count; i++)
            {
                for (int q = 0; q < this.Teams[i].members.Count; q++)
                {
                    if (this.Teams[i].members[q].isunique)
                    {
                        Save.SavedCharacter chara = GameManager.saveData.characterStorage.SavedCharacters.Find(item => item.id == this.Teams[i].members[q].id);
                        if (chara != null)
                        {
                            this.Teams[i].members[q].LoadSaveData(chara);
                            this.Teams[i].members[q].stats.CalculateStats();
                            this.Teams[i].members[q].stats.ResetHPandMP();
                        }
                        else
                        {
                            Character chara2 = GameManager._intance.CharacterDatabase.uniqueCharacters.Find(item => item.id == this.Teams[i].members[q].id);
                            this.Teams[i].members[q] = chara2.Clone(true);
                            this.Teams[i].members[q].stats.CalculateStats();
                            this.Teams[i].members[q].stats.ResetHPandMP();
                        }
                    }
                    else
                    {
                        this.Teams[i].members[q].stats.CalculateStats();
                        this.Teams[i].members[q].stats.ResetHPandMP();
                    }           
                }

            }
        }

        public override void Init()
        {
 
            currentMission = this;
            foreach (MissionEventContainer evt in events)
            {
               evt.CombineAll();
               evt.DoneOnce = false;
            }

            foreach (Team team in Teams)
            {
                team.Init();

            }
            RefreshCharacterDictionary();

            Transform firstTarget = Teams[0].GetAliveMembers()[0].body.transform;
            CameraFollow.target = firstTarget;
            CameraFollow._intance.MoveInstant();

        }


        public override void Begin()
        {
            if (DoAtLoad != null) { DoAtLoad.Invoke(); }
            else
            {        
               DoAtLoadEnded.Invoke();
            }
    
            CameraTransitionHandlerer.fadeEnded -= this.Begin;
            if (this.MissionIntroText != string.Empty)
            UIManager._instance.DisplayText(this.MissionIntroText);
            GameManager._intance.StartCoroutine(BeginAfterIntroText());
        }

        private System.Collections.IEnumerator BeginAfterIntroText()
        {

            yield return new WaitUntil(()=>!UIManager._instance.TextDisplay.gameObject.activeSelf);
        
            GameManager._intance.turnManager.Initialize();
        }


        public override void SaveData()
        {
            Debug.Log("Mission Saved");
            foreach (Team team in Teams) { 
                foreach (Character chara in team.members)
                {
                    if (chara.isunique)
                    {
                        GameManager.saveData.characterStorage.RegisterData(chara);
                    }
                }
            }
            
        }

        public override void End()
        {
            foreach (Team team in Teams)
            {
                team.Destroy();
            }
            if (currentMission == this)
            currentMission = null;
            base.End();

        }
        public List<MissionEventContainer> GetTrueEvents()
        {
            List<MissionEventContainer> containers = new List<MissionEventContainer>();

            for (int i = 0; i < events.Count; i++)
            {
                if (events[i].cond == null) continue;
                if (!events[i].DoneOnce || events[i].RepeatEvent)
                {
                    if (events[i].cond.Check())
                    {
                        containers.Add(events[i]);
                    }
                }
            }

            return containers;
        }


        public override string ToString()
        {
            return this.Mission_Name;
        }
        public Team FindTeam(Character chara)
        {
            foreach(Team team in Teams)
            {
                if (team.members.Contains(chara)) return team;
            }

            return null;
        }
        public static class GenericCopier<T>
        {
            public static T DeepCopy(object objectToCopy)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, objectToCopy);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return (T)binaryFormatter.Deserialize(memoryStream);
                }
            }
        }
    }


}
