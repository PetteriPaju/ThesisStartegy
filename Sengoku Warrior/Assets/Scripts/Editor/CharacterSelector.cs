using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System.Collections.Generic;

namespace SengokuWarrior
{
    public class CharacterSelector : EditorWindow
    {
        private static UnityAction<Character> callback;
        private static List<List<Character>> allCharacters;
        public static void DoIt(UnityAction<Character> callback, List<Team> teams)
        {
            CharacterSelector.callback = callback;
            allCharacters = TeamsToList(teams);
            GetWindow<CharacterSelector>();
           
        }

        public static void DoIt(UnityAction<Character> callback, List<List<Character>> characters)
        {
            CharacterSelector.callback = callback;
            allCharacters = characters;
            GetWindow<CharacterSelector>();

        }
        void OnDestroy()
        {
            callback = null;
        }

        private static List<List<Character>> TeamsToList(List<Team> teams)
        {
            List<List<Character>> newList = new List<List<Character>>();
            
            foreach(Team team in teams)
            {
                newList.Add(team.members);
            }

            return newList;
        }

        void OnGUI()
        {

            for (int i = 0; i<allCharacters.Count; i++)
            {
                EditorGUILayout.LabelField("Team #" + i.ToString());
                for (int q = 0; q<allCharacters[i].Count; q++)
                {


                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(allCharacters[i][q]._Name);
                    if (GUILayout.Button("Pick"))
                    {
                        if (callback != null) callback.Invoke(allCharacters[i][q]);
                        this.Close();
                    }
                    EditorGUILayout.EndHorizontal();

                }
                GUILayout.Space(10f);
            }



        }
    }
}