using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace SengokuWarrior
{
    [CustomEditor(typeof(GameTile))]
    [CanEditMultipleObjects]
    public class GameTileEditor : Editor
    {
        private static Vector3 verticalOffset = new Vector3(0, 0.1f, 0);
        private static Sprite LastSprite;
        private static Vector2 overlayScrollPosition = Vector2.zero;
        public bool gridEditorMode = false;

        void OnEnable()
        {

        }


        public override void OnInspectorGUI()
        {    
                GameTile selectedTile = target as GameTile;
                if (selectedTile == null)
                    return;

                if (!gridEditorMode)
                {
                    if (GUILayout.Button("Find In Grid"))
                    {
                        GameGrid parentGrid = selectedTile.gameObject.GetComponentInParent<GameGrid>();

                        if (parentGrid == null) return;
                        GridPosition pos = parentGrid.FindTile(selectedTile);

                        EditorPrefs.SetInt("selectedX", pos.x);
                        EditorPrefs.SetInt("selectedY", pos.y);
                        EditorPrefs.SetInt("selectedZ", pos.z);


                        Selection.activeGameObject = parentGrid.gameObject;

                    }
                }
                if (selectedTile != null)
                {


                    EditorGUI.BeginChangeCheck();
                    //Tile + overlays
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(150));

                    //Main texture

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(60));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Sprite", GUILayout.Width(38));
                    bool deleteTile = GUILayout.Button("x", GUILayout.Width(20));

                    EditorGUILayout.EndHorizontal();

                Sprite currentSprite = EditorGUILayout.ObjectField(GUIContent.none, selectedTile.Srenderer.sprite, typeof(Sprite), false, GUILayout.Width(60)) as Sprite;
                if (EditorGUI.EndChangeCheck()) {
                    LastSprite = currentSprite;

                    foreach (GameTile tg in targets)
                    {
                        Undo.RecordObject(tg.Srenderer, "Change Sprite");
                        tg.Srenderer.sprite = LastSprite;
                    }
               
                }

                    GUILayout.Space(-7);
                    EditorGUI.BeginChangeCheck();
                   Color defColor = EditorGUILayout.ColorField(GUIContent.none, selectedTile.DefaultColor, false, true, false, new ColorPickerHDRConfig(1, 1, 1, 1), GUILayout.Width(60));
                    if (EditorGUI.EndChangeCheck())
                    {
                    foreach (GameTile tg in targets)
                    {
                        Undo.RecordObjects(new Object[] { tg.Srenderer, tg }, "Change Sprite");
                        tg.Srenderer.color = defColor;
                        tg.DefaultColor = defColor;

                    }
                  
                     }
            



                //end Main texture
                GUILayout.FlexibleSpace();
                    EditorGUILayout.EndVertical();
                bool layerCountMatch = true;
                for (int i = targets.Length - 1; i > 0; i--)
                {
                    if (((GameTile)targets[i]).overlays.Count != ((GameTile)targets[i - 1]).overlays.Count) layerCountMatch = false;
                }
                if (layerCountMatch)
                {
                    //Overlays
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.LabelField("Overlays");
                    if (GUILayout.Button("+", GUILayout.Width(20)))
                    {
                        Undo.RecordObjects(targets, "Change Sprite");
                        foreach (GameTile tg in targets)
                        {
                            tg.AddOverLay(tg);
                        }

                    }

                    EditorGUILayout.EndHorizontal();
                    overlayScrollPosition = EditorGUILayout.BeginScrollView(overlayScrollPosition);
                    EditorGUILayout.BeginHorizontal();

  
                    for (int i = 0; i < selectedTile.overlays.Count; i++)
                    {
                        EditorGUILayout.BeginVertical(GUILayout.Width(60));
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Sprite", GUILayout.Width(38));
                        bool deleteOverlay = GUILayout.Button("x");
                        EditorGUILayout.EndHorizontal();
                        selectedTile.overlays[i].sprite = EditorGUILayout.ObjectField(GUIContent.none, selectedTile.overlays[i].sprite, typeof(Sprite), false, GUILayout.Width(60)) as Sprite;

                        EditorGUILayout.EndVertical();

                        if (deleteOverlay)
                        {
                            GameObject.DestroyImmediate(selectedTile.overlays[i].gameObject);
                            selectedTile.overlays.RemoveAt(i);
                        }
                    }
       
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();



               
                }
                else
                {
                    EditorGUILayout.HelpBox("Overlay Count missmatch."+System.Environment.NewLine+"Multi Object editing supports only Tiles with same amount of overlays", MessageType.Warning,true);
                }
                //End tile + overlays
                EditorGUILayout.EndHorizontal();
                EditorGUI.BeginChangeCheck();
                    bool isWalkable = EditorGUILayout.Toggle("Walkable", selectedTile.isWalkable);
                    if (EditorGUI.EndChangeCheck())
                    {
                    foreach (GameTile tg in targets)
                    {
                        Undo.RecordObject(tg, "Change Sprite");
                        tg.isWalkable = isWalkable;
                    }
                }


                    if (deleteTile)
                    {
                    Undo.RecordObject(EditorTools.CurrentInspectedGrid, "Delete Tile");
                    foreach (GameTile tg in targets)
                    {
                        EditorTools.CurrentInspectedGrid.DestroyBlock(tg);
                    }
                        //    t.DestroyBlock(Selected.y, Selected.x, Selected.z);
                    }

                }

                if (!gridEditorMode && target != null)
                {
                    DrawDefaultInspector();
                }
            
        }


        void OnSceneGUI()
        {
            // get the chosen game object
            GameTile t = target as GameTile;

            if (t == null)
                return;


            // grab the center of the parent
            Vector3 center =  GameGrid.BlockDirections.UP + t.transform.position + GameGrid.BlockDirections.SW - verticalOffset;

            Handles.DrawLine(center + Vector3.zero, center + GameGrid.BlockDirections.SE);
            Handles.DrawLine(center + Vector3.zero, center + GameGrid.BlockDirections.NE);

         
            Handles.DrawLine(center + GameGrid.BlockDirections.SE , center + GameGrid.BlockDirections.SE  + GameGrid.BlockDirections.NE );
            Handles.DrawLine(center + GameGrid.BlockDirections.NE , center + GameGrid.BlockDirections.NE  + GameGrid.BlockDirections.SE );

            ObjectHilighter.Add(t.GetComponent<SpriteRenderer>(), this);
            /*
            center += GameGrid.BlockDirections.Down;

            Handles.DrawLine(center + Vector3.zero, center + GameGrid.BlockDirections.SE);
            Handles.DrawLine(center + Vector3.zero, center + GameGrid.BlockDirections.NE);

            Handles.DrawLine(center + GameGrid.BlockDirections.SE, center + GameGrid.BlockDirections.SE + GameGrid.BlockDirections.NE);
            Handles.DrawLine(center + GameGrid.BlockDirections.NE, center + GameGrid.BlockDirections.NE + GameGrid.BlockDirections.SE);

            Handles.DrawLine(center, center + GameGrid.BlockDirections.UP);
            Handles.DrawLine(center + GameGrid.BlockDirections.SE, center + GameGrid.BlockDirections.SE+GameGrid.BlockDirections.UP);

            Handles.DrawLine(center + GameGrid.BlockDirections.SE + GameGrid.BlockDirections.NE, center + GameGrid.BlockDirections.SE + GameGrid.BlockDirections.NE + GameGrid.BlockDirections.UP);
            // Handles.DrawLine(center + GameGrid.BlockDirections.NE, center + GameGrid.BlockDirections.NE+GameGrid.BlockDirections.UP);
            */

            Color orgColor = Handles.color;

            orgColor = Handles.color;
            Handles.color = Color.red;

  
            Handles.color = orgColor;
            center = GameGrid.BlockDirections.UP + t.transform.position + GameGrid.BlockDirections.SW + GameGrid.BlockDirections.SE/2 - verticalOffset + GameGrid.BlockDirections.NE/2;

            //Handles.DrawLine(center + Vector3.zero, center + GameGrid.BlockDirections.SE);


        }

    }
}
