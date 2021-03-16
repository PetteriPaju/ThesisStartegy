using UnityEditor;
using UnityEngine;

namespace SengokuWarrior
{
    public class SaveDataMonitor : EditorWindow
    {


        [MenuItem("Game Tools/Data Viewer")]
        public static void Do()
        {
            SaveDataMonitor thisWindow = GetWindow<SaveDataMonitor>();
            thisWindow.titleContent = new GUIContent("Data View");
        }


        void OnGUI()
        {
            if (GameManager.saveData == null) return;
            if (GameManager.saveData.characterStorage.SavedCharacters.Count == 0)
            {
                EditorGUILayout.HelpBox("No Data",MessageType.Info);
            }

            foreach (var item in GameManager.saveData.characterStorage.SavedCharacters)
            {
                EditorGUILayout.LabelField(item.Name);
            }

        }



    }
}
