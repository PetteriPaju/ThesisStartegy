using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace SengokuWarrior
{
    [CustomEditor(typeof(GameGrid))]

    public class GridEditor : Editor
    {

        private static GUISkin skini;
        private static GridPosition pos = new GridPosition();
        public static Vector3 verticalOffset = new Vector3(0, 0.1f, 0);

        public static GridPosition Selected = new GridPosition();
        public static bool ShowAll = false;
        public static GameTile selectedTile = null;
        private static Sprite LastSprite;
        private static TileTemplateDatabase database;
        private static string[] databaseNames;
        private int currentTilestyle = 0;
        private static Vector2 overlayScrollPosition = Vector2.zero;
        private static bool clickUpListener = false;

        private static Editor tileEditor;

        public void OnEnable()
        {
            GameGrid t = target as GameGrid;

            if (t == null)
                return;

            database = (TileTemplateDatabase)AssetDatabase.LoadAssetAtPath("Assets/TileTempalteDatabase.asset", typeof(TileTemplateDatabase));

            SetLayerOpacity();


            if (EditorPrefs.HasKey("selectedX"))
            {
                Selected.x = EditorPrefs.GetInt("selectedX");
                Selected.y = EditorPrefs.GetInt("selectedY");
                Selected.z = EditorPrefs.GetInt("selectedZ");


                if (!t.isInBounds(new GridPosition(Selected.z, Selected.y, Selected.z)))
                {
                    Selected.x = 0;
                    Selected.y = 0;
                    Selected.z = 0;

                    EditorPrefs.SetInt("selectedX", Selected.x);
                    EditorPrefs.SetInt("selectedY", Selected.y);
                    EditorPrefs.SetInt("selectedZ", Selected.z);
                }

            }

            if (EditorPrefs.HasKey("selectedX"))
            {
                ShowAll = EditorPrefs.GetBool("ShowAll");
            }


        }

        public void SetLayerOpacity()
        {
            return;
            if (ShowAll) return;
            GameGrid t = target as GameGrid;

            if (t == null)
                return;

            for (int i = 0; i < t.layers.Count; i++)
            {
                if (i == Selected.z)
                {
                    t.layers[i].ReturnNormalOpacity();
                }
                else
                {
                    t.layers[i].ChangeOpacity(.55f);
                }
            }
        }
        public void ShowAllLayers()
        {
            GameGrid t = target as GameGrid;

            if (t == null)
                return;

            for (int i = 0; i < t.layers.Count; i++)
            {

                t.layers[i].ReturnNormalOpacity();

            }
        }
        Rect buttonRect;
        public override void OnInspectorGUI()
        {
            if (skini == null) skini = EditorGUIUtility.Load("Navigation.guiskin") as GUISkin;
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("P", EditorStyles.miniButtonRight, GUILayout.Width(20)))
            {
                var asm = Assembly.GetAssembly(typeof(EditorWindow));
                var T = asm.GetType("UnityEditor.PreferencesWindow");
                var M = T.GetMethod("ShowPreferencesWindow", BindingFlags.NonPublic | BindingFlags.Static);
                M.Invoke(null, null);
            }
            EditorGUILayout.EndHorizontal();
            DrawDefaultInspector();

            // get the chosen game object
            GameGrid t = target as GameGrid;

            if (t == null)
                return;

            if (database == null) { database = (TileTemplateDatabase)AssetDatabase.LoadAssetAtPath("Assets/TileTempalteDatabase.asset", typeof(TileTemplateDatabase)); }

            if (t.layers.Count == 0) t.Initialize();

            EditorGUI.BeginChangeCheck();
            currentTilestyle = EditorGUILayout.Popup("Templates", currentTilestyle, database.GetNames());
            if (EditorGUI.EndChangeCheck())
            {
                if (selectedTile)
                {
                    selectedTile.SetData(database.tiles[currentTilestyle]);
                }
            }
            if (GUILayout.Button("New Style") && selectedTile != null)
            {
                PopupWindow.Show(buttonRect, new TemplateCreatorPopUp(database, selectedTile));
            }
            if (Event.current.type == EventType.Repaint) buttonRect = GUILayoutUtility.GetLastRect();

            t.GridSortingWeights[0] = EditorGUILayout.IntField("Column", t.GridSortingWeights[0]);
            t.GridSortingWeights[1] = EditorGUILayout.IntField("Row", t.GridSortingWeights[1]);
            t.GridSortingWeights[2] = EditorGUILayout.IntField("Layer", t.GridSortingWeights[2]);


            EditorGUI.BeginChangeCheck();
            Navigation(t);

            if (selectedTile != t.layers[Selected.z].Rows[Selected.y].Tiles[Selected.x])
            {
                selectedTile = t.layers[Selected.z].Rows[Selected.y].Tiles[Selected.x];
                if (selectedTile != null)
                {
                    tileEditor = Editor.CreateEditor(selectedTile);
                    ((GameTileEditor)tileEditor).gridEditorMode = true;
                }
            }



            if (selectedTile != null)
            {
                if (tileEditor == null)
                {
                    tileEditor = Editor.CreateEditor(selectedTile);
                    ((GameTileEditor)tileEditor).gridEditorMode = true;
                }

                if (tileEditor != null && selectedTile != null)
                {
                    tileEditor.OnInspectorGUI();
                }

            }
            else
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Sprite", GUILayout.Width(38));
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    t.CreateBlock(Selected.y, Selected.x, Selected.z);
                    if (LastSprite != null) t.layers[Selected.z].Rows[Selected.y].Tiles[Selected.x].Srenderer.sprite = LastSprite;
                    else LastSprite = t.layers[Selected.z].Rows[Selected.y].Tiles[Selected.x].Srenderer.sprite;

                    if (database.tiles.Count != 0)
                    {
                        t.layers[Selected.z].Rows[Selected.y].Tiles[Selected.x].SetData(database.tiles[currentTilestyle]);
                    }
                }

                EditorGUILayout.EndHorizontal();

            }


            EditorGUILayout.Space();
            if (GUILayout.Button("New Layer"))
            {
                t.CreateEmptyLayer(t.layers.Count);
                Selected.z = t.layers.Count - 1;
                SceneView.RepaintAll();
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt("selectedX", Selected.x);
                EditorPrefs.SetInt("selectedY", Selected.y);
                EditorPrefs.SetInt("selectedZ", Selected.z);
            }
            if (GUILayout.Button("Fill"))
            {
                t.FillLayer(Selected.z);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(t.gameObject.scene);

            }
            if (GUILayout.Button("Reset Grid"))
            {
                t.ResetGrid();
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
            }
            if (GUI.changed && !Application.isPlaying)
            {
                EditorUtility.SetDirty(t.gameObject);
                Debug.Log("SAved");
            }
            WasdListener(t);
        }

        void Move(int dir, int amount, GameGrid t)
        {
            int[] dirs = new int[] { t.rows - 1, t.columns - 1, t.layers.Count - 1 };

            if (dir == 0)
            {
                Selected.x += amount;
                Selected.x = FitToGrid(Selected.x, dirs[dir]);
            }
            else if (dir == 1)
            {
                Selected.y += amount;
                Selected.y = FitToGrid(Selected.y, dirs[dir]);
            }
            else if (dir == 2)
            {
                Selected.z += amount;
                Selected.z = FitToGrid(Selected.z, dirs[dir]);
                SetLayerOpacity();
            }

            SceneView.RepaintAll();
            this.Repaint();
        }
        private int FitToGrid(int currentValue, int max)
        {
            if (currentValue < 0) currentValue = max;
            if (currentValue > max) currentValue = 0;

            return currentValue;
        }


        void MouseListener(GameGrid t)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            Event e = Event.current;

            if (e.type == EventType.MouseDown && Event.current.button == 0 && Event.current.modifiers == EventModifiers.None)
            {

                clickUpListener = false;
                if (SceneView.currentDrawingSceneView.position.Contains(e.mousePosition))
                {

                    Camera camera = SceneView.currentDrawingSceneView.camera;
                    Vector3 mouseposition = Event.current.mousePosition;
                    mouseposition = new Vector2(mouseposition.x, mouseposition.y);

                    GameObject selected = HandleUtility.PickGameObject(mouseposition, false);
                    if (selected != null)
                    {
                        GameTile tile = selected.GetComponent<GameTile>();
                        if (tile == null)
                        {
                            tile = selected.GetComponentInParent<GameTile>();
                        }




                        if (tile)
                        {
                            GUIUtility.hotControl = controlId;
                            GridPosition pos = t.FindTile(tile);
                            Selected.z = pos.z;
                            Selected.y = pos.y;
                            Selected.x = pos.x;
                            // Don't forget to use the event
                            Event.current.Use();
                            clickUpListener = true;
                            this.Repaint();
                        }
                        else
                        {
                        }

                    }
                }
            }
            else if (e.type == EventType.MouseUp && Event.current.button == 0 && Event.current.modifiers == EventModifiers.None)
            {
                if (clickUpListener)
                {
                    GUIUtility.hotControl = controlId;
                    clickUpListener = false;
                    Event.current.Use();
                }
            }
            else
            {
                clickUpListener = false;
            }


        }
        void WasdListener(GameGrid t)
        {
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.KeyDown:
                    {
                        if (Event.current.keyCode == (KeyCode.A) || Event.current.keyCode == KeyCode.LeftArrow)
                        {
                            Move(1, -1, t);
                        }

                        if (Event.current.keyCode == (KeyCode.D) || Event.current.keyCode == KeyCode.RightArrow)
                        {
                            Move(1, 1, t);
                        }

                        if (Event.current.keyCode == (KeyCode.W) || Event.current.keyCode == KeyCode.UpArrow)
                        {
                            if (Event.current.shift) Move(2, 1, t);
                            else
                                Move(0, 1, t);

                        }

                        if (Event.current.keyCode == (KeyCode.S) || Event.current.keyCode == KeyCode.DownArrow)
                        {
                            if (Event.current.shift) Move(2, -1, t);
                            else
                                Move(0, -1, t);
                        }


                        if (Event.current.keyCode == (KeyCode.Backspace) || Event.current.keyCode == KeyCode.Delete)
                        {
                            t.DestroyBlock(Selected.y, Selected.x, Selected.z);
                        }

                        e.Use();
                        break;
                    }

            }
        }


        void Navigation(GameGrid t)
        {
            //return;
        

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical(GUILayout.Width(50));


            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("", skini.customStyles[0], GUILayout.Width(50), GUILayout.Height(50))) Move(1, -1, t);
            if (GUILayout.Button("", skini.customStyles[1], GUILayout.Width(50), GUILayout.Height(50))) Move(0, 1, t);
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();


            if (GUILayout.Button("", skini.customStyles[3], GUILayout.Width(50), GUILayout.Height(50))) Move(0, -1, t);
            if (GUILayout.Button("", skini.customStyles[2], GUILayout.Width(50), GUILayout.Height(50))) Move(1, 1, t);



            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUILayout.Width(50));
            if (GUILayout.Button("^ Layer", EditorStyles.miniButton, GUILayout.Width(50), GUILayout.Height(50))) Move(2, 1, t);
            if (GUILayout.Button("v Layer", EditorStyles.miniButton, GUILayout.Width(50), GUILayout.Height(50))) Move(2, -1, t);
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();





        }
        void OnSceneGUI()
        {
            if (skini == null) skini = EditorGUIUtility.Load("Navigation.guiskin") as GUISkin;
            // get the chosen game object
            GameGrid t = target as GameGrid;

            if (t == null || t.layers.Count == 0)
                return;



            Color orgColor = Handles.color;
            Handles.color = PreferenceWindow.GridColor;
            // grab the center of the parent
            Vector3 center = Selected.z * GameGrid.BlockDirections.UP + t.transform.position - GameGrid.BlockDirections.SE + verticalOffset;

            //Outline
            Handles.DrawLine(center + Vector3.zero, center + GameGrid.BlockDirections.SE * t.rows);
            Handles.DrawLine(center + Vector3.zero, center + GameGrid.BlockDirections.NE * t.columns);

            Handles.DrawLine(center + GameGrid.BlockDirections.SE * t.rows, center + GameGrid.BlockDirections.SE * t.rows + GameGrid.BlockDirections.NE * t.columns);
            Handles.DrawLine(center + GameGrid.BlockDirections.NE * t.columns, center + GameGrid.BlockDirections.NE * t.columns + GameGrid.BlockDirections.SE * t.rows);

            center += GameGrid.BlockDirections.Down;
            //Outline
            Handles.DrawLine(center + Vector3.zero, center + GameGrid.BlockDirections.SE * t.rows);
            Handles.DrawLine(center + GameGrid.BlockDirections.SE * t.rows, center + GameGrid.BlockDirections.SE * t.rows + GameGrid.BlockDirections.NE * t.columns);
            center += GameGrid.BlockDirections.UP;

            //SW outline
            for (int r = 0; r < t.rows; r++)
            {
                Vector3 offset = GameGrid.BlockDirections.SE * r;
                Handles.DrawLine(center + offset + Vector3.zero, offset + center + GameGrid.BlockDirections.Down);
            }

            //SE outline
            for (int c = 0; c < t.columns; c++)
            {
                Vector3 SS_edge = Selected.z * GameGrid.BlockDirections.UP + t.transform.position - GameGrid.BlockDirections.SE + verticalOffset + GameGrid.BlockDirections.SE * t.rows;
                Vector3 offset = GameGrid.BlockDirections.NE * c;
                Handles.DrawLine(SS_edge + offset + Vector3.zero, offset + SS_edge + GameGrid.BlockDirections.Down);
            }

            //Ground
            for (int r = 0; r < t.rows; r++)
            {
                Vector3 offset = GameGrid.BlockDirections.SE * r;
                Handles.DrawLine(center + offset + Vector3.zero, offset + center + GameGrid.BlockDirections.NE * t.columns);


            }
            for (int c = 0; c < t.columns; c++)
            {
                Vector3 offset = GameGrid.BlockDirections.NE * c;
                Handles.DrawLine(center + offset + Vector3.zero, offset + center + GameGrid.BlockDirections.SE * t.rows);
            }


            orgColor = Handles.color;
            Handles.color = Color.red;


            //Selecter object
            Handles.color = PreferenceWindow.CapColor;
            Vector3 temCenter = center;


            Vector3 nw = temCenter + Selected.x * GameGrid.BlockDirections.NE + Selected.y * GameGrid.BlockDirections.SE;
            Vector3 sw = temCenter + GameGrid.BlockDirections.SE * (Selected.y + 1) + GameGrid.BlockDirections.NE * (Selected.x);
            Vector3 ne = temCenter + GameGrid.BlockDirections.NE * (Selected.x + 1) + GameGrid.BlockDirections.SE * (Selected.y);
            Vector3 se = temCenter + GameGrid.BlockDirections.SE * (Selected.y + 1) + GameGrid.BlockDirections.NE * (Selected.x + 1);

            Vector3[] rectPints = new Vector3[] { nw, sw, se, ne };


            Handles.DrawSolidRectangleWithOutline(rectPints, PreferenceWindow.CapColor, Color.red);
            Handles.color = orgColor;
            WasdListener(t);
            MouseListener(t);
            GUILayout.BeginArea(new Rect(10, 10, 200, 200));
            Navigation(t);
            GUILayout.EndArea();

        }
    }



    public class TemplateCreatorPopUp : PopupWindowContent
    {

        string _name = "name";

        TileTemplateDatabase database;

        GameTile tile;

        public TemplateCreatorPopUp(TileTemplateDatabase database, GameTile targetTile)
        {
            this.tile = targetTile;
            this.database = database;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(200, 150);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Input name", EditorStyles.boldLabel);
            _name = EditorGUILayout.TextField(_name);

            if (GUILayout.Button("Save"))
            {
                database.tiles.Add(new GameTileSerialized(tile, _name));

                EditorUtility.SetDirty(database);
                AssetDatabase.SaveAssets();

                this.OnClose();
            }

        }

        public override void OnOpen()
        {

        }

        public override void OnClose()
        {

        }
    }
}
