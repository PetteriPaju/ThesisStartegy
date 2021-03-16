using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

namespace SengokuWarrior
{
    public class SaveLoadWindow : UI_element
    {
        private Save[] allSaves;
        private TextAsset[] saveAssets;

        private Dictionary<Save, FileInfo> saveDictionary = new Dictionary<SengokuWarrior.Save, FileInfo>();

        public Button SaveButtonPrefab;

        public Button NewSaveButton;
        public GameObject SaveButtonContainer;

        public Button SaveButton;
        public Button LoadButton;
        public Button DeleteButton;


        public override void Init()
        {
            base.Init();
            RefreshSaves();
            GenerateUI();
            SetButtonsDisabled(false, false, false);
        }

        public void SetButtonsDisabled(bool save, bool load, bool delete)
        {
            SaveButton.interactable = save;
            LoadButton.interactable = load;
            DeleteButton.interactable = delete;
        }
        public void Save(Save data)
        {
            Delete(data);
            data.LoadableId = Loadable.CurrentlyLoaded.ID;
            data.date = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

            if (Mission.currentMission.ID == data.LoadableId)
            {
                data.SaveMission(Mission.currentMission);
            }


            using (StreamWriter writer =
              new StreamWriter("SaveData/" + data.date + ".sv"))
            {
                writer.Write(UnityEngine.JsonUtility.ToJson(data));
            }

            Init();

        }

        private void Load(Save data)
        {
            GameManager._intance.LoadSaveData(data);
        }
        public void SaveCurrentData()
        {
            GameManager.saveData.date = System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            GameManager.saveData.LoadableId = Loadable.CurrentlyLoaded.ID;

            if (Mission.currentMission.ID == GameManager.saveData.LoadableId)
            {
                GameManager.saveData.SaveMission(Mission.currentMission);
                Debug.Log("Save Mission");
            }
              
            using (StreamWriter writer =
                 new StreamWriter("SaveData/" + GameManager.saveData.date + ".sv"))
            {
                writer.Write(UnityEngine.JsonUtility.ToJson(GameManager.saveData));
            }

            
        Init();
        
        }
        private void Delete(Save data)
        {
            FileInfo file = saveDictionary[data];
            file.Delete();
            Init();
        }
        public void SetSaveListener(Save data)
        {
            SaveButton.onClick.RemoveAllListeners();
            SaveButton.onClick.AddListener(() => Save(data));
            SetButtonsDisabled(true, true, true);
        }
        public void SetLoadListener(Save data)
        {
            LoadButton.onClick.RemoveAllListeners();
            LoadButton.onClick.AddListener(() => Load(data));
            SetButtonsDisabled(true, true, true);
        }
        public void SetDeleteListener(Save data)
        {
            DeleteButton.onClick.RemoveAllListeners();
            DeleteButton.onClick.AddListener(() => Delete(data));
            SetButtonsDisabled(true, true, true);
        }

        public void GenerateUI()
        {
            DestroyChildren(SaveButtonContainer.transform);
        
            for (int i= 0; i<allSaves.Length; i++)
            {
                int _i = i;
                Button btn = Instantiate<Button>(SaveButtonPrefab);
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => SetSaveListener(allSaves[_i]));
                btn.onClick.AddListener(() => SetLoadListener(allSaves[_i]));
                btn.onClick.AddListener(() => SetDeleteListener(allSaves[_i]));
                btn.GetComponentInChildren<Text>().text = allSaves[_i].date;

                btn.transform.SetParent(SaveButtonContainer.transform);
                btn.transform.localScale = new Vector3(1, 1, 1);

            }




        }


        public void RefreshSaves()
        {

            List<Save> saves = new List<SengokuWarrior.Save>();
            saveDictionary.Clear();
            if (!Directory.Exists("SaveData"))
            {
                Directory.CreateDirectory("SaveData");
            }
            DirectoryInfo fl = new DirectoryInfo("SaveData");
            FileInfo[] array1 = fl.GetFiles("*.sv").OrderByDescending(f => f.LastWriteTime).ToArray<FileInfo>();

            for (int a = 0; a < array1.Length; a++)
            {       
                string text;
                using (StreamReader sr = array1[a].OpenText())
                {
                    text = sr.ReadToEnd();

                    Save sv = JsonUtility.FromJson<Save>(text);
                    saveDictionary.Add(sv, array1[a]);
                    saves.Add(sv);
                }

            }

            allSaves = saves.ToArray();
        }

        private void DestroyChildren(Transform transform)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
