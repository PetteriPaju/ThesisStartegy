using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior {

    [CreateAssetMenu(fileName = "TileTempalteDatabase", menuName = "Create TileTempalteDatabase", order = 1)]
    public class TileTemplateDatabase : ScriptableObject {


        private string[] names;

        public string[] GetNames()
        {
            if (name.Length == tiles.Count) return names;

            List<string> _names = new List<string>();

            foreach (GameTileSerialized ts in tiles)
            {
                _names.Add(ts.Tittle);
            }

            names = _names.ToArray();

            return names;
        }

        public List<GameTileSerialized> tiles = new List<GameTileSerialized>();

        


    }
    [System.Serializable]
    public class GameTileSerialized
    {

        public string Tittle = "TileStyle";
        public Vector3 centerPoint;
        public AudioClip[] stepSounds;
        public AudioClip jumpAudio;
        public bool isWalkable = true;


        public GameTileSerialized(GameTile tile, string name)
        {
            this.Tittle = name;
            this.centerPoint = tile.centerPoint.transform.localPosition;
            this.stepSounds = tile.stepSounds;
            this.jumpAudio = tile.jumpAudio;
            this.isWalkable = tile.isWalkable;
        }
    }
}
