using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
namespace SengokuWarrior
{
    [CustomEditor(typeof(NewItem))]
    public class ItemEditor : Editor
    {
        public static NewItem CopiedItem = null;
        public static bool Debug = false;
        private Dictionary<ItemAttribute, AttributeEditor> cachedEditors = new Dictionary<ItemAttribute, AttributeEditor>();

        void OnEnable()
        {
            CacheEditors();
        }

        private void CacheEditors()
        {
            if (target == null) return;


            foreach(var editor in cachedEditors.Values)
            {
                Editor.DestroyImmediate(editor);
            }

            cachedEditors.Clear();
            NewItem _target = (NewItem)target;
            for (int i = 0; i < ((NewItem)target).attributes.Count; i++)
            {
             
                cachedEditors.Add(_target.attributes[i], AttributeEditor.CreateEditor(_target.attributes[i]) as AttributeEditor);
            }
        }

        public override void OnInspectorGUI()
        {
            NewItem _target = (NewItem)target;
             if (target == null) return;

            EditorGUILayout.VerticalScope v = new EditorGUILayout.VerticalScope();
            using (v)
            {
                EditorGUI.BeginChangeCheck();
                string name = EditorGUILayout.TextField("Editor name", _target.ItemName);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(_target, "Modify Item");
                    _target.ItemName = name;
                    EditorTools.ItemDatabase.RefreshDictionary();
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Attributes", EditorStyles.boldLabel);
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    GenericMenu CreateMenu = new GenericMenu();

                    CreateMenu.AddItem(new GUIContent("Generic"), false, MenuItemCreateAttribute, new object[] { typeof(CommonAttributes) });
                    CreateMenu.AddItem(new GUIContent("Weapons/Stats Attribute"), false, MenuItemCreateAttribute, new object[] { typeof(StatsAttribute) });
                    CreateMenu.AddItem(new GUIContent("Weapons/Weapon Attribute"), false, MenuItemCreateAttribute, new object[] {typeof(WeaponAttribute)});
                    CreateMenu.AddItem(new GUIContent("Weapons/Slot Attribute"), false, MenuItemCreateAttribute, new object[] {typeof(GearSlotAttribute) });
                    CreateMenu.AddItem(new GUIContent("Potion/Recover"), false, MenuItemCreateAttribute, new object[] { typeof(Use_RecoverAttribute) });


                    CreateMenu.ShowAsContext();
                }
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < _target.attributes.Count; i++)
                {
                    if (!cachedEditors.ContainsKey(_target.attributes[i])) CacheEditors();
                }
                EditorGUI.indentLevel++;
                int deleteIndex = -1;
                for (int i = 0; i < _target.attributes.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    cachedEditors[_target.attributes[i]].OnInspectorGUI();

                    EditorGUI.BeginDisabledGroup(!cachedEditors[_target.attributes[i]].CanDelete() && !Debug);
                    if (GUILayout.Button("X", GUILayout.Width(20))) deleteIndex = i;
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndHorizontal();

                }
                EditorGUI.indentLevel--;
                if (deleteIndex != -1)
                {
                    cachedEditors.Remove(_target.attributes[deleteIndex]);
                    _target.Destroy(_target.attributes[deleteIndex]);
                }
            }
            Event e = Event.current;

            if (e.isMouse && e.button == 1 && v.rect.Contains(e.mousePosition))
            {
                GenericMenu CreateMenu = new GenericMenu();
                CreateMenu.AddItem(new GUIContent("Debug Mode"), Debug, ToggleDebug);
                CreateMenu.AddItem(new GUIContent("Copy"), false, Copy, _target);
                CreateMenu.ShowAsContext();
            }


        }

        private void ToggleDebug()
        {
            Debug = !Debug;
        }
        private void Copy(object obj)
        {
            CopiedItem = (NewItem)obj;
        }
        public void MenuItemCreateAttribute(object attribute)
        {
            object[] arr = attribute as object[];
            foreach (object obj in arr)
            {
                CreateAttribute(obj, (NewItem)target);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void CreateAttribute(object attribute, NewItem itm)
        {
            var newAttribute = CreateInstance((System.Type)attribute) as ItemAttribute;
            newAttribute.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(newAttribute, itm);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newAttribute));

            (itm).Add(newAttribute);
        }

        public static NewItem CreateItemWithAttributes (ItemDatabase database, object arributeArray)
        {

            var Item = NewItem.CreateInstance<NewItem>();
            Item.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(Item, database);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Item));


            object[] arr = arributeArray as object[];
            foreach (object obj in arr)
            {
                CreateAttribute(obj, Item);
            }
            database.AddItem(Item);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return Item;
        }

    }
}

