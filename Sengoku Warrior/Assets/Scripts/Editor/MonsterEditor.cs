using UnityEditor;
using UnityEngine;
//What type of Object is this Editor inpecting?
[CustomEditor(typeof(Monster))]
public class MonsterEditor : Editor
{
    //Override defaul Inspector function
    public override void OnInspectorGUI()
    {
       
        //Implement custom controls
        EditorGUILayout.LabelField("This is custom Inspector");
        GUILayout.Button("Here is a Button");
        GUILayout.Space(5);
        EditorGUILayout.LabelField("This is Default Inspector");

        //Optionally display default Inspector
        DrawDefaultInspector();
    }

}

