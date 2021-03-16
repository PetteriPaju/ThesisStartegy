using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
namespace SengokuWarrior{ 
    [CustomEditor(typeof(ItemDatabase))]
    public class ItemdatabaseEditor : Editor
    {
        private Dictionary<NewItem, bool> itemFolds = new Dictionary<NewItem, bool>();
        private Dictionary<NewItem, Editor> cachedEditors = new Dictionary<NewItem, Editor>();

        private List<NewItem> displayedItems = new List<NewItem>();

        void RefreshDictionaries()
        {
            ItemDatabase database = (ItemDatabase)target;

            foreach(var editor in cachedEditors.Values)
            {
                Editor.DestroyImmediate(editor);
            }

            cachedEditors.Clear();
            for (int i = 0; i < database.items.Count; i++)
            {
                if (!itemFolds.ContainsKey(database.items[i]))
                {
                    itemFolds[database.items[i]] = false;
                }
                cachedEditors[database.items[i]] = Editor.CreateEditor(database.items[i]);
            }
        }
        void OnEnable()
        {
            RefreshDictionaries();
        }

        public override void OnInspectorGUI()
        {
            ItemDatabase database = (ItemDatabase)target;

            if (GUILayout.Button("+"))
            {

                GenericMenu CreateMenu = new GenericMenu();

                CreateMenu.AddItem(new GUIContent("Generic"), false, MenuCreateItem, new object[] {typeof(CommonAttributes) });
                CreateMenu.AddItem(new GUIContent("Weapon"), false, MenuCreateItem, new object[] { typeof(CommonAttributes), typeof(WeaponAttribute), typeof(GearSlotAttribute) });
                CreateMenu.AddItem(new GUIContent("Armor"), false, MenuCreateItem, new object[] { typeof(CommonAttributes), typeof(StatsAttribute), typeof(GearSlotAttribute) });
                CreateMenu.AddItem(new GUIContent("Potion"), false, MenuCreateItem, new object[] { typeof(CommonAttributes), typeof(Use_RecoverAttribute) });
                CreateMenu.ShowAsContext();
                /*
                var newAttribute = NewItem.CreateInstance<NewItem>();
                newAttribute.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(newAttribute, (ItemDatabase)target);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newAttribute));
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                ItemEditor.CreateAttribute(typeof(NameAttribute), newAttribute);
                ItemEditor.CreateAttribute(typeof(ValueAttribute), newAttribute);
                ItemEditor.CreateAttribute(typeof(StackableAttribute), newAttribute);
                ItemEditor.CreateAttribute(typeof(SpriteAttribute), newAttribute);
                database.AddItem(newAttribute);
                */
            }

            int deleteIndex = -1;

            for (int i = 0; i < database.items.Count; i++)
            {
                if (!itemFolds.ContainsKey(database.items[i]))
                {
                    RefreshDictionaries();
                }
            }

            for (int i = 0; i < database.items.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                itemFolds[database.items[i]] = EditorGUILayout.Foldout(itemFolds[database.items[i]], database.items[i].ItemName);
                if (GUILayout.Button("X", EditorStyles.miniButton,GUILayout.Width(20))) deleteIndex = i;
                EditorGUILayout.EndHorizontal();
                if (itemFolds[database.items[i]])cachedEditors[database.items[i]].OnInspectorGUI();   
                                  
            }
            if (deleteIndex != -1) {
                database.Destroy(database.items[deleteIndex]);
            }

        }

        public void MenuCreateItem(object array)
        {
            ItemEditor.CreateItemWithAttributes((ItemDatabase)target, array);
        }
    }
}
