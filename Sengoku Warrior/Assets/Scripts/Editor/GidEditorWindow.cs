using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SengokuWarrior
{
    public class GidEditorWindow : EditorWindow
    {
        private static EditorWindow thisWindow;
        private static Editor testEditor;
        [MenuItem("Visu16/Frame Editor")]
        public static void Do()
        {
            thisWindow = GetWindow<GidEditorWindow>();
            GameGrid grid = FindObjectOfType<GameGrid>();
            testEditor = Editor.CreateEditor(grid);
        }

        void OnGUI()
        {
            if (testEditor != null)
            {
                testEditor.OnInspectorGUI();
            }

        }

    }
}
