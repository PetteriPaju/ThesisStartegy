using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

namespace SengokuWarrior
{
    public class GridPositionSelectionTool:EditorWindow
    {
        private static UnityAction<GridPosition> callback;
        private static UnityAction OnChangeCallback;
        private static UnityAction OnPreChangeCallback;
        private static GridPositionSelectionTool currentWindow;
        public static void Begin(UnityAction<GridPosition> callback, UnityAction OnChangeCallback = null, UnityAction OnPreChangeCallback = null)
        {
            GridPositionSelectionTool.callback = callback;
            GridPositionSelectionTool.OnChangeCallback = OnChangeCallback;
            GridPositionSelectionTool.OnPreChangeCallback = OnPreChangeCallback;
            currentWindow = GetWindow<GridPositionSelectionTool>();
            currentWindow.ShowUtility();
            
        }


        void OnGUI()
        {

            GameTile selectedTile = null;

            if (Selection.activeGameObject != GridEditor.selectedTile && Selection.activeGameObject != null) selectedTile = Selection.activeGameObject.GetComponent<GameTile>();
            EditorGUI.BeginDisabledGroup(GridEditor.selectedTile == null && selectedTile == null);

        if (GUILayout.Button("Pick"))
            {


                if (callback != null)
                {
                    GridPosition pos;

                    if (selectedTile != null)
                    {
                       pos = EditorTools.CurrentInspectedGrid.FindTile(selectedTile);
                    }
                    else
                    {
                        pos = GridEditor.Selected;
                    }

                    if (OnPreChangeCallback != null) OnPreChangeCallback();
                    callback.Invoke(pos);
                }
                if (OnChangeCallback != null) OnChangeCallback.Invoke();
                this.Close();
            }
            EditorGUI.EndDisabledGroup();
        }

        void OnDestroy()
        {
            callback = null;
            OnChangeCallback = null;
        }

    }
}
