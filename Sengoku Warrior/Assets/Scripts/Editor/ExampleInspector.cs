using UnityEngine;
using UnityEditor;




[CustomEditor(typeof(ExampleScript))]
public class ScaledCurveDrawer : Editor
{


}

/*
[CustomPropertyDrawer(typeof(DefaultFloatAttribute))]
public class ScaledCurveDrawer : PropertyDrawer
{
    bool fold = false;
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        fold = EditorGUI.Foldout(new Rect(pos.x, pos.y, pos.width * 0.75f, 20), fold, new GUIContent("Curve"));
        if (fold)
        {
            // Calculate Position of the PropertyField
            Rect fieldRect = new Rect(pos.x, pos.y+20, pos.width * 0.75f, 20);
            // Draw scale
            EditorGUI.PropertyField(fieldRect, prop, label);
            // Calculate Position of the Button
            Rect buttonRect = new Rect(pos.x + pos.width * 0.75f, pos.y+20, pos.width * 0.25f, 20);
            // Draw the Button
            if (GUI.Button(buttonRect, new GUIContent("Reset")))
                prop.floatValue = ((DefaultFloatAttribute)attribute).defaultValue;
        }
        
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (fold)
        {
            return 40;
        }
        else
        return base.GetPropertyHeight(property, label);
    }
}

    */


