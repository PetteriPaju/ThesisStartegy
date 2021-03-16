using UnityEditor;
using UnityEngine;

namespace SengokuWarrior
{
    class PreferenceWindow : MonoBehaviour
    {

        // Have we loaded the prefs yet
        private static bool prefsLoaded = false;

        // The Preferences
        private static Color gridColor = new Color(1, 1, 1, 0.75f);
        private static Color capColor = new Color(1, 1, 1, 0.33f);
        private static bool showCrosshair = false;

        public static Color GridColor
        {
            get
            {
                if (!prefsLoaded) LoadSettings();
                return gridColor;
            }

            set
            {
                gridColor = value;
                EditorPrefs.SetString("GridColor", JsonUtility.ToJson(gridColor).ToString());
            }
        }
        public static bool ShowCrosshair
        {
            get
            {
                if (!prefsLoaded) LoadSettings();
                return showCrosshair;
            }

            set
            {
                showCrosshair = value;
                EditorPrefs.SetString("ShowCrosshair", JsonUtility.ToJson(showCrosshair).ToString());
            }
        }
        public static Color CapColor
        {
            get
            {
                if (!prefsLoaded) LoadSettings();
                return capColor;
            }

            set
            {
                gridColor = value;
                EditorPrefs.SetString("CapColor", JsonUtility.ToJson(gridColor).ToString());
            }
        }




        // Add preferences section named "My Preferences" to the Preferences Window
        [PreferenceItem("IMGUI Settings")]
        public static void PreferencesGUI()
        {
            if (!prefsLoaded)
            {

                LoadSettings();

            }

            EditorGUILayout.LabelField("Grid Editor", EditorStyles.boldLabel);
            // Preferences GUI
            gridColor = EditorGUILayout.ColorField("Grid Color", gridColor);
            capColor = EditorGUILayout.ColorField("Cap Color", capColor);
            showCrosshair = EditorGUILayout.Toggle("Display Croshair",showCrosshair);
            EditorGUILayout.Space();
            // Save the preferences
            EditorGUILayout.LabelField("Font", EditorStyles.boldLabel);
            // Preferences GUI
            EditorGUILayout.IntField("Font Size", 12);
            EditorGUILayout.IntField("Line Height", 8);
            EditorGUILayout.Space();
            if (GUI.changed)
            {
                EditorPrefs.SetString("GridColor", JsonUtility.ToJson(gridColor).ToString());
                EditorPrefs.SetString("CapColor", JsonUtility.ToJson(capColor).ToString());
                EditorPrefs.SetString("ShowCrosshair", JsonUtility.ToJson(showCrosshair).ToString());


            }
        }


        public static void LoadSettings()
        {
            gridColor = JsonUtility.FromJson<Color>(EditorPrefs.GetString("GridColor", JsonUtility.ToJson(gridColor).ToString()));
            capColor = JsonUtility.FromJson<Color>(EditorPrefs.GetString("CapColor", JsonUtility.ToJson(capColor).ToString()));
            prefsLoaded = true;
        }
    }
}
