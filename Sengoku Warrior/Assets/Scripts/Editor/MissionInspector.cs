using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace SengokuWarrior
{
    [CustomEditor(typeof(Mission))]
    public class MissionInspector : Editor
    {
        private static Color[] teamColors = new Color[] { Color.blue, Color.green, Color.red, Color.yellow, Color.magenta };

        private static MissionInspector currentEditor;
        [SerializeField]
        public IEmbedable auxWindow;

        private MissionCondition.ConditionType[] ConditionTypes = new MissionCondition.ConditionType[] { MissionCondition.ConditionType.Custom, MissionCondition.ConditionType.Custom };
        private int[] selectedCharacterIndex = new int[] { 0, 0 };
        [SerializeField]
        private Vector3 selectedCharacterDragPosition = Vector3.zero;
        public static bool characterSelected = false;

        private bool[] ShowdCondition = new bool[] { false, false };
        private Dictionary<MissionEvent, MissionEventEditor> eventEditors = new Dictionary<MissionEvent, MissionEventEditor>();


        void Awake()
        {
            currentEditor = this;
        }
        void RemoveAssets()
        {
            foreach (Object obj in ((Mission)target).attatchedObjects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                AssetDatabase.DeleteAsset(path);
            }
        }

        void UpdateDictionaries()
        {
            Mission selectedMission = target as Mission;
            List<MissionEvent> allevents = new List<MissionEvent>();
            for (int i = 0; i < selectedMission.events.Count; i++)
            {
                allevents.AddRange(selectedMission.events[i].allEvents);
            }

            for (int i = 0; i < allevents.Count; i++)
            {
                if (!eventEditors.ContainsKey(allevents[i]))
                {
                    MissionEventEditor edit = MissionEventEditor.Create(allevents[i], selectedMission);
                    eventEditors.Add(allevents[i], edit);
                }
            }
        }
        void OnEnable()
        {
            if (!target) return;
            OnSceneGUIHelper.Register(OnSceneGUI);
            Mission selectedMission = target as Mission;
            List<Team> teams = selectedMission.Teams;
            int last = SessionState.GetInt("MissionEditor_LastTeamIndex", 0);
            if (teams.Count == 0 && last < teams.Count) return;

            if (auxWindow == null)
            {
                TeamEditor teamEdit = Editor.CreateInstance<TeamEditor>();
                if (teams.Count > SessionState.GetInt("MissionEditor_LastTeamIndex", 0))
                {
                    teamEdit.Do(teams[SessionState.GetInt("MissionEditor_LastTeamIndex", 0)], selectedMission);
                    teamEdit.SetParent(this);
                    auxWindow = teamEdit;
                }
            }
            MissionCondition.ConditionType a = selectedMission.WinDondition == null ? MissionCondition.ConditionType.Custom : selectedMission.WinDondition.conditionType;
            MissionCondition.ConditionType b = selectedMission.LoseCondition == null ? MissionCondition.ConditionType.Custom : selectedMission.LoseCondition.conditionType;

            ConditionTypes = new MissionCondition.ConditionType[] { a, b };
            UpdateDictionaries();
        }
        void OnDisable()
        {
            OnSceneGUIHelper.UnRegister(OnSceneGUI);
            AssetDatabase.SaveAssets();
        }


        public override void OnInspectorGUI()
        {

            Mission selectedMission = target as Mission;

            if (auxWindow == null)
            {
                TeamEditor teamEdit = Editor.CreateInstance<TeamEditor>();
                if (selectedMission.Teams.Count > SessionState.GetInt("MissionEditor_LastTeamIndex", 0))
                {
                    teamEdit.Do(selectedMission.Teams[SessionState.GetInt("MissionEditor_LastTeamIndex", 0)], selectedMission);
                    teamEdit.SetParent(this);
                    auxWindow = teamEdit;
                }
            }

            EditorGUI.BeginChangeCheck();
            Loadable next = EditorGUILayout.ObjectField("Next Mission", selectedMission.NextLoadable, typeof(Loadable), false) as Loadable;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedMission, "Modify Mission");
                selectedMission.NextLoadable = next;
            }
            List<Team> teams = selectedMission.Teams;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Teams", EditorStyles.boldLabel, GUILayout.Width(50));

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20)))
            {
                Undo.RecordObject(selectedMission, "Add Team");
                teams.Add(new Team());
            }
            EditorGUILayout.EndHorizontal();
            int i = 0;
            for (i = 0; i < teams.Count; i++)
            {
                int _i = i;
                Team team = teams[_i];


                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                if (GUILayout.Button("Team: " + _i + "    Members: " + team.members.Count.ToString(), EditorStyles.miniButtonLeft))
                {
                    TeamEditor teamEdit = Editor.CreateInstance<TeamEditor>();
                    teamEdit.Do(team, selectedMission);
                    teamEdit.SetParent(this);
                    auxWindow = teamEdit;
                    SessionState.SetInt("MissionEditor_LastTeamIndex", _i);

                }
                if (GUILayout.Button("Pop", EditorStyles.miniButtonRight, GUILayout.Width(35)))
                {
                    TeamEditor teamEdit = Editor.CreateInstance<TeamEditor>();
                    teamEdit.Do(team, selectedMission);
                    teamEdit.Show();
                    auxWindow = null;
                }

                EditorGUILayout.EndHorizontal();


            }
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            EditorGUILayout.LabelField("End Conditions", EditorStyles.boldLabel);
            selectedMission.ConditionSelector("Victory", ref ShowdCondition[0], ref selectedMission.WinDondition, ref ConditionTypes[0]);
            selectedMission.ConditionSelector("Defeat", ref ShowdCondition[1], ref selectedMission.LoseCondition, ref ConditionTypes[1]);
            GUILayout.Space(10);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Intro Text", EditorStyles.boldLabel);

            string introText = EditorGUILayout.TextArea(selectedMission.MissionIntroText);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedMission, "Modify Mission");
                selectedMission.MissionIntroText = introText;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            if (GUILayout.Button("+"))
            {
                Undo.RegisterCompleteObjectUndo(selectedMission, "Add Event Group");
                selectedMission.events.Add(new MissionEventContainer());
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;

            for (int q = 0; q < selectedMission.events.Count; q++)
            {
                for (int z = 0; z < selectedMission.events[q].allEvents.Count; z++)
                {
                    if (!eventEditors.ContainsKey(selectedMission.events[q].allEvents[z]))
                    {
                        UpdateDictionaries();
                        break;
                    }
                }
            }

            int deleteindex = -1;
            for (int q = 0; q < selectedMission.events.Count; q++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                selectedMission.events[q].OnGUI(eventEditors, selectedMission);
                EditorGUILayout.EndVertical();
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    deleteindex = q;
                }

                EditorGUILayout.EndHorizontal();
            }
            if (deleteindex != -1)
            {
                MissionEventContainer cont = selectedMission.events[deleteindex];
                selectedMission.events.RemoveAt(deleteindex);

                if (cont.cond != null && AssetDatabase.GetAssetPath(cont.cond) == AssetDatabase.GetAssetPath(selectedMission))
                {
                    DestroyImmediate(cont.cond, true);
                }

            }
            EditorGUI.indentLevel--;

            GUILayout.Space(10);
            if (auxWindow != null)
            {
                if (auxWindow.Render())
                {
                    Editor.DestroyImmediate(auxWindow as EditorWindow);
                    auxWindow = null;
                }


            }

            if (GUI.changed && !Application.isPlaying && selectedMission != null)
            {
                EditorUtility.SetDirty(selectedMission);
            }
        }

        private Vector3[] CreateArrow(GameGrid.BlockDirections.FaceDirections facedir, Vector3 origin)
        {
            Vector3 a, b, c;
            switch (facedir)
            {
                case GameGrid.BlockDirections.FaceDirections.NE:

                    a = origin + GameGrid.BlockDirections.NW / 2;
                    b = origin + GameGrid.BlockDirections.SE / 2;
                    c = origin + GameGrid.BlockDirections.NE / 2;
                    return new Vector3[] { a, b, c, c };
               

                case GameGrid.BlockDirections.FaceDirections.NW:

                    a = origin + GameGrid.BlockDirections.SW / 2;
                    b = origin + GameGrid.BlockDirections.NE / 2;
                    c = origin + GameGrid.BlockDirections.NW / 2;
                    return new Vector3[] { a, b, c, c };
                


                case GameGrid.BlockDirections.FaceDirections.SE:

                    a = origin + GameGrid.BlockDirections.SW / 2;
                    b = origin + GameGrid.BlockDirections.NE / 2;
                    c = origin + GameGrid.BlockDirections.SE/2;
                    return new Vector3[] { a, b, c, c };


                case GameGrid.BlockDirections.FaceDirections.SW:

                    a = origin + GameGrid.BlockDirections.NW / 2;
                    b = origin + GameGrid.BlockDirections.SE / 2;
                    c = origin + GameGrid.BlockDirections.SW / 2;
                    return new Vector3[] { a, b, c, c };
            }

            return null;
        }

        void OnSceneGUI(SceneView sceneView)
        {
            if (EditorTools.CurrentInspectedGrid == null) return;
            Mission selectedMission = target as Mission;
            int i = 0;
            for (i = 0; i < selectedMission.Teams.Count; i++)
            {

                for (int q = 0; q < selectedMission.Teams[i].Members.Count; q++)
                {

                    Vector3 position = EditorTools.CurrentInspectedGrid.CenterPointOnGrid(selectedMission.Teams[i].Members[q].SpawnPositon);
                    Handles.DrawLine(position, position + Vector3.up);

                 

                    GameGrid.BlockDirections.FaceDirections facedir = selectedMission.Teams[i].Members[q].SpawnDirection;
               


                    Handles.DrawSolidRectangleWithOutline(CreateArrow(facedir, position), teamColors[i], Color.red);

                    // grab the center of the parent
                    Vector3 center = position + GameGrid.BlockDirections.NW / 2 + GameGrid.BlockDirections.SW / 2;
                    Handles.DrawLine(center + Vector3.zero, center + GameGrid.BlockDirections.SE);
                    Handles.DrawLine(center + Vector3.zero, center + GameGrid.BlockDirections.NE);


                    Handles.DrawLine(center + GameGrid.BlockDirections.SE, center + GameGrid.BlockDirections.SE + GameGrid.BlockDirections.NE);
                    Handles.DrawLine(center + GameGrid.BlockDirections.NE, center + GameGrid.BlockDirections.NE + GameGrid.BlockDirections.SE);

                    center += GameGrid.BlockDirections.Down;
                    /*
                    Handles.DrawLine(center + Vector3.zero, center + GameGrid.BlockDirections.SE);
                    Handles.DrawLine(center + Vector3.zero, center + GameGrid.BlockDirections.NE);

                    Handles.DrawLine(center + GameGrid.BlockDirections.SE, center + GameGrid.BlockDirections.SE + GameGrid.BlockDirections.NE);
                    Handles.DrawLine(center + GameGrid.BlockDirections.NE, center + GameGrid.BlockDirections.NE + GameGrid.BlockDirections.SE);

                    Handles.DrawLine(center, center + GameGrid.BlockDirections.UP);
                    Handles.DrawLine(center + GameGrid.BlockDirections.SE, center + GameGrid.BlockDirections.SE + GameGrid.BlockDirections.UP);

                    Handles.DrawLine(center + GameGrid.BlockDirections.SE + GameGrid.BlockDirections.NE, center + GameGrid.BlockDirections.SE + GameGrid.BlockDirections.NE + GameGrid.BlockDirections.UP);

                    */
                    if (selectedMission.Teams.Count < selectedCharacterIndex[0] || selectedMission.Teams[selectedCharacterIndex[0]].members.Count < selectedCharacterIndex[1])
                    {
                        selectedCharacterIndex[0] = 0;
                        selectedCharacterIndex[1] = 0;
                        characterSelected = false;

                    }
                    center = selectedMission.Teams[selectedCharacterIndex[0]].Members[selectedCharacterIndex[1]].SpawnPositon.z * GameGrid.BlockDirections.UP + EditorTools.CurrentInspectedGrid.transform.position - GameGrid.BlockDirections.SE + GridEditor.verticalOffset;
                    Handles.DrawLine(center + Vector3.zero, center + GameGrid.BlockDirections.SE * EditorTools.CurrentInspectedGrid.rows);
                    Handles.DrawLine(center + Vector3.zero, center + GameGrid.BlockDirections.NE * EditorTools.CurrentInspectedGrid.columns);

                    if (PreferenceWindow.ShowCrosshair)
                    {
                        Handles.DrawLine(center + GameGrid.BlockDirections.SE * EditorTools.CurrentInspectedGrid.rows / 2, center + GameGrid.BlockDirections.SE * EditorTools.CurrentInspectedGrid.rows / 2 + GameGrid.BlockDirections.NE * EditorTools.CurrentInspectedGrid.columns);
                        Handles.DrawLine(center + GameGrid.BlockDirections.NE * EditorTools.CurrentInspectedGrid.columns / 2, center + GameGrid.BlockDirections.NE * EditorTools.CurrentInspectedGrid.columns / 2 + GameGrid.BlockDirections.SE * EditorTools.CurrentInspectedGrid.rows);
                    }
                    Handles.DrawLine(center + GameGrid.BlockDirections.SE * EditorTools.CurrentInspectedGrid.rows, center + GameGrid.BlockDirections.SE * EditorTools.CurrentInspectedGrid.rows + GameGrid.BlockDirections.NE * EditorTools.CurrentInspectedGrid.columns);
                    Handles.DrawLine(center + GameGrid.BlockDirections.NE * EditorTools.CurrentInspectedGrid.columns, center + GameGrid.BlockDirections.NE * EditorTools.CurrentInspectedGrid.columns + GameGrid.BlockDirections.SE * EditorTools.CurrentInspectedGrid.rows);
                }
            }

            if (characterSelected && selectedMission.Teams.Count != 0)
            {

                if (ObjectHilighter.currentEditor == this)
                {
                    GameTile tile = EditorTools.CurrentInspectedGrid.GetTile(selectedMission.Teams[selectedCharacterIndex[0]].Members[selectedCharacterIndex[1]].SpawnPositon);
                    if (tile)
                    {
                        ObjectHilighter.Clear();
                        ObjectHilighter.Add(tile.GetComponent<SpriteRenderer>(), this);
                    }
                    else
                    {
                        ObjectHilighter.Clear();
                    }
                }

                Vector3 spawnPos = EditorTools.CurrentInspectedGrid.CenterPointOnGrid(selectedMission.Teams[selectedCharacterIndex[0]].Members[selectedCharacterIndex[1]].SpawnPositon);

                if (Event.current.type == EventType.MouseDrag)
                {

                    float closestDist = Mathf.Infinity;
                    int closestDir = 0;
                    Vector3[] positions = new Vector3[] { spawnPos, spawnPos + GameGrid.BlockDirections.NW, spawnPos + GameGrid.BlockDirections.NE, spawnPos + GameGrid.BlockDirections.SE, spawnPos + GameGrid.BlockDirections.SW, spawnPos + GameGrid.BlockDirections.UP, spawnPos + GameGrid.BlockDirections.Down };


                    for (int c = 0; c < positions.Length; c++)
                    {

                        if (Vector3.Distance(selectedCharacterDragPosition, positions[c]) < closestDist)
                        {
                            closestDir = c;
                            closestDist = Vector3.Distance(selectedCharacterDragPosition, positions[c]);
                        }
                    }
                    if (Event.current.modifiers == EventModifiers.Control)
                    {
                        if (closestDir == 5 || closestDir == 6)
                        {
                            closestDir = 0;
                        }
                    }
                    if (closestDir != 0)
                    {

                        GridPosition future = selectedMission.Teams[selectedCharacterIndex[0]].Members[selectedCharacterIndex[1]].SpawnPositon.Clone() + InputHandlerer.GetMovement(closestDir);
                        if (EditorTools.CurrentInspectedGrid.isInBounds(future))
                        {
                            Undo.RecordObject(target, "Move");
                            selectedMission.Teams[selectedCharacterIndex[0]].Members[selectedCharacterIndex[1]].SpawnPositon += InputHandlerer.GetMovement(closestDir);
                        }
                    }

                }

                if (Event.current.type == EventType.MouseUp)
                {

                    selectedCharacterDragPosition = EditorTools.CurrentInspectedGrid.CenterPointOnGrid(selectedMission.Teams[selectedCharacterIndex[0]].Members[selectedCharacterIndex[1]].SpawnPositon);
                    sceneView.Repaint();
                }

                selectedCharacterDragPosition = Handles.PositionHandle(selectedCharacterDragPosition, Quaternion.Euler(-60, 0, 45));

            }


            for (i = 0; i < selectedMission.Teams.Count; i++)
            {

                for (int q = 0; q < selectedMission.Teams[i].Members.Count; q++)
                {
                    Vector3 position = EditorTools.CurrentInspectedGrid.CenterPointOnGrid(selectedMission.Teams[i].Members[q].SpawnPositon);
                    Vector3 buttonPosition = position + Vector3.up;
                    float size = 0.25f;
                    float pickSize = size * 2f;

                    Vector2 screenPos = HandleUtility.WorldToGUIPoint(buttonPosition);
                    Rect areaRect = new Rect(screenPos.x - 120 / 2, screenPos.y, 120, 50);
                    GUILayout.BeginArea(areaRect);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(selectedMission.Teams[i].Members[q]._Name, EditorStyles.miniButtonMid))
                    {
                        SessionState.SetInt("MissionEditor_LastTeamIndex", i);
                        SessionState.SetInt("CharacterEditor_LastCharacterIndex", q);
                        CharacterEditor newEditor = EditorWindow.CreateInstance<CharacterEditor>();
                        newEditor.Do(selectedMission.Teams[i].Members[q], selectedMission, selectedMission, false);
                        newEditor.SetParent(this);
                        auxWindow = newEditor;
                        EditorWindow.FocusWindowIfItsOpen<MissionEditor>();

                        selectedCharacterIndex = new int[] { i, q };
                        characterSelected = true;
                        selectedCharacterDragPosition = position;
                        ObjectHilighter.Clear();
                        ObjectHilighter.currentEditor = this;
                        Selection.activeGameObject = null;
                    }

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    GUILayout.EndArea();


                }
            }
            Handles.Label( Vector3.up, "", EditorStyles.boldLabel);
        }

    }
}
