using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

namespace SengokuWarrior {
    class ExampleWindow : EditorWindow
    {
        UnityEngine.Object anyObject;
        [MenuItem("Example/Example A")]
        public static void Do()
        {

            EditorWindow win = GetWindow<ExampleWindow>();
            win.titleContent = new GUIContent("Example");
        }

        bool show = false;
        Vector2 scroll = Vector2.zero;

        bool[] pos = new bool[3] { true, true, true };
        bool[] rot = new bool[3] { true, true, true };
        bool[] scale = new bool[3] { true, true, true };

        bool posGroupEnabled = true;
        bool rotGroupEnabled = true;
        bool scaleGroupEnabled = false;

        bool foldout = false;
        bool foldout2 = false;


        GUISkin customSkin;       
        void OnEnable()
        {
            customSkin = EditorGUIUtility.Load("Custom Skin.guiskin") as GUISkin;       
        }

        void OnGUI()
        {
            GUI.skin = customSkin;
            GUILayout.Button("I now use the Custom Skin!");
        }




        void OtherElements()
        {
            EditorGUILayout.BeginHorizontal();
            // Begin Scroll area
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Width(150));

            //Create 50 buttons
            for (int i = 0; i < 50; i++)
                GUILayout.Button("Button " + i, GUILayout.Height(40));

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            show = EditorGUILayout.ToggleLeft("Enable controls", show);
            EditorGUI.BeginDisabledGroup(!show);

            EditorGUILayout.LabelField("Numeric", EditorStyles.boldLabel);
            EditorGUILayout.FloatField("Float Field:", 5.5f);
            EditorGUILayout.Slider("Float Slider:", 8.5f, 0, 15);

            EditorGUILayout.LabelField("Other", EditorStyles.boldLabel);
            EditorGUILayout.Toggle("Boolean Toggle", true);
            EditorGUILayout.Vector3Field("Vector3 Field", new Vector3(2, 15, 10));
            EditorGUILayout.RectField("Rectangle Field", new Rect(5, 10, 6, 10));

            EditorGUILayout.Space();
            EditorGUILayout.ColorField("Color Field", Color.blue);
            AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
            EditorGUILayout.CurveField("Curve Field", curve);

            EditorGUI.EndDisabledGroup();


            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            posGroupEnabled = EditorGUILayout.BeginToggleGroup("Align position", posGroupEnabled);
            pos[0] = EditorGUILayout.Toggle("x", pos[0]);
            pos[1] = EditorGUILayout.Toggle("y", pos[1]);
            pos[2] = EditorGUILayout.Toggle("z", pos[2]);
            EditorGUILayout.EndToggleGroup();

            rotGroupEnabled = EditorGUILayout.BeginToggleGroup("Align rotation", rotGroupEnabled);
            rot[0] = EditorGUILayout.Toggle("x", rot[0]);
            rot[1] = EditorGUILayout.Toggle("y", rot[1]);
            rot[2] = EditorGUILayout.Toggle("z", rot[2]);
            EditorGUILayout.EndToggleGroup();

            scaleGroupEnabled = EditorGUILayout.BeginToggleGroup("Align scale", scaleGroupEnabled);
            scale[0] = EditorGUILayout.Toggle("x", scale[0]);
            scale[1] = EditorGUILayout.Toggle("y", scale[1]);
            scale[2] = EditorGUILayout.Toggle("z", scale[2]);
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foldout = EditorGUILayout.Foldout(foldout, "Show extra options");
            if (foldout)
            {
                EditorGUILayout.TextField("Some Text", "Some Text");
                GUILayout.Button("Reset");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foldout2 = EditorGUILayout.Foldout(foldout2, "Show even more extra options");
            if (foldout2)
            {
                EditorGUILayout.TextField("Some Text", "Some Text");
                GUILayout.Button("Reset");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        void Elements()
        {
            EditorGUILayout.LabelField("Numeric", EditorStyles.boldLabel);
            EditorGUILayout.FloatField("Float Field:", 5.5f);
            EditorGUILayout.Slider("Float Slider:", 8.5f, 0, 15);

            float a = 4.5f, b = 10, c = 0, d = 20;
            EditorGUILayout.MinMaxSlider("Min-Max Slider", ref a, ref b, c, d);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Text Fields", EditorStyles.boldLabel);
            EditorGUILayout.TextField("This is one line textfield");
            EditorGUILayout.TextArea("This is multiline textfield", GUILayout.Height(50));
            EditorGUILayout.PasswordField("Password Field", "Passw0rd");
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Other", EditorStyles.boldLabel);
            EditorGUILayout.Toggle("Boolean Toggle", true);
            EditorGUILayout.Vector3Field("Vector3 Field", new Vector3(2, 15, 10));
            EditorGUILayout.RectField("Rectangle Field", new Rect(5, 10, 6, 10));

            EditorGUILayout.Space();
            EditorGUILayout.ColorField("Color Field", Color.blue);
            AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
            EditorGUILayout.CurveField("Curve Field", curve);
            anyObject = EditorGUILayout.ObjectField("Object Selector", anyObject, typeof(UnityEngine.Object), true);
        }

    }
}
