using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
namespace SengokuWarrior
{   [SerializeField]
    public class MissionEditor : EditorWindow
    {

        private static MissionEditor thisWindow;

        [SerializeField]
        private List<Mission> Missions;
        [SerializeField]
        private string[] MissionList = new string[] {"No Missions"};
        [SerializeField]
        private int selectedMissionIndex;
        [SerializeField]
        private string missionFolderPath = "Assets/Missions";
        public Mission selectedMission;

        [SerializeField]
        private Editor missionEditor;

        [MenuItem("Game Tools/Mission Editor")]
        public static void Do()
        {
            thisWindow = GetWindow<MissionEditor>();
            thisWindow.Refresh();
            thisWindow.titleContent = new GUIContent("Mission Editor");
        }

        public void OnLostFocus()
        {
            ObjectHilighter.Clear();
        }
        public void OnSelectionChange()
        {
            ObjectHilighter.Clear();
            MissionInspector.characterSelected = false;
        }
        public void Refresh()
        {

            Missions = LoadMissions();

            int lastMission = SessionState.GetInt("MissionEditor_LastSelectedMission", 0);
            if (Missions.Count != 0 && Missions.Count-1>=lastMission)
            {
                if (selectedMission != Missions[lastMission] || selectedMission == null)
                {
                    selectedMission = Missions[lastMission];
                    missionEditor = Editor.CreateEditor(selectedMission);
                }
            }

                if (selectedMission == null && Missions.Any()) {
                selectedMission = Missions[0];
                SessionState.SetInt("MissionEditor_LastSelectedMission",0);
                missionEditor = Editor.CreateEditor(selectedMission);
            }
            selectedMissionIndex = GetIndexOfCurrentMission();
        }

        void OnProjectChange()
        {
            Refresh();
        }
        public void OnFocus()
        {       
        }
        public void OnEnable()
        {

            Refresh();

        }
        public void OnDisable()
        {

        }

        public List<Mission> LoadMissions()
        {
            List<Mission> allMissions = new List<Mission>();

            string[] aMaterialFiles = Directory.GetFiles(Application.dataPath+"/Missions", "*.asset", SearchOption.AllDirectories);
            foreach (string matFile in aMaterialFiles)
            {
                string assetPath = "Assets" + matFile.Replace(Application.dataPath, "").Replace('\\', '/');
                Mission sourceMat = (Mission)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Mission));


                if (sourceMat)
                {
                    allMissions.Add(sourceMat);
                }
                // .. do whatever you like
            }

            MissionList = CreateNameArray(allMissions);

            return allMissions;
        }
        public static string[] CreateNameArray(List<Mission> list)
        {
            if (list.Count == 0) return new string[] { "No Missions" };
            List<string> lst = new List<string>();

            foreach (Mission obj in list)
            {
                    lst.Add(obj.Mission_Name); 
            }
            return lst.ToArray();
        }

        public int GetIndexOfCurrentMission()
        {
            
            if (selectedMission == null) return 0;

          for (int i = 0; i<MissionList.Length; i++)
            {
                if (MissionList[i] == selectedMission.Mission_Name) return i;
            }

            return 0;
        }
        void OnGUI()
        {
          
            TopbarGUI();
            GUILayout.Space(5);
            if (missionEditor != null && selectedMission != null)
            {
                missionEditor.OnInspectorGUI();
            }
            
        }

        void RemoveAssets(Mission miss)
        {
            foreach (Object obj in miss.attatchedObjects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                AssetDatabase.DeleteAsset(path);
            }
        }

        public void TopbarGUI()
        {

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Missions",GUILayout.Width(70));
            selectedMissionIndex = EditorGUILayout.Popup(selectedMissionIndex, MissionList, EditorStyles.toolbarPopup);

            if (EditorGUI.EndChangeCheck() && Missions.Any())
            {
                selectedMission = Missions[selectedMissionIndex];
                SessionState.SetInt("MissionEditor_LastSelectedMission", selectedMissionIndex);
                missionEditor = Editor.CreateEditor(selectedMission);
                QuickSwitchScenes((Mission)Missions[selectedMissionIndex]);
            }

            EditorGUI.BeginDisabledGroup(Missions.Count == 0);
            if (GUILayout.Button("-", EditorStyles.toolbarButton))
            {
                if (EditorUtility.DisplayDialog("Delete Mission?",
                 "Are you sure you want to delete Mission: " + Missions[selectedMissionIndex].Mission_Name + "?  This action cannot be undone!"
                 , "Delete", "Cancel"))
                {
                    RemoveAssets(Missions[selectedMissionIndex]);
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath((Mission)Missions[selectedMissionIndex]));
                }
                SessionState.EraseInt("MissionEditor_LastSelectedMission");
            }

            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("+", EditorStyles.toolbarButton))
            {
                MissionCreator.GetWindow<MissionCreator>();
                MissionCreator.onMissionCreated += () => {
                    this.Focus();

                  
                    selectedMission = MissionCreator.LastCreatedMission;
                    missionEditor = Editor.CreateEditor(selectedMission);
                    selectedMissionIndex = GetIndexOfCurrentMission();
                    SessionState.SetInt("MissionEditor_LastSelectedMission", selectedMissionIndex);

                };
            }
            EditorGUILayout.EndHorizontal();
        }

        public static void QuickSwitchScenes(Mission MissionToBeLoaded)
        {
            if (Application.isEditor)
            {
                if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {

                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene(MissionToBeLoaded.scenePath, UnityEditor.SceneManagement.OpenSceneMode.Additive);

                    if (Mission.currentMission != MissionToBeLoaded && Mission.currentMission != null)
                    {

                        UnityEditor.SceneManagement.EditorSceneManager.CloseScene(UnityEditor.SceneManagement.EditorSceneManager.GetSceneByPath(Mission.currentMission.scenePath), true);
                    }
            


                
                    UnityEditor.SceneView.RepaintAll();
                    Mission.currentMission = MissionToBeLoaded;
                    Loadable.CurrentlyLoaded = MissionToBeLoaded;
                }
            }
        }

    }

    public abstract class IEmbedable : EditorWindow
    {

        public virtual bool Render() { return true; }

        void SetParent(MissionInspector parent) {}

    }

    

}