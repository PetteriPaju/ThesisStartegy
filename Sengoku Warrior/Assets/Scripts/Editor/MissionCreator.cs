using UnityEditor;
using UnityEngine;

namespace SengokuWarrior
{
    public class MissionCreator : EditorWindow
    {
        private string Mission_Name = "00.NewMission";
        private SceneAsset MapScene;
        private string scenePath = string.Empty;

        public static UnityEngine.Events.UnityAction onMissionCreated;
        public static Mission LastCreatedMission;

        [MenuItem("Game Tools/Create/Create Mission")]
        static void CreateWizard()
        {
            EditorWindow.GetWindow<MissionCreator>();
        }

        void OnDestroy()
        {
            onMissionCreated = null;
        }

        void Awake()
        {
            onMissionCreated = null;
        }
        void OnLostFocus()
        {
     
        }
        void OnGUI()
        {
            Mission_Name = EditorGUILayout.TextField(Mission_Name);
            EditorGUI.BeginChangeCheck();
            MapScene = EditorGUILayout.ObjectField("Scene", MapScene, typeof(SceneAsset), false) as SceneAsset;

            if (EditorGUI.EndChangeCheck())
            {
                scenePath = AssetDatabase.GetAssetPath(MapScene);
                Debug.Log(scenePath);
            }
            EditorGUI.BeginDisabledGroup(scenePath == string.Empty);
            if (GUILayout.Button("Create"))
            {
                var path = EditorUtility.SaveFilePanel(
           "Save the Mission",
           "",
           Mission_Name + ".asset",
           "asset");

                if (path.Length != 0)
                {

                    Mission newMission = ScriptableObject.CreateInstance<Mission>();
                    newMission.scenePath = scenePath;
                    newMission.Mission_Name = Mission_Name;
                    newMission.ID = new UID();

                    AssetDatabase.CreateAsset(newMission, "Assets/Missions/" + Mission_Name + ".asset");
                    EditorTools.LoadableDatabase.loadables.Add(newMission);
                    EditorUtility.SetDirty(EditorTools.LoadableDatabase);
                    AssetDatabase.SaveAssets();
                    LastCreatedMission = newMission;
            

                    if (onMissionCreated != null) onMissionCreated.Invoke();
                    EditorUtility.FocusProjectWindow();
    
                    this.Close();

                }

            }
            EditorGUI.EndDisabledGroup();
        }
      
    }
}
