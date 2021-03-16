using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SengokuWarrior
{
    [System.Serializable]
    public abstract class MissionEvent
    {
        public int listIndex = -1;

        [System.NonSerialized]
        protected bool Done = false;


        public virtual void Execute()
        {
            Debug.Log("Doing event: " + this.GetType().ToString());
            Done = true;
        }

        public virtual void DoAtEnd()
        {

        }


        public virtual bool isDone()
        {
            return true;
        }
        #if UNITY_EDITOR
        public string editorName = "new Event";
        public string editorDescr;
        public bool showFold = false;
        #endif
    }

    [System.Serializable]
    public class ME_KillCharacter: MissionEvent
    {
        public List<int> charactersToKill = new List<int>();

        public ME_KillCharacter()
        {

        #if UNITY_EDITOR
            editorName = "Kill Characters";
        #endif
        }


        public override bool isDone()
        {
            return Done;
        }


        public override void Execute()
        {
            Debug.Log("Doing event: " + this.GetType().ToString());
            GameManager._intance.StartCoroutine(ZoomToCharacters());
       
        }

        public System.Collections.IEnumerator ZoomToCharacters()
        {

            for (int i = 0; i < charactersToKill.Count; i++)
            {
                if (Mission.currentMission.FindCharacter(charactersToKill[i]).isAlive)
                {
                    CameraFollow.target = Mission.currentMission.FindCharacter(charactersToKill[i]).body.transform;
                    yield return new WaitForSeconds(0.5f);
                    Mission.currentMission.FindCharacter(charactersToKill[i]).RecieveAttack(new AttackMessage(float.NegativeInfinity));
                }
            }
            yield return new WaitForSeconds(1f);
            Done = true;
        }

    }

    [System.Serializable]
    public class ME_GiveItem: MissionEvent
    {

        public int CharacterId =-1;
        public NewItem itemToGive = null;
        public int amount = -1;
        public ME_GiveItem()
        {
            #if UNITY_EDITOR
            editorName = "Kill Give Item";
            #endif
        }


        public override void Execute()
        {
            if (itemToGive == null || CharacterId == -1 || amount <= 0) {
                Done = true;
                return;
            }
            Debug.Log("Doing event: " + this.GetType().ToString());
            GameManager._intance.StartCoroutine(ShowDialog());
        }


        private System.Collections.IEnumerator ShowDialog()
        {
            GiveItemDialog dialog = UIManager._instance.GetItemDialog();

            dialog.SetData(Mission.currentMission.FindCharacter(CharacterId)._Name, itemToGive.ItemName, amount, itemToGive.GetAttribute<CommonAttributes>().GetSprite());
            dialog.gameObject.SetActive(true);
            yield return new WaitUntil(() => dialog.gameObject.activeSelf == false);
            Done = true;

        }

        public override bool isDone()
        {
            return Done;
        }

    }
    [System.Serializable]
    public class ME_Visu16 : MissionEvent
    {
        private bool loaded = false;
        public Visu16.VisuScene scene;
        public ME_Visu16()
        {
#if UNITY_EDITOR
            editorName = "Visu 16 Event";
#endif
        }


        public override void Execute()
        {
            if (scene)
            {
                UIManager._instance.HideAll();
                Visu16.Manager.PlayScene(scene);
            }
            else Done = true;
           
        }


        public override void DoAtEnd()
        {
            base.DoAtEnd();
            Debug.Log("End");
            UIManager._instance.ShowWindow(UIManager.WindowType.GameUI);
        }

        public override bool isDone()
        {
            return !Visu16.Manager.PlayingScene;
        }

    }
}


