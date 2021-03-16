using UnityEngine;
namespace SengokuWarrior
{
    public abstract class ItemAttribute : ScriptableObject
    {
        public virtual int Priority() { return 0; }
        public virtual float floatValue (){ return 0; }
        public virtual int intValue() { return 0; }



    }


}


