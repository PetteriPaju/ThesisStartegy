using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace SengokuWarrior
{
    [CustomEditor(typeof(MissionCondition))]
    public class MissionConditionEditor : Editor
    {
        public static Dictionary<MissionCondition, MissionConditionEditor> cachedEditors = new Dictionary<MissionCondition, MissionConditionEditor>();

        protected Mission missionBeingEdited;

        public void Init(Mission mission)
        {
            missionBeingEdited = mission;
            cachedEditors.Add((MissionCondition)target, this);
        }

        public virtual void Show()
        {

        }
        public virtual void Hide()
        {
            ObjectHilighter.ClearIfLast(this);
        }

        public override void OnInspectorGUI()
        {
            MissionCondition t = target as MissionCondition;

        }

        private void OnDisable()
        {
            ObjectHilighter.ClearIfLast(this);
            cachedEditors.Remove((MissionCondition)target);
        }
    }


    [CustomEditor(typeof(TeamsDeadCondition))]
    public class TeamsDeadConiditon : MissionConditionEditor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            TeamsDeadCondition t = target as TeamsDeadCondition;
            if (missionBeingEdited == null) return;
            if (t == null) return;

            if (t.DeadTeams.Count != missionBeingEdited.Teams.Count)
            {
                while (t.DeadTeams.Count < missionBeingEdited.Teams.Count)
                {
                    t.DeadTeams.Add(false);
                }
                while (t.DeadTeams.Count > missionBeingEdited.Teams.Count)
                {
                    t.DeadTeams.RemoveAt(t.DeadTeams.Count - 1);
                }
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
            serializedObject.Update();
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Dead Teams");
            EditorGUI.indentLevel++;
            for (int i = 0; i<missionBeingEdited.Teams.Count; i++)
            {
              
                EditorGUILayout.PropertyField(serializedObject.FindProperty("DeadTeams").GetArrayElementAtIndex(i),new GUIContent("Team " + i.ToString()));
            }
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(missionBeingEdited);
                
            }

        }
    }


    [CustomEditor(typeof(UnitsDeadCondition))]
    public class UnitsDeadConditionEditor : MissionConditionEditor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UnitsDeadCondition t = target as UnitsDeadCondition;
            if (missionBeingEdited == null) return;
            if (t == null) return;
            serializedObject.Update();
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Dead Characters");
            if (GUILayout.Button("+",GUILayout.Width(25)))
            {
                GenericMenu CreateMenu = new GenericMenu();
                int _i = 0;
                for (int i = 0; i < missionBeingEdited.Teams.Count; i++)
                {
                    int _q = 0;
                    for (int q = 0; q < missionBeingEdited.Teams[i].members.Count; q++)
                    {
                        _i = i;
                        _q = q;
                        CreateMenu.AddItem(new GUIContent("Team " + i + "/" + missionBeingEdited.Teams[_i].members[_q]._Name), false, Add, missionBeingEdited.Teams[_i].members[_q].id.ToInt());
                    }
                }
                CreateMenu.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            for (int i = t.DeadCharacters.Count-1; i >= 0; i--)
            {
                Character chara = missionBeingEdited.FindCharacter(t.DeadCharacters[i]);
                if (chara != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(chara._Name);
                    if (GUILayout.Button("X", GUILayout.Width(25)))
                    {
                        Undo.RegisterCompleteObjectUndo(t, "Modify Condition");
                        t.DeadCharacters.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    Undo.RegisterCompleteObjectUndo(t, "Modify Condition");
                    t.DeadCharacters.RemoveAt(i);
                }

            }
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(missionBeingEdited);

            }

        }

        void Add(object obj)
        {
            UnitsDeadCondition t = (UnitsDeadCondition)target;

            int id = (int)obj;

            if (!t.DeadCharacters.Contains(id))
            {
                Undo.RegisterCompleteObjectUndo(t, "Modify Condition");
                t.DeadCharacters.Add(id);
            }

            EditorUtility.SetDirty(t);

        }
    }


    [CustomEditor(typeof(TurnsPassedCondition))]
    public class TurnsPasseConditionEditor : MissionConditionEditor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            TurnsPassedCondition t = target as TurnsPassedCondition;
            if (missionBeingEdited == null) return;
            if (t == null) return;
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Turn"));

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(missionBeingEdited);

            }

        }
    }

    [CustomEditor(typeof(UnitsAliveCondition))]
    public class UnitsAliveConditionEditor : MissionConditionEditor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UnitsAliveCondition t = target as UnitsAliveCondition;
            if (missionBeingEdited == null) return;
            if (t == null) return;
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("elements"),true);
            
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(missionBeingEdited);

            }

        }
    }

    [CustomEditor(typeof(PointReachedCondition))]
    public class PointReachedConditionEditor : MissionConditionEditor
    {
 
        public ReorderableList list;
        void Init()
        {
            serializedObject.Update();
            // Remove delegate listener if it has previously
            // been assigned.
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
            // Add (or re-add) the delegate.
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;
            list = CreateList(serializedObject, serializedObject.FindProperty("Positions"));

            RefershSelection();
        }

        private void RefershSelection()
        {
            foreach (GridPosition pos in ((PointReachedCondition)target).Positions)
            {
                GameTile tile = EditorTools.CurrentInspectedGrid.GetTile(pos);

                if (tile)
                {
                    ObjectHilighter.Add(tile.GetComponent<SpriteRenderer>(), this);
                }

            }
        }

        public override void Show()
        {
            base.Show();
            RefershSelection();

        }

        private void OnEnable()
        {
            Init();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (list == null) Init();

            if (!Application.isPlaying)
            {
                serializedObject.Update();
                list.DoLayoutList();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Team"));
                serializedObject.ApplyModifiedProperties();
    
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
            new Rect(rect.x + 60, rect.y, rect.width - 270, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("y"), GUIContent.none);
        EditorGUI.PropertyField(
            new Rect(rect.x + rect.width - 270 + 60, rect.y, 30, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("x"), GUIContent.none);

        GridPosition pos = ((PointReachedCondition)target).Positions[index];
        if (EditorTools.PickField(ref pos, new Rect(rect.x + rect.width - 80, rect.y, 40, EditorGUIUtility.singleLineHeight)))
        {
            OnPathChange();
        }
        if (GUI.Button(new Rect(rect.x + rect.width - 30, rect.y, 40, EditorGUIUtility.singleLineHeight),"-"))
        {
            ((PointReachedCondition)target).Positions.RemoveAt(index);
            OnPathChange();
        }
    };
            newList.onAddCallback = (ReorderableList l) => {
                //  GridPositionSelectionTool.Begin(CreateGridPosition);
                if (Selection.activeGameObject)
                {
                    if (Selection.activeGameObject.GetComponent<GameTile>())
                    {
                        foreach (UnityEngine.GameObject tile in Selection.gameObjects)
                        {
                            if (tile.GetComponent<GameTile>())
                            CreateGridPosition(EditorTools.CurrentInspectedGrid.FindTile(tile.GetComponent<GameTile>()));
                        }
                       
                    }
                }
            };

            newList.onSelectCallback = (ReorderableList l) =>
            {
                OnPathChange();
            };

            newList.onChangedCallback = (ReorderableList l) => {

                OnPathChange();
            };


            return newList;
        }

        public void CreateGridPosition(GridPosition pos)
        {
            list.serializedProperty.serializedObject.ApplyModifiedProperties();
            ((PointReachedCondition)target).Positions.Add(pos);

        }

        public void OnPathChange()
        {
            RefershSelection();
        }

        Vector3[] positions;
        void OnSceneGUI(SceneView sceneView)
        {
           
        }
    }



    }

