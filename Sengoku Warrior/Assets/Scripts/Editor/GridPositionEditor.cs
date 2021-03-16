using UnityEditor;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace SengokuWarrior
{
    public static class EditorTools {


        private static ItemDatabase itemDatabase;

        public static ItemDatabase ItemDatabase{

            get
            {
                if (itemDatabase == null)
                {
                    itemDatabase = AssetDatabase.LoadAssetAtPath<ItemDatabase>("Assets/Databases/Itemdatabase.asset");

                    if (itemDatabase == null) Debug.LogWarning("No Itemdatabase found!");

                }

                return itemDatabase;
            }
            set
            {
                itemDatabase = value;
            }
            }
        private static CharacterTemplateDatabase charaDatabase;

        public static CharacterTemplateDatabase CharaDatabase
        {

            get
            {
                if (charaDatabase == null)
                {
                    charaDatabase = AssetDatabase.LoadAssetAtPath<CharacterTemplateDatabase>("Assets/Databases/CharacterDatabase.asset");

                    if (charaDatabase == null) Debug.LogWarning("No Itemdatabase found!");

                }

                return charaDatabase;
            }
            set
            {
                charaDatabase = value;
            }
        }

        private static LoadableDatabase loadableDatabase;

        public static LoadableDatabase LoadableDatabase
        {

            get
            {
                if (loadableDatabase == null)
                {
                    loadableDatabase = AssetDatabase.LoadAssetAtPath<LoadableDatabase>("Assets/Databases/LoadableDatabase.asset");

                    if (loadableDatabase == null) Debug.LogWarning("No Itemdatabase found!");

                }

                return loadableDatabase;
            }
            set
            {
                loadableDatabase = value;
            }
        }

        private static GameGrid currentInpectedGrid;
        public static GameGrid CurrentInspectedGrid
        {
            get
            {
                if (currentInpectedGrid == null)
                {
                    currentInpectedGrid = Component.FindObjectOfType<GameGrid>();
                }
                else if (GameGrid.currentGrid != null && currentInpectedGrid != GameGrid.currentGrid)
                {
                    currentInpectedGrid = GameGrid.currentGrid;
                }

                return currentInpectedGrid;
            }

            set
            {
                currentInpectedGrid = value;
            }
        }




      
        
        public static void ConditionSelector(this Mission mission, string label, ref bool fold, ref MissionCondition cond, ref MissionCondition.ConditionType type)
        {
            bool foldChanged = false;

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();

            fold = EditorGUILayout.Foldout(fold, new GUIContent(label));

            if (EditorGUI.EndChangeCheck())foldChanged = true;
            
            EditorGUI.BeginChangeCheck();
            type = (MissionCondition.ConditionType)EditorGUILayout.EnumPopup(type);
            if (EditorGUI.EndChangeCheck())
            {
                if (cond != null)
                {
                    if (AssetDatabase.GetAssetPath(cond) == AssetDatabase.GetAssetPath(mission))
                    {
                        GameObject.DestroyImmediate(cond, true);
                    }
                    cond = null;
                    fold = false;
                }

                switch (type)
                {
                    case MissionCondition.ConditionType.TeamsDead:
                        cond = TeamsDeadCondition.CreateInstance<TeamsDeadCondition>();
                        break;
                    case MissionCondition.ConditionType.UnitsDead:
                        cond = UnitsDeadCondition.CreateInstance<UnitsDeadCondition>();
                        break;
                    case MissionCondition.ConditionType.UnitsDeadAmount:
                        cond = UnitsAliveCondition.CreateInstance<UnitsAliveCondition>();
                        break;
                    case MissionCondition.ConditionType.PointReached:
                        cond = PointReachedCondition.CreateInstance<PointReachedCondition>();
                        break;

                    case MissionCondition.ConditionType.Custom:
                        break;
                    case MissionCondition.ConditionType.TurnsPassed:
                        cond = TurnsPassedCondition.CreateInstance<TurnsPassedCondition>();
                        break;
                    default:
                        break;

                }
                if (cond)
                {
                    fold = true;
                    cond.conditionType = type;
                    cond.name = cond.GetType().ToString();
                    //Hide the file
                    cond.hideFlags = HideFlags.HideInHierarchy;
                    //Attatch file to the Mission ScriptableObject
                    AssetDatabase.AddObjectToAsset(cond, mission);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(cond));
                }
                AssetDatabase.SaveAssets();

            }
            EditorGUILayout.EndHorizontal();
            MissionCondition condition = cond;
            if (fold)
            {
                if (cond != null)
                {

                    if (MissionConditionEditor.cachedEditors.ContainsKey(cond) && MissionConditionEditor.cachedEditors[cond] != null)
                    {
           
                        MissionConditionEditor.cachedEditors[cond].OnInspectorGUI();
                    }
                    else {
                        //Create editor for the condition
                        Editor ConditionEditor = Editor.CreateEditor(condition);

                        //See if the Inpector can be casted to MissionConditionEditor
                        if (ConditionEditor as MissionConditionEditor != null)
                        {
                            //Initialize custom condition editor
                            ((MissionConditionEditor)ConditionEditor).Init(mission); 
                        }
                        //Draw the editor
                        ConditionEditor.OnInspectorGUI();
                    }
                }
                else
                {
                    EditorGUI.indentLevel++;
                    cond = EditorGUILayout.ObjectField("Custom Condition", cond, typeof(MissionCondition), false) as MissionCondition;
                    EditorGUI.indentLevel--;
                }
            }

            if (foldChanged && cond != null && MissionConditionEditor.cachedEditors.ContainsKey(cond) && MissionConditionEditor.cachedEditors[cond] != null)
            {
                if (MissionConditionEditor.cachedEditors[cond] as MissionConditionEditor != null)
                {
                    if (fold) MissionConditionEditor.cachedEditors[cond].Show();
                    else MissionConditionEditor.cachedEditors[cond].Hide();
                }
            }


        }


        public static void EditorField(this GridPosition pos)
        {
            pos.x = EditorGUILayout.IntField(pos.x);
            pos.y = EditorGUILayout.IntField(pos.y);
            pos.z = EditorGUILayout.IntField(pos.z);
        }
        public static void _RegisterUndo(this Mission mission)
        {
            if (!Application.isPlaying)
            {
                Undo.RecordObject(mission, "Modify Mission");
            }
        }

        public static bool PickField(ref GridPosition pos)
        {
            bool picked = false;
            GameTile selectedTile = null;

            if (Selection.activeGameObject != GridEditor.selectedTile && Selection.activeGameObject != null) selectedTile = Selection.activeGameObject.GetComponent<GameTile>();
            EditorGUI.BeginDisabledGroup(GridEditor.selectedTile == null && selectedTile == null);

            if (GUILayout.Button("Pick", GUILayout.Width(40), GUILayout.Height(22)))
            {

                    if (selectedTile != null)
                    {
                        pos.Set(EditorTools.CurrentInspectedGrid.FindTile(selectedTile));
                    }
                    else
                    {
                        pos.Set(GridEditor.Selected.Clone());
                    }

                picked = true;
            }
            EditorGUI.EndDisabledGroup();


            return picked;
        }
        public static bool PickField(ref GridPosition pos, Rect rect)
        {
            bool picked = false;
            GameTile selectedTile = null;

            if (Selection.activeGameObject != GridEditor.selectedTile && Selection.activeGameObject != null) selectedTile = Selection.activeGameObject.GetComponent<GameTile>();
            EditorGUI.BeginDisabledGroup(GridEditor.selectedTile == null && selectedTile == null);

            if (GUI.Button(rect,"Pick"))
            {

                if (selectedTile != null)
                {
                    pos.Set(EditorTools.CurrentInspectedGrid.FindTile(selectedTile));
                }
                else
                {
                    pos.Set(GridEditor.Selected.Clone());
                }

                picked = true;
            }
            EditorGUI.EndDisabledGroup();


            return picked;
        }
        public static void EditorField(this GridPosition pos,Mission mission ,GameGrid grid, GUIContent prefix)
        {

            EditorGUI.BeginChangeCheck();
         
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(prefix);
            GUILayout.FlexibleSpace();
            int x = IntField(pos.x,new GUIContent("X"));
            GUILayout.Space(2);
           // GUILayout.FlexibleSpace();
            int y = IntField(pos.y, new GUIContent("Y"));
            GUILayout.Space(2);
            int z = IntField(pos.z, new GUIContent("Z"));
           // GUILayout.FlexibleSpace();
            if (PickField(ref pos))
            {

                Debug.Log(pos.ToString());
            }
            else if (EditorGUI.EndChangeCheck())
            {

                Undo.RecordObject(mission, "Modify Grid Position");
                pos.x = x;
                pos.y = y;
                pos.z = z;

                if (pos.x < 0) pos.x = 0;
                else if (pos.x > grid.columns) pos.x = grid.columns;
                if (pos.y < 0) pos.y = 0;
                else if (pos.y > grid.rows) pos.y = grid.rows;
                if (pos.z < 0) pos.z = 0;
                else if (pos.z > grid.layers.Count) pos.z = grid.layers.Count;

            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

      
  
        }

        private static int IntField(int number, GUIContent label)
        {
            int num = number;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(15));
            GUILayout.Space(-20);
            num = EditorGUILayout.IntField(num,GUILayout.Height(22), GUILayout.MaxWidth(45));

            EditorGUILayout.BeginVertical(GUILayout.Width(15));

            if (GUILayout.Button("^", GUILayout.Height(12))) num++;
            GUILayout.Space(-4);
            if (GUILayout.Button("v", GUILayout.Height(12))) num--;
            EditorGUILayout.EndVertical();
      
            EditorGUILayout.EndHorizontal();

            return num;
        }

        public static class GenericCopier<T>
        {
            public static T DeepCopy(object objectToCopy)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, objectToCopy);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return (T)binaryFormatter.Deserialize(memoryStream);
                }
            }
        }

    }
    }


