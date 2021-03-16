using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace SengokuWarrior
{
    public class RangeTester : EditorWindow
    {
        [SerializeField]
        private static List<GameTile> tiles = new List<GameTile>();
        private int Hrange = 5;
        private int Vrange = 5;

        private static GameTile currentSelectedTile = null;
        private static GridPosition currentPos = null;
    
     

        [MenuItem("Game Tools/RangeTester")]
        public static void Open()
        {

            GetWindow<RangeTester>();

         
        }

        void OnEnable()
        {
            ClearPrevious();
        }
        void Awake()
        {
            ClearPrevious();
            OnSelectionChange();
         
        }
        void OnDestroy()
            {
            ClearPrevious();
            }
        void OnDisable()
        {
            ClearPrevious();
        }

        void OnSelectionChange()
        {
            if (Selection.activeGameObject == null) return;

            GameTile activeSelectedTile = Selection.activeGameObject.GetComponent<GameTile>();

            if (activeSelectedTile == null) return;

            if (activeSelectedTile != currentSelectedTile)
            {
                ClearPrevious();
                currentSelectedTile = activeSelectedTile;
                currentPos = EditorTools.CurrentInspectedGrid.FindTile(currentSelectedTile);
                tiles = EditorTools.CurrentInspectedGrid.drawcircle(currentPos.x, currentPos.y, currentPos.z, Hrange, Vrange);
               
                SetLayerColor();
            }
            this.Repaint();
        }

        void OnGUI()
        {
            if (currentSelectedTile != null)
            {
                EditorGUI.BeginChangeCheck();
                Hrange = EditorGUILayout.IntSlider("Horizontal Range", Hrange, 0, 10);
                Vrange = EditorGUILayout.IntSlider("Vertical Range", Vrange, 0, 10);

                if (EditorGUI.EndChangeCheck())
                {
                    ClearPrevious();
                    tiles =  EditorTools.CurrentInspectedGrid.drawcircle(currentPos.x, currentPos.y, currentPos.z, Hrange,Vrange);
                    SetLayerColor();
                }
                

            }
            else
            {
                EditorGUILayout.LabelField("Please Select GameTile!", EditorStyles.helpBox);
            }


        }
        void SetLayerColor()
        {
            if (tiles.Count != 0)
            {
                foreach (GameTile tile in tiles)
                {
                    tile.Srenderer.color = Color.blue;
                }
            }
        }


        List<GameTile> GetTilesFromPosition(List<GridPosition> posses)
        {
            List<GameTile> selectedTiles = new List<GameTile>();
            foreach (GridPosition tile in posses)
            {

                selectedTiles.Add(EditorTools.CurrentInspectedGrid.GetTile(tile));
            }

            return selectedTiles;
        }

        void ClearPrevious()
        {
            if (tiles.Count != 0)
            {
                foreach (GameTile tile in tiles)
                {
                    if (tile != null)
                    tile.Srenderer.color = tile.DefaultColor;
                }
            }
            tiles.Clear();
        }

        void SetTileAt(GridPosition pos)
        {
            this.Repaint();
            this.Focus();
            if (currentSelectedTile != null)
            {
                ClearPrevious();
            }

            currentSelectedTile = EditorTools.CurrentInspectedGrid.GetTile(pos);
            currentPos = pos;

            tiles = EditorTools.CurrentInspectedGrid.drawcircle(currentPos.x, currentPos.y, currentPos.z, Hrange, Vrange);
            SetLayerColor();
        }

    }
}
