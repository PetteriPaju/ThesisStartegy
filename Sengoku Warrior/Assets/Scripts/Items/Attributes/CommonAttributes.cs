using UnityEngine;

namespace SengokuWarrior
{
    public class CommonAttributes : ItemAttribute
    {
        public override int Priority() { return 0; }

        public string _name = "Game name";
        public Sprite sprite = null;
        public bool isStackable = false;
        public float value = 0;
        [TextArea(3, 10)]
        public string Description = "";


        public Sprite GetSprite()
        {
            if (sprite == null) return new Sprite();
            else return sprite;
        }
    }
}
