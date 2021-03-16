using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SengokuWarrior
{
    [System.Serializable]
    public class MissionEventContainer : ISerializationCallbackReceiver
    {

        #if UNITY_EDITOR
        public Dictionary<MissionEvent, bool> EditorFolds = new Dictionary<MissionEvent, bool>();
        public string editorName = "new Event";
        public string editorDescr;
        public bool MainFold = false;
        public bool conditionFold = false;
        #endif


        [System.NonSerialized]
        public List<MissionEvent> allEvents = new List<MissionEvent>();

        [SerializeField]
        private List<ME_KillCharacter> killCharacterList = new List<ME_KillCharacter>();
        [SerializeField]
        private List<ME_GiveItem> giveItemsList = new List<ME_GiveItem>();
        [SerializeField]
        private List<ME_Visu16> visu16Events = new List<ME_Visu16>();
        public MissionCondition cond;

        public bool RepeatEvent = false;
        [System.NonSerialized]
        public bool DoneOnce = false;
        [System.NonSerialized]
        public bool DoneCurrent = false;


        public void OnBeforeSerialize()
        {
            Save();
        }

        public void OnAfterDeserialize()
        {
            allEvents = CombineAll();
        }


        public System.Collections.IEnumerator DoAll()
        {
            for (int i =0; i<allEvents.Count; i++)
            {
                allEvents[i].Execute();
                yield return new WaitUntil(() => allEvents[i].isDone());
                allEvents[i].DoAtEnd();
            }
            DoneOnce = true;
            DoneCurrent = true;
        }


        public List<MissionEvent> CombineAll()
        {
            List<MissionEvent> newList = new List<MissionEvent>();

            ListToAll(killCharacterList, ref newList);
            ListToAll(giveItemsList, ref newList);
            ListToAll(visu16Events, ref newList);
            newList.Sort((a, b) => (a.listIndex.CompareTo(b.listIndex)));

            return newList;
        }

        private void ListToAll<T>(List<T> sourceList, ref List<MissionEvent> allEvents) where T : MissionEvent
        {

            foreach (MissionEvent evt in sourceList)
            {
                allEvents.Add(evt);
            } 

        }

        public void Save()
        {
            killCharacterList.Clear();
            giveItemsList.Clear();
            visu16Events.Clear();
            for (int i = 0; i<allEvents.Count; i++)
            {
                allEvents[i].listIndex = i;
                if (allEvents[i].GetType() == typeof(ME_KillCharacter))                               
                    killCharacterList.Add(allEvents[i] as ME_KillCharacter);
                
                else if (allEvents[i].GetType() == typeof(ME_GiveItem))              
                    giveItemsList.Add(allEvents[i] as ME_GiveItem);
                else if (allEvents[i].GetType() == typeof(ME_Visu16))
                    visu16Events.Add(allEvents[i] as ME_Visu16);

            }
        }

    }
}
