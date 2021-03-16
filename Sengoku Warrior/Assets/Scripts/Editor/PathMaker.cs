using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEditorInternal;
using System;
using System.Collections.Generic;

namespace SengokuWarrior
{
    class PathMaker : EditorWindow
    {
        public SerializedObject serializedObject;
        public PathSerializedObject objList;
        public ReorderableList list;

        private int editedIndex = 0;
        bool displaySolvedPath = false;

        private List<GridPosition> previewPath = new List<GridPosition>();

        private  UnityAction<List<GridPosition>> callback;
        private List<GridPosition> modifiedList;
        private static PathMaker currentWindow;

        public static void Begin(UnityAction<List<GridPosition>> callback, List<GridPosition> list)
        {
            currentWindow = GetWindow<PathMaker>();
            currentWindow.callback = callback;
            currentWindow.modifiedList = list;
            currentWindow.Init();
            currentWindow.ShowUtility();
        }

        void Init()
        {
            // Remove delegate listener if it has previously
            // been assigned.
            OnSceneGUIHelper.RegisterSecondary(OnSceneGUI);
            // Add (or re-add) the delegate.
      

            objList = ScriptableObject.CreateInstance<PathSerializedObject>();
            if (modifiedList != null)
            objList.list = EditorTools.GenericCopier<List<GridPosition>>.DeepCopy(modifiedList);
            serializedObject = new UnityEditor.SerializedObject(objList);
            list = CreateList(serializedObject, serializedObject.FindProperty("list"));
            OnPathChange();
        }


        private void OnEnable()
        {
            Init();
        }

        void OnDestroy()
        {

            OnSceneGUIHelper.UnRegisterSecondary(OnSceneGUI);
            callback = null;
            ScriptableObject.DestroyImmediate(objList);
        }

        void OnPathChange()
        {
            previewPath.Clear();

            if (displaySolvedPath)
            {
                if (objList.list.Count < 1) return;

                for (int i = 0; i < objList.list.Count-1; i++)
                {
                    AStar pathfinder = AStar.GetPath(objList.list[i], objList.list[i+1], EditorTools.CurrentInspectedGrid,null);
                    previewPath.AddRange(pathfinder.CellsFromPath());
                }
            }
            else
            {
                for (int i = 0; i < objList.list.Count; i++)
                {
                
                    previewPath.Add(objList.list[i]);
                }
            }


            SceneView.RepaintAll();
        }

        public void OnGUI ()
        {
            if (!Application.isPlaying)
            {
                serializedObject.Update();
                list.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
                EditorGUI.BeginChangeCheck();
                displaySolvedPath = EditorGUILayout.Toggle("Display solved path", displaySolvedPath);
                if (EditorGUI.EndChangeCheck())
                {
                    OnPathChange();
                }

                if (GUILayout.Button("Apply"))
                {
                    if (callback != null)
                    {
                        callback.Invoke(EditorTools.GenericCopier<List<GridPosition>>.DeepCopy(objList.list));
                        this.Close();
                    }
                }
            }
            else
            {
                this.Close();
            }

        }

        ReorderableList CreateList(SerializedObject obj, SerializedProperty prop)
        {


            ReorderableList newList = new ReorderableList(serializedObject,
              prop,
              true, true, true, true);

            newList.drawElementCallback =
    (Rect rect, int index, bool isActive, bool isFocused) => {
        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("z"), GUIContent.none);
        EditorGUI.PropertyField(
            new Rect(rect.x + 60, rect.y, rect.width - 60 - 30-40, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("y"), GUIContent.none);
        EditorGUI.PropertyField(
            new Rect(rect.x + rect.width - 30-30, rect.y, 30, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("x"), GUIContent.none);

        GridPosition pos = objList.list[index];
        if(EditorTools.PickField(ref pos, new Rect(rect.x + rect.width - 30, rect.y, 40, EditorGUIUtility.singleLineHeight)))
        {
            OnPathChange();
        }

        
       /* if (GUI.Button(new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight), "Edit"))
        {
            editedIndex = index;
            GameTile tile = EditorTools.CurrentInspectedGrid.GetTile(new GridPosition(element.FindPropertyRelative("z").intValue, element.FindPropertyRelative("y").intValue, element.FindPropertyRelative("x").intValue));
            if (tile != null)
                Selection.activeGameObject = tile.gameObject;
            GridPositionSelectionTool.Begin(ModifyGridPosition);
        }
        */
    };
            newList.onAddCallback = (ReorderableList l) => {
                //  GridPositionSelectionTool.Begin(CreateGridPosition);
                if (Selection.activeGameObject)
                {
                    if (Selection.activeGameObject.GetComponent<GameTile>())
                    {
                        CreateGridPosition(EditorTools.CurrentInspectedGrid.FindTile(Selection.activeGameObject.GetComponent<GameTile>()));
                    }
                }
            };

            newList.onChangedCallback = (ReorderableList l) => {
                OnPathChange();
              
            };


            return newList;
        }

        public void ModifyGridPosition(GridPosition pos)
        {
            objList.list[editedIndex] = pos;
            OnPathChange();

        }
        public void CreateGridPosition(GridPosition pos)
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            editedIndex = index;
            list.serializedProperty.serializedObject.ApplyModifiedProperties();
            objList.list[editedIndex] = pos;
            OnPathChange();


        }

        Vector3[] positions;
        void OnSceneGUI(SceneView sceneView)
        {
            if (previewPath == null) return;
            

            positions = new Vector3[previewPath.Count];
            for (int i = 0; i< previewPath.Count; i++)
            {
                GameTile tile = EditorTools.CurrentInspectedGrid.GetTile(previewPath[i]);
                Vector3 position;

                if (tile) position = EditorTools.CurrentInspectedGrid.GetTile(previewPath[i]).CenterPoint;
                else position = Vector3.zero;

                positions[i] = position;

            }

            Color tmp = Handles.color;
            Handles.color = Color.red;
            Handles.DrawPolyLine(positions);
            Handles.color = tmp;
            for (int i = 0; i < objList.list.Count; i++)
            {
                GameTile tile = EditorTools.CurrentInspectedGrid.GetTile(objList.list[i]);
                Vector3 position;

                if (tile) position = EditorTools.CurrentInspectedGrid.GetTile(objList.list[i]).CenterPoint;
                else position = Vector3.zero;

                Handles.Label(position, i.ToString(), EditorStyles.helpBox);

            }
        }
    }
   

    public class PathSerializedObject: ScriptableObject
    {
        public List<GridPosition> list = new List<GridPosition>();
    }
   
    class TestWindow : PhysicsDebugWindow
    {
        [MenuItem("Game Tools/phycisc")]
        public static void Do()
        {
            GetWindow<TestWindow>();
        }
    }
    }
