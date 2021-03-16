using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace SengokuWarrior
{
    [SerializeField]
    class CharacterEditor : IEmbedable
    {
        private MissionCondition.ConditionType currentSelectedCtype = MissionCondition.ConditionType.Custom;
        private Dictionary<ItemStack, bool> itemFolds = new Dictionary<ItemStack, bool>();
        private Dictionary<ItemStack, Editor> cachedEditors = new Dictionary<ItemStack, Editor>();
        private StatsEditor statsEditor;
        private Vector2 scroll = Vector2.zero;
        [SerializeField]
        private bool[] Folds = new bool[] { true, false, false, false };
        [SerializeField]
        private Character character;
        [SerializeField]
        private Mission mission;
        [SerializeField]
        private UnityEngine.Object UndoObject;
        [SerializeField]
        private MissionInspector parent;
        private static bool ShowSpanwCondFold = false;
        private static bool ShowwSpawnFold = false;

        [SerializeField]
        private Editor itemEditor = null;

        public static Character CopyCharacter = null;

        private bool showUniqueControls = true;



        void RefreshDictionaries()
        {

            foreach (var editor in cachedEditors.Values)
            {
                Editor.DestroyImmediate(editor);
            }
            cachedEditors.Clear();
           for (int i = 0; i<character.inventroy.Items.Count; i++)
            {
                if (!itemFolds.ContainsKey(character.inventroy.Items[i]))
                {
                    itemFolds[character.inventroy.Items[i]] = false;
                }
                cachedEditors[character.inventroy.Items[i]] = Editor.CreateEditor(character.inventroy.Items[i].item);
            }
        }

        void OnDisable()
        {
            if (this.character == null) return;
  
            AssetDatabase.SaveAssets();
        }

        public void Do(Character chara, Mission mission, UnityEngine.Object UndoObject, bool allowUnique)
        {
            this.character = chara;

            if (this.character.isunique)
            {
                Character orgChara = EditorTools.CharaDatabase.GetCharacterWithID(this.character.id);

                if (orgChara != null) chara.SoftClone(orgChara);
                else Debug.LogWarning("Unique character has no equivalant in Database!");
            }

            this.mission = mission;
            this.UndoObject = UndoObject;
            if (chara.SpawnCondition != null)
            {
                currentSelectedCtype = chara.SpawnCondition.conditionType;
            }
            statsEditor = new StatsEditor(chara.stats, mission, UndoObject);
            if (chara.isunique && !allowUnique) showUniqueControls = false;


  

            RefreshDictionaries();
        }
        void OnGUI()
        {

            if (Render())
                this.Close();
        }

        public void SetParent(MissionInspector parent)
        {
            this.parent = parent;
        }

        public override bool Render()
        {
            if (this.character == null) return false;


            scroll = EditorGUILayout.BeginScrollView(scroll);
            bool delete = false;
            bool close = false;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.VerticalScope clickArea = new EditorGUILayout.VerticalScope();
            using (clickArea)
            {
                EditorGUILayout.BeginHorizontal();
                if (showUniqueControls)
                {
                    EditorGUI.BeginChangeCheck();
                    GUI.SetNextControlName("user");
                    GUIStyle style = GUI.GetNameOfFocusedControl() == "user" ? EditorStyles.textField : EditorStyles.label;
                    string charactername = EditorGUILayout.TextField(GUIContent.none, this.character._Name, style);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(UndoObject, "Modify Character");
                        this.character._Name = charactername;
                    }
                }
                else
                {
                    EditorGUILayout.LabelField(this.character._Name, EditorStyles.label);
                }

                if (this.parent != null)
                {
                    if (GUILayout.Button("Pop"))
                    {
                        this.Show();
                        TeamEditor teamEdit = Editor.CreateInstance<TeamEditor>();

                        teamEdit.Do(mission.FindTeam(character), mission);
                        teamEdit.SetParent(this.parent);
                        this.parent.auxWindow = teamEdit;
                        this.parent = null;
                    }
                }

                if (GUILayout.Button("X"))
                {
                    Undo.RecordObject(UndoObject, "Remove Character");
                    if (this.parent != null)
                    {
                        TeamEditor teamEdit = Editor.CreateInstance<TeamEditor>();
                        teamEdit.Do(mission.FindTeam(this.character), mission);
                        teamEdit.SetParent(this.parent);
                        this.parent.auxWindow = teamEdit;
                        this.parent = null;
                    }
                    else this.Close();


                    DestroyImmediate(character.SpawnCondition, true);
                    mission.FindTeam(this.character).members.Remove(this.character);
                    mission.RefreshCharacterDictionary();

                }

                if (GUILayout.Button("Close", GUILayout.Width(55))) close = true;

                EditorGUILayout.EndHorizontal();

                
                EditorGUI.BeginDisabledGroup(!showUniqueControls);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                Sprite spr = EditorGUILayout.ObjectField(GUIContent.none, this.character.UIIcon, typeof(Sprite), false, GUILayout.Width(75)) as Sprite;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    this.character.UIIcon = spr;
                }
                    EditorGUI.EndDisabledGroup();
                this.character.prefab = EditorGUILayout.ObjectField("Prefab", this.character.prefab, typeof(CharacterBody), false) as CharacterBody;
                GUILayout.Space(10);
                EditorGUILayout.LabelField("AI", EditorStyles.boldLabel);

                EditorGUI.indentLevel++;

             

                EditorGUI.BeginChangeCheck();
                AIBehaviour _behav = EditorGUILayout.ObjectField("Ai Behaviour", this.character.aiBehaviour, typeof(AIBehaviour), false) as AIBehaviour;
                if (EditorGUI.EndChangeCheck()) { Undo.RecordObject(UndoObject, "Modify AI"); this.character.aiBehaviour = _behav; }


                if (this.character.aiBehaviour && mission != null)
                {
                    if (character.onStartPath.Count != 0)
                    {
                        if (GUILayout.Button("Modify Path")) PathMaker.Begin(SetStartPath, character.onStartPath);

                    }
                    else
                    {
                        if (GUILayout.Button("Create Path")) PathMaker.Begin(SetStartPath, new List<GridPosition>());
                    }
                }
                GUILayout.Space(10);
                EditorGUI.indentLevel--;
                if (mission != null)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    Folds[0] = EditorGUILayout.Foldout(Folds[0], GUIContent.none);
                    GUILayout.Space(-40);
                    EditorGUILayout.LabelField("Spawning", EditorStyles.boldLabel);                   
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    if (Folds[0])
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(18);
                        this.character.SpawnPositon.EditorField(mission, EditorTools.CurrentInspectedGrid, new GUIContent("Position"));
                        EditorGUILayout.EndHorizontal();


                        mission.ConditionSelector("Spawn Condition", ref ShowSpanwCondFold, ref character.SpawnCondition, ref currentSelectedCtype);

                        EditorGUI.BeginChangeCheck();
                        GameGrid.BlockDirections.FaceDirections dir = (GameGrid.BlockDirections.FaceDirections)EditorGUILayout.EnumPopup("Face direction:", character.SpawnDirection);

                        if (EditorGUI.EndChangeCheck()) { Undo.RecordObject(UndoObject, "Change Direction"); character.SpawnDirection = dir; }
                        EditorGUI.indentLevel--;
                    }
                }
            }

            Event e = Event.current;

            if (e.isMouse && e.button == 1 && clickArea.rect.Contains(e.mousePosition))
            {
                GenericMenu CreateMenu = new GenericMenu();
                if (CopyCharacter != null)
                    CreateMenu.AddItem(new GUIContent("Paste Character Values"), false, Paste);
                else
                    CreateMenu.AddDisabledItem(new GUIContent("Paste Character Values"));
                CreateMenu.AddItem(new GUIContent("Copy Character Values"), false, Copy);
                CreateMenu.ShowAsContext();
            }

            EditorGUI.BeginDisabledGroup(!showUniqueControls);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            Folds[1] = EditorGUILayout.Foldout(Folds[1], GUIContent.none);
            GUILayout.Space(-40);
            EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (Folds[1])
            {
                EditorGUI.indentLevel++;
                if (statsEditor == null) statsEditor = new StatsEditor(character.stats, mission, UndoObject);
                statsEditor.OnGUI();

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            Folds[2] = EditorGUILayout.Foldout(Folds[2], GUIContent.none);
            GUILayout.Space(-40);
            EditorGUILayout.LabelField("Inventory", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (Folds[2])
            {
                EditorGUI.BeginChangeCheck();

                for (int i = 0; i < character.inventroy.Items.Count; i++)
                {
                    if (!itemFolds.ContainsKey(character.inventroy.Items[i]))
                    {
                        RefreshDictionaries();
                        break;
                    }
                }

                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                int addItem = EditorGUILayout.Popup("Add Item", 0, EditorTools.ItemDatabase.ItemNames);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(UndoObject, "Modify Inventory");
                    int index = character.inventroy.Items.FindIndex(a => a.item == EditorTools.ItemDatabase.items[addItem]);
                    if (index == -1)
                        character.inventroy.Items.Add(new ItemStack(EditorTools.ItemDatabase.items[addItem]));
                    else
                    {
                        character.inventroy.Items[index].amount++;
                    }
                }
                EditorGUI.BeginDisabledGroup(ItemEditor.CopiedItem == null);
                if (GUILayout.Button("Paste"))
                {
                    int index = character.inventroy.Items.FindIndex(a => a.item == ItemEditor.CopiedItem);
                    Undo.RecordObject(UndoObject, "Modify Inventory");
                    if (index == -1)
                        character.inventroy.Items.Add(new ItemStack(ItemEditor.CopiedItem));
                    else
                    {
                        character.inventroy.Items[index].amount++;
                    }
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();

                int deleteIndex = -1;
                for (int i = 0; i < character.inventroy.Items.Count; i++)
                {
                    if (itemFolds.ContainsKey(character.inventroy.Items[i]))
                    {
                        if (character.inventroy.Items[i].item == null)
                        {
                            character.inventroy.Items.RemoveAt(i);
                            continue;
                        }
                        EditorGUILayout.BeginHorizontal();
                        itemFolds[character.inventroy.Items[i]] = EditorGUILayout.Foldout(itemFolds[character.inventroy.Items[i]], character.inventroy.Items[i].item.ItemName);
                        character.inventroy.Items[i].amount = EditorGUILayout.IntField(character.inventroy.Items[i].amount);

                        if (GUILayout.Button("X", GUILayout.Width(20)) || character.inventroy.Items[i].amount <= 0)
                        {
                            deleteIndex = i;
                        }

                        EditorGUILayout.EndHorizontal();
                        if (itemFolds[character.inventroy.Items[i]])
                        {
                            EditorGUI.indentLevel++;
                            cachedEditors[character.inventroy.Items[i]].OnInspectorGUI();
                            EditorGUI.indentLevel--;
                        }


                    }
                    else RefreshDictionaries();
                }
                if (deleteIndex != -1)
                {
                    Undo.RecordObject(UndoObject, "Modify Inventory");
                    character.inventroy.Items.RemoveAt(deleteIndex);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;

            }
            if (GUI.changed && !Application.isPlaying)
            {
                EditorUtility.SetDirty(UndoObject);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            return delete || close;

        }

        public void Copy()
        {
            CopyCharacter = character.Clone(false);
        }
        public void Paste()
        {
            character._Name = CopyCharacter._Name;
            List<int> equipbables = character.stats.CanEquipWeapons.Select(item => (int)item).ToList();

            character.stats = new Stats(CopyCharacter._Name, CopyCharacter.stats.Level, CopyCharacter.stats.BaseHP, CopyCharacter.stats.BaseMP, CopyCharacter.stats.BaseAttack, CopyCharacter.stats.BaseDef, CopyCharacter.stats.BaseSpeed,CopyCharacter.stats.Gear.Clone(), equipbables, character);
            character.inventroy.Items = CopyCharacter.inventroy.Items.Select(item => (ItemStack)item).ToList();
            character.aiBehaviour = CopyCharacter.aiBehaviour;
            character.prefab = CopyCharacter.prefab;   
        }

        public void SetStartPath(List<GridPosition> lst)
        {
            this.character.onStartPath = lst;
            EditorUtility.SetDirty(mission);

        }
    }
}
