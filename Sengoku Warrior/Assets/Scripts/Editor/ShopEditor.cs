using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace SengokuWarrior
{
    [CustomEditor(typeof(Shop))]
    public class ShopEditor : Editor
    {
        private Dictionary<NewItem, ItemEditor> Editors = new Dictionary<NewItem, ItemEditor>();
        private Dictionary<ItemStack, bool> itemFolds = new Dictionary<ItemStack, bool>();
        SceneAsset MapScene = null;
        private void RefreshDictionary()
        {
            Shop targetShop = (Shop)target;

            foreach(ItemStack stack in targetShop.Items)
            {
                if (!Editors.ContainsKey(stack.item))
                {
                    ItemEditor editor = ItemEditor.CreateEditor(stack.item) as ItemEditor;
                    Editors.Add(stack.item, editor);
                }

                if (!itemFolds.ContainsKey(stack))
                {
                    itemFolds.Add(stack, false);
                }
            }
                List<ItemStack> deleteKeys = new List<ItemStack>();
                foreach (var item in itemFolds.Keys)
                {
                    if (!targetShop.Items.Contains(item))
                    {
                        deleteKeys.Add(item);
                    }
                }

                for (int i = 0; i<deleteKeys.Count; i++)
                {
                itemFolds.Remove(deleteKeys[i]);
                }

                 List<NewItem> deleteItems = new List<NewItem>();
                 foreach (var item in Editors.Keys)
                {
                    if (targetShop.Items.Find(obj => obj.item == item) == null)
                    {
                    deleteItems.Add(item);
                    }

                }

                for (int i = 0; i < deleteItems.Count; i++)
                {
                    Editor.DestroyImmediate(Editors[deleteItems[i]]);
                    Editors.Remove(deleteItems[i]);
                }
       

        }

        void OnEnable()
        {
            RefreshDictionary();
            Shop targetShop = (Shop)target;
            if (targetShop.scenePath != string.Empty)
            {
                MapScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(targetShop.scenePath);
            }
        }

        public override void OnInspectorGUI()
        {

            Shop targetShop = (Shop)target;

            targetShop.NextLoadable = EditorGUILayout.ObjectField("Next Object", targetShop.NextLoadable, typeof(Loadable), false) as Loadable;
            EditorGUI.BeginChangeCheck();
            MapScene = EditorGUILayout.ObjectField("Scene", MapScene, typeof(SceneAsset), false) as SceneAsset;

            if (EditorGUI.EndChangeCheck())
            {
               targetShop.scenePath = AssetDatabase.GetAssetPath(MapScene);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            int addItem = EditorGUILayout.Popup("Add Item", 0, EditorTools.ItemDatabase.ItemNames);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetShop, "Modify Inventory");
                int index = targetShop.Items.FindIndex(a => a.item == EditorTools.ItemDatabase.items[addItem]);
                if (index == -1)
                {
                    targetShop.Items.Add(new ItemStack(EditorTools.ItemDatabase.items[addItem]));
                    targetShop.valueOverrides.Add(EditorTools.ItemDatabase.items[addItem].GetAttribute<CommonAttributes>().value);
                }
                    
                else
                {
                    targetShop.Items[index].amount++;
                }
            }
            EditorGUI.BeginDisabledGroup(ItemEditor.CopiedItem == null);
            if (GUILayout.Button("Paste"))
            {
                int index = targetShop.Items.FindIndex(a => a.item == ItemEditor.CopiedItem);
                Undo.RecordObject(targetShop, "Modify Inventory");
                if (index == -1)
                {
                    targetShop.Items.Add(new ItemStack(ItemEditor.CopiedItem));
                    targetShop.valueOverrides.Add(EditorTools.ItemDatabase.items[addItem].GetAttribute<CommonAttributes>().value);
                }
              
                else
                {
                    targetShop.Items[index].amount++;
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();


            for (int i = 0; i < targetShop.Items.Count; i++)
            {
                if (!Editors.ContainsKey(targetShop.Items[i].item))
                {
                    RefreshDictionary();
                }
            }


            EditorGUILayout.LabelField("Items", EditorStyles.boldLabel);
            int deleteIndex = -1;
            for (int i = 0; i < targetShop.Items.Count; i++)
            {
            
                    if (targetShop.Items[i].item == null)
                    {
                        targetShop.Items.RemoveAt(i);
                        targetShop.valueOverrides.RemoveAt(deleteIndex);
                        continue;
                    }

                    EditorGUILayout.BeginHorizontal();
                    itemFolds[targetShop.Items[i]] = EditorGUILayout.Foldout(itemFolds[targetShop.Items[i]], targetShop.Items[i].item.ItemName);
                    
                if (GUILayout.Button("X", GUILayout.Width(20)) || targetShop.Items[i].amount <= 0)
                    {
                        deleteIndex = i;
                    EditorGUIUtility.hotControl = 0;
                    }

             
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel++;
                targetShop.Items[i].amount = EditorGUILayout.IntField("Amount", targetShop.Items[i].amount);
                targetShop.valueOverrides[i] = EditorGUILayout.FloatField("Cost", targetShop.valueOverrides[i]);

                if (itemFolds[targetShop.Items[i]])
                {
                    Editors[targetShop.Items[i].item].OnInspectorGUI();
                }
                EditorGUI.indentLevel--;

            }
            if (deleteIndex != -1)
            {
                Undo.RegisterCompleteObjectUndo(targetShop, "Remove Item");
                targetShop.Items.RemoveAt(deleteIndex);
                targetShop.valueOverrides.RemoveAt(deleteIndex);
                RefreshDictionary();
            }

            if (GUI.changed && !Application.isPlaying && targetShop != null)
            {
                EditorUtility.SetDirty(targetShop);
            }

        }

    }
}
