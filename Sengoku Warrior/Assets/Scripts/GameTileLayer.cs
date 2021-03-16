using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior
{
    [System.Serializable]
    public class GameTileLayer
    {

        public List<GameTileRow> Rows = new List<GameTileRow>();

        public GameTileLayer(int rows, int columns)
        {


            for (int i = 0; i < rows; i++)
            {
                Rows.Add(new GameTileRow());
                for (int c = 0; c < columns; c++)
                {
                    Rows[i].Tiles.Add(null);
                }

            }
        }
        public void ReturnNormalOpacity()
        {
            foreach (GameTileRow r in Rows)
            {
                foreach (GameTile gt in r.Tiles)
                {
                    if (gt != null)
                    {
                        gt.Srenderer.color = gt.DefaultColor;
                    }
                }
            }
        }
        public void ChangeOpacity(float NewOpacity)
        {
            foreach (GameTileRow r in Rows)
            {
                foreach (GameTile gt in r.Tiles)
                {
                    if (gt != null)
                    {
                        Color a = gt.DefaultColor;
                        a.a *= NewOpacity;
                        gt.Srenderer.color = a;
                    }
                }
            }
        }

    }


    [System.Serializable]
    public class GameTileRow
    {
        public List<GameTile> Tiles = new List<GameTile>();
    }
}