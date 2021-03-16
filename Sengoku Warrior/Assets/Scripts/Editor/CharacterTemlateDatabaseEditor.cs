using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SengokuWarrior
{
    [CustomEditor(typeof(CharacterTemplateDatabase))]
    public class CharacterTemlateDatabaseEditor : Editor
    {
        private Dictionary<Character, bool> itemFolds = new Dictionary<Character, bool>();
        private Dictionary<Character, CharacterEditor> cachedEditors = new Dictionary<Character, CharacterEditor>();

        void RefreshDictionaries()
        {
            CharacterTemplateDatabase database = (CharacterTemplateDatabase)target;


            foreach (var editor in cachedEditors.Values)
            {
                Editor.Destroy(editor);
            }

            cachedEditors.Clear();
            for (int i = 0; i < database.commonCharacters.Count; i++)
            {
                if (!itemFolds.ContainsKey(database.commonCharacters[i]))
                {
                    itemFolds[database.commonCharacters[i]] = false;
                }
                CharacterEditor newEdit = EditorWindow.CreateInstance<CharacterEditor>();
                newEdit.Do(database.commonCharacters[i], null, database,true);
                cachedEditors[database.commonCharacters[i]] = newEdit;
            }

            for (int i = 0; i < database.uniqueCharacters.Count; i++)
            {
                if (!itemFolds.ContainsKey(database.uniqueCharacters[i]))
                {
                    itemFolds[database.uniqueCharacters[i]] = false;
                }
                CharacterEditor newEdit = EditorWindow.CreateInstance<CharacterEditor>();
                newEdit.Do(database.uniqueCharacters[i], null, database,true);
                cachedEditors[database.uniqueCharacters[i]] = newEdit;
            }

        }
        void OnEnable()
        {
            RefreshDictionaries();
        }

        public override void OnInspectorGUI()
        {
            CharacterTemplateDatabase database = (CharacterTemplateDatabase)target;

            if (GUILayout.Button("+"))
            {

                GenericMenu CreateMenu = new GenericMenu();

                CreateMenu.AddItem(new GUIContent("Common"), false, AddToList,false);
                CreateMenu.AddItem(new GUIContent("Unique"), false, AddToList,true);
                CreateMenu.ShowAsContext();
            }

            GenerateList("Common Characters", false, database.commonCharacters);
            GenerateList("Unique Characters", true, database.uniqueCharacters);


        }

        private void GenerateList(string label, bool isUnique, List<Character> lst)
        {
            CharacterTemplateDatabase database = (CharacterTemplateDatabase)target;

            int deleteIndex = -1;

            for (int i = 0; i < lst.Count; i++)
            {
                if (!itemFolds.ContainsKey(lst[i]))
                {
                    RefreshDictionaries();
                }
            }
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            for (int i = 0; i < lst.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                itemFolds[lst[i]] = EditorGUILayout.Foldout(itemFolds[lst[i]], lst[i]._Name);
                if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20))) deleteIndex = i;
                EditorGUILayout.EndHorizontal();
                if (itemFolds[lst[i]]) itemFolds[lst[i]] = !cachedEditors[lst[i]].Render();

            }
            EditorGUI.indentLevel--;
            if (deleteIndex != -1)
            {
                lst.RemoveAt(deleteIndex);
                RefreshDictionaries();
            }
        }

        private void AddToList(object isUnique)
        {
            CharacterTemplateDatabase database = (CharacterTemplateDatabase)target;

            bool unique = (bool)isUnique;

            List<Character> lst;

            if (unique) lst = database.uniqueCharacters;
            else lst = database.commonCharacters;

            lst.Add(new Character(unique));


        }


    }
}
