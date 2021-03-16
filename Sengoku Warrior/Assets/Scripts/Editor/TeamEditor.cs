using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SengokuWarrior
{
    [SerializeField]
    public class TeamEditor :IEmbedable
    {
        [SerializeField]
        private Team team;
        [SerializeField]
        private MissionInspector parent;
        [SerializeField]
        private Mission mission;
        [SerializeField]
        private Vector2 scroll = Vector2.zero;

        public void SetParent(MissionInspector parent)
        {
            this.parent = parent;
         
        }
        public void Do(Team team, Mission mission)
        {
            this.team = team;
            this.mission = mission;

            if (this.team.members.Count != 0)
            {
                int last = SessionState.GetInt("CharacterEditor_LastCharacterIndex", 0);

                if (last > team.members.Count-1)
                {
                     last = 0;
                }
                CharacterEditor newEditor = EditorWindow.CreateInstance<CharacterEditor>();
                newEditor.Do(team.members[last], mission,mission,false);
            }
        }

        public override bool Render()
        {
            if (this.team == null) return false;

            bool delete = false;
            bool close = false;
            EditorGUILayout.ScrollViewScope scrollScope = new EditorGUILayout.ScrollViewScope(scroll);
            using (scrollScope)
            {
                scroll = scrollScope.scrollPosition;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(20)))
                {
                    Undo.RecordObject(mission, "Remove Team");
                    mission.Teams.Remove(this.team);
                    mission.RefreshCharacterDictionary();
                    close = true;
                }
                EditorGUILayout.LabelField("Team: " + mission.GetTeamIndex(this.team));
                if (GUILayout.Button("Close", EditorStyles.toolbarButton, GUILayout.Width(55))) close = true;

                EditorGUILayout.EndHorizontal();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                    string _name = EditorGUILayout.TextField("Team Name: ", team.TeamName);
                    Color tColor = EditorGUILayout.ColorField(team.TeamColor);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RegisterCompleteObjectUndo(mission, "Modify Team");
                        team.TeamName = _name;
                        team.TeamColor = tColor;
                    }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
 
                EditorGUILayout.LabelField("Members", EditorStyles.miniBoldLabel);

                List<Character> characters = team.members;

                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    Undo.RecordObject(mission, "Add Character");

                    GenericMenu CreateMenu = new GenericMenu();
                    CreateMenu.AddItem(new GUIContent("Add new"),false,CreateNewCharacter);
                        
                    foreach (Character chara in EditorTools.CharaDatabase.commonCharacters)
                    {
                    CreateMenu.AddItem(new GUIContent("Common/" + chara._Name), false, GetCharacter, chara);
                    }
                    foreach (Character chara in EditorTools.CharaDatabase.uniqueCharacters)
                    {
                    CreateMenu.AddItem(new GUIContent("Unique/" + chara._Name), false, GetCharacter, chara);
                    }

                    CreateMenu.ShowAsContext();
                    




                }

                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel++;


                for (int q = 0; q < characters.Count; q++)
                {
                    Character chara = characters[q];
                    var v = new EditorGUILayout.HorizontalScope();
                    using (v)
                    {
                        GUILayout.Space(20 * EditorGUI.indentLevel);
                        if (GUILayout.Button(chara._Name, EditorStyles.label,GUILayout.Width(150)))
                        {
                            CharacterEditor newEditor = EditorWindow.CreateInstance<CharacterEditor>();
                            newEditor.Do(chara, mission, mission,false);
                            SessionState.SetInt("CharacterEditor_LastCharacterIndex", q);

                            if (this.parent != null)
                            {
                                this.parent.auxWindow = newEditor;
                                newEditor.SetParent(this.parent);

                            }
                            else
                            {
                                newEditor.Show();
                            }
                        }

                        if (this.parent != null)
                        {
                            if (GUILayout.Button("Pop", EditorStyles.miniButtonLeft))
                            {
                                CharacterEditor newEditor = EditorWindow.CreateInstance<CharacterEditor>();
                                newEditor.Do(chara, mission, mission,false);
                                newEditor.Show();
                                parent.auxWindow = null;
                                this.parent = null;
                            }
                        }
                        if (GUILayout.Button("X", EditorStyles.miniButtonRight))
                        {
                            Undo.RecordObject(mission, "Remove Character");
                            characters.Remove(chara);
                            mission.attatchedObjects.Remove(chara.SpawnCondition);
                            string path = AssetDatabase.GetAssetPath(chara.SpawnCondition);
                            AssetDatabase.DeleteAsset(path);
                            SessionState.EraseInt("CharacterEditor_LastCharacterIndex");
                        }

                    }
                    if (Event.current.button == 1 && v.rect.Contains(Event.current.mousePosition))
                    {
                        CharacterContextMenu(v.rect, chara);
                    }
                    //  EditorGUILayout.EndHorizontal();

                }
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
      

            return delete || close;
        }

        public void CharacterContextMenu(Rect rect, Character chara)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Copy ID to Clipboard"), false, IDtoClipboard, chara.id);
            menu.ShowAsContext();
        }

        public void GetCharacter(object chara)
        {
            Character newChara = EditorTools.CharaDatabase.GetCharacter((Character)chara);
            if (Selection.activeGameObject)
            {
                if (Selection.activeGameObject.GetComponent<GameTile>())
                {
                    GridPosition spawnPos = EditorTools.CurrentInspectedGrid.FindTile(Selection.activeGameObject.GetComponent<GameTile>());
                    newChara.SpawnPositon = spawnPos;
                }
            }
            team.members.Add(newChara);
        }

        public void CreateNewCharacter()
        {
            Character newChara = new Character("Character #" + (team.members.Count + 1));
            if (Selection.activeGameObject)
            {
                if (Selection.activeGameObject.GetComponent<GameTile>())
                {
                    GridPosition spawnPos = EditorTools.CurrentInspectedGrid.FindTile(Selection.activeGameObject.GetComponent<GameTile>());
                    newChara.SpawnPositon = spawnPos;
                }
            }
            team.members.Add(newChara);
        }

        private void IDtoClipboard(object id)
        {
            EditorGUIUtility.systemCopyBuffer = id as string;
        }

        public void OnGUI()
        {
    
            if(Render())
             this.Close();
        }

    }

}
