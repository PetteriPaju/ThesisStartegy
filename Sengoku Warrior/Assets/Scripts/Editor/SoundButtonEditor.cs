using UnityEngine;
using UnityEditor;

namespace SengokuWarrior
{

    [CustomEditor(typeof(SoundButton))]
    public class SoundButton_Editor : UnityEditor.UI.ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("PlayOnSelect"));
            EditorGUILayout.PropertyField(base.serializedObject.FindProperty("PlayOnClick"));
            base.serializedObject.ApplyModifiedProperties();

        }
    }

}
