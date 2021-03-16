using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SengokuWarrior
{
    static class ME_ContainerEditor
    {
        public static void UpdateDictionary(this MissionEventContainer cont)
        {
            for (int i = 0; i < cont.allEvents.Count; i++)
            {
                if (!cont.EditorFolds.ContainsKey(cont.allEvents[i]))
                {
                    cont.EditorFolds.Add(cont.allEvents[i], false);
                }
            }
        }

        public static void OnGUI(this MissionEventContainer cont,Dictionary<MissionEvent,MissionEventEditor> editors, Mission miss = null)
        {

            EditorGUILayout.BeginHorizontal();
            cont.MainFold = EditorGUILayout.Foldout(cont.MainFold, cont.editorName, true);
            EditorGUI.BeginChangeCheck();

            if (GUILayout.Button("+",GUILayout.Width(25)))
            {
                GenericMenu CreateMenu = new GenericMenu();
                CreateMenu.AddItem(new GUIContent("Kill Character"), false, AddEventMenu, new object[] {cont,typeof(ME_KillCharacter),miss });
                CreateMenu.AddItem(new GUIContent("Give Item"), false, AddEventMenu, new object[] { cont, typeof(ME_GiveItem), miss });
                CreateMenu.AddItem(new GUIContent("Visu 16"), false, AddEventMenu, new object[] { cont, typeof(ME_Visu16), miss });

                CreateMenu.ShowAsContext();

            }
            EditorGUILayout.EndHorizontal();

    

            if (cont.MainFold)
            {
                EditorGUI.indentLevel++;
                MissionCondition.ConditionType t = cont.cond == null ? MissionCondition.ConditionType.Custom : cont.cond.conditionType;
                miss.ConditionSelector("Trigger when", ref cont.conditionFold, ref cont.cond, ref t);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
                for (int i = 0; i < cont.allEvents.Count; i++)
                {
                    if (!cont.EditorFolds.ContainsKey(cont.allEvents[i]))
                    {
                        cont.UpdateDictionary();
                    }
                }

                int deleteIndex = -1;
                for (int i = 0; i < cont.allEvents.Count; i++)
                {
                    if (cont.allEvents[i] != null)
                    {
                    
                        EditorGUILayout.BeginHorizontal();

                        cont.EditorFolds[cont.allEvents[i]] = EditorGUILayout.Foldout(cont.EditorFolds[cont.allEvents[i]], cont.allEvents[i].editorName + " (" + (cont.allEvents[i]).GetType().Name +")" );
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            deleteIndex = i;
                        }

                        EditorGUI.BeginDisabledGroup(i == 0);
                        if (GUILayout.Button("^", GUILayout.Width(20)))
                        {
                            cont.allEvents[i].listIndex--;
                            cont.allEvents[i - 1].listIndex++;
                            MissionEvent evt = cont.allEvents[i];
                            cont.allEvents[i] = cont.allEvents[i - 1];
                            cont.allEvents[i - 1] = evt;
                        }
                        EditorGUI.EndDisabledGroup();
                        EditorGUI.BeginDisabledGroup(i == cont.allEvents.Count - 1);
                        if (GUILayout.Button("v", GUILayout.Width(20)))
                        {
                            cont.allEvents[i].listIndex++;
                            cont.allEvents[i + 1].listIndex--;
                            MissionEvent evt = cont.allEvents[i];
                            cont.allEvents[i] = cont.allEvents[i + 1];
                            cont.allEvents[i + 1] = evt;
                        }
                        EditorGUI.EndDisabledGroup();


                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.indentLevel++;
                        GUILayout.Space(20 * EditorGUI.indentLevel);
                        
                        if (cont.EditorFolds[cont.allEvents[i]])
                        {
                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            editors[cont.allEvents[i]].OnGUI();
                            EditorGUILayout.EndVertical();
                        }
                            
                        EditorGUI.indentLevel--;
                        EditorGUILayout.EndHorizontal();
                        
                    }

                }
                if (deleteIndex != -1)
                {
                 
                    cont.EditorFolds.Remove(cont.allEvents[deleteIndex]);
                    cont.allEvents.RemoveAt(deleteIndex);
                    
                    cont.Save();
                }
                EditorGUI.indentLevel--;
            }
        }

        public static void AddEventMenu(object obj)
        {
            object[] arr = obj as object[];
            AddEvent((MissionEventContainer)arr[0],(System.Type)arr[1], (Mission)arr[2]);
        }

        public static void AddEvent(MissionEventContainer cont, Type t, Mission miss = null)
        {

            MissionEvent evt = null;

            evt = Activator.CreateInstance(t) as MissionEvent;

            if (evt == null) return;
            cont.allEvents.Add(evt);
            cont.Save();
        }



    }
}
