using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SengokuWarrior
{

    public class StatsEditor
    {

        private Dictionary<int, Editor> dict = new Dictionary<int, Editor>();
        private bool[] gearFolds = new bool[6] { false, false, false, false, false, false };
        private Mission miss;
        private UnityEngine.Object UndoObj;
        private Stats stats;

        private static List<Dictionary<string, NewItem>> slotItems = new List <Dictionary<string, NewItem>>();
        private static List<string[]> slotItemStrings = new List<string[]>();

        public void RefreshDictionary()
        {
            slotItems = new List<Dictionary<string, NewItem>>();
            slotItemStrings = new List<string[]>();
            for (int i =0; i< stats.Gear.items.Length; i++)
            {
                NewItem[] itms = EditorTools.ItemDatabase.GetItemsWithSlot(i);
                string[] itemnames = itms.Select(x => x.ItemName).ToArray();
                Dictionary<string, NewItem> dict = new Dictionary<string, NewItem>();
                slotItemStrings.Add(itemnames);
                for (int q =0; q<itms.Length; q++)
                {
                    dict.Add(itemnames[q], itms[q]);
                }

                slotItems.Add(dict);
            }

            foreach (var editor in dict.Values)
            {
                Editor.DestroyImmediate(editor);
            }
            dict.Clear();
            for (int i = 0; i<stats.Gear.items.Length; i++)
            {
                if(stats.Gear.items[i] != null)
                dict.Add(i, Editor.CreateEditor(stats.Gear.items[i]));
            }
        }
        public StatsEditor(Stats stats, Mission miss, UnityEngine.Object UndoObject)
        {
            this.stats = stats;
            this.miss = miss;
            this.UndoObj = UndoObject;
            RefreshDictionary();
        }

        public void OnGUI()
        {

            if (stats == null || UndoObj == null) return;
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Base stats");
            float baseHP = (int)EditorGUILayout.FloatField("HP", stats.BaseHP);
            float baseMP = (int)EditorGUILayout.FloatField("MP", stats.BaseMP);
            float baseAtt = (int)EditorGUILayout.FloatField("Attack", stats.BaseAttack);
            float baseDef = (int)EditorGUILayout.FloatField("Defence", stats.BaseDef);
            float baseSpeed = (int)EditorGUILayout.FloatField("Speed", stats.BaseSpeed);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            stats.CalculateStats();
            EditorGUILayout.LabelField("Calculated stats");
            EditorGUILayout.SelectableLabel(((int)stats.CalculatedHP).ToString(), GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.SelectableLabel(((int)stats.CalculatedMP).ToString(), GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.SelectableLabel(((int)stats.CalculatedAtt).ToString(), GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.SelectableLabel(((int)stats.CalculatedDef).ToString(), GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.SelectableLabel(((int)stats.CalculatedSpeed).ToString(), GUILayout.Height(EditorGUIUtility.singleLineHeight));

            EditorGUILayout.EndVertical();



            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                if (UndoObj != null) Undo.RegisterCompleteObjectUndo(UndoObj, "Modify Stats");

                stats.BaseHP = Mathf.Max(1, baseHP);
                stats.BaseMP = Mathf.Max(0, baseMP);
                stats.BaseAttack = Mathf.Max(1, baseAtt);
                stats.BaseDef = Mathf.Max(0, baseDef);
                stats.BaseSpeed = Mathf.Max(0, baseSpeed); ;
            }

            for (int i = 0; i < stats.Gear.items.Length; i++)
            {
                if (stats.Gear.items[i] != null)
                {
                    if (!dict.ContainsKey(i))
                    {
                        RefreshDictionary();
                        break;
                    }
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Weapon Types", EditorStyles.boldLabel, GUILayout.Width(120));

            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                var myEnumMemberCount = Enum.GetNames(typeof(Stats.WeaponType)).Length;

                GenericMenu CreateMenu = new GenericMenu();
            
                for (int i=0; i<myEnumMemberCount; i++)
                {
                    if (!stats.CanEquipWeapons.Contains(i))CreateMenu.AddItem(new GUIContent(((Stats.WeaponType)i).ToString()), false, AddToEquipables, i);
                }

                CreateMenu.ShowAsContext();
            }
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            int deleteIndex1 = -1;

            for (int i = 0; i < stats.CanEquipWeapons.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(((Stats.WeaponType)stats.CanEquipWeapons[i]).ToString(),GUILayout.Width(75));
                if (GUILayout.Button("x",GUILayout.Width(20)))
                {
                    deleteIndex1 = i;
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            if (deleteIndex1 != -1) {
                Undo.RecordObject(UndoObj, "Modify Stats");
                stats.CanEquipWeapons.RemoveAt(deleteIndex1);
            }
            

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Gear",EditorStyles.boldLabel);

            int deleteIndex = -1;
             for (int i = 0; i < stats.Gear.items.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(((Stats.GearSlots)i).ToString(),GUILayout.Width(100));
                if (stats.Gear.items[i]!= null || stats.Gear.Names[i] != null)
                {
                    EditorGUI.BeginDisabledGroup(stats.Gear.items[i] == null);

                    NewItem item = stats.Gear.items[i] != null ? stats.Gear.items[i] : stats.Gear.Names[i];

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    gearFolds[i] = EditorGUILayout.Foldout(gearFolds[i], item.ItemName);
                    if (gearFolds[i])
                    {
                        if (dict.ContainsKey(i)) dict[i].OnInspectorGUI();
                        else RefreshDictionary();
                    }
                    EditorGUILayout.EndVertical();

                    if (GUILayout.Button("X",GUILayout.Width(20))) deleteIndex = i;
                    
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    EditorGUILayout.HorizontalScope v = new EditorGUILayout.HorizontalScope();
                    using (v) {
                        EditorGUILayout.HelpBox("No Gear", MessageType.Info);
                    EditorGUI.BeginChangeCheck();

                        EditorGUILayout.BeginVertical();

                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                        int select = EditorGUILayout.Popup(0, slotItemStrings[i]);
                        EditorGUILayout.EndVertical();

                        if (EditorGUI.EndChangeCheck())
                    {
                        NewItem itm = slotItems[i][slotItemStrings[i][select]];
                        GearSlotAttribute attr = itm.GetAttribute<GearSlotAttribute>();
                        if (attr == null) Debug.LogWarning("This Item lack GearSlot Attribute and cannot be added to the Gear slot");
                        else
                        {
                                WeaponAttribute weaponAttr = itm.GetAttribute<WeaponAttribute>();
                                bool weaponCancel = false;
                                if (weaponAttr)
                                {
                                    if (!stats.CanEquipWeapons.Contains((int)weaponAttr.WeaponType)) {
                                        weaponCancel = true;
                                        Debug.LogWarning("This character cannot equip this weapon type");
                                    }

                                }

                                //Add to a slot
                                if (attr.PossibleSlots[i] && !weaponCancel)
                                {
                                    Undo.RecordObject(UndoObj, "Modify Stats");
                                    stats.Gear.Equip(itm, i,null);
                                }

                        }
                        RefreshDictionary();
                    }

                    }
                    Event e = Event.current;
                    if (e.isMouse && e.button == 1 && v.rect.Contains(e.mousePosition))
                    {
                        GenericMenu CreateMenu = new GenericMenu();
                        if (ItemEditor.CopiedItem == null)
                        CreateMenu.AddDisabledItem(new GUIContent("Paste"));
                        else
                        CreateMenu.AddItem(new GUIContent("Paste"), false, Paste,i);
                        CreateMenu.ShowAsContext();
                    }
                }
        
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }
            if (deleteIndex != -1)
            {
                Undo.RecordObject(UndoObj, "Modify Stats");
                stats.Gear.Unequip(deleteIndex, null);
                RefreshDictionary();
            }       
        }

        void AddToEquipables(object obj)
        {
            Undo.RecordObject(UndoObj, "Modify Stats");
            stats.CanEquipWeapons.Add((int)obj);
        }


        void Paste(object obj)
        {
            int i = (int)obj;
            NewItem itm = ItemEditor.CopiedItem;
            GearSlotAttribute attr = itm.GetAttribute<GearSlotAttribute>();
            if (attr == null) Debug.LogWarning("This Item lack GearSlot Attribute and cannot be added to the Gear slot");
            else
            {
                WeaponAttribute weaponAttr = itm.GetAttribute<WeaponAttribute>();
                bool weaponCancel = false;
                if (weaponAttr)
                {
                    if (!stats.CanEquipWeapons.Contains((int)weaponAttr.WeaponType)) weaponCancel = true;

                }
                //Add to a slot
                if (attr.PossibleSlots[i] && !weaponCancel)
                {
                    Undo.RecordObject(UndoObj, "Modify Stats");
                    stats.Gear.Equip(itm,i,null);
                }
                else Debug.LogWarning("This items GearSlot attribute does not allow this slot!");

            }
            RefreshDictionary();
        }
    }

        

}
