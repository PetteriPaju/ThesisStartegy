using System.Collections.Generic;
using UnityEngine;

namespace SengokuWarrior
{
    [System.Serializable]
    public class Loadable  : ScriptableObject
    {
        [System.NonSerialized]
        public static Loadable CurrentlyLoaded;
        public Loadable NextLoadable;
        [System.NonSerialized]
        public Loadable original;
        public UID ID;

        public string scenePath;

        public virtual void Init()
        {

        }
        public virtual void Begin()
        {

        }

        public virtual Loadable Clone()
        {
            return Loadable.Instantiate<Loadable>(this);
        }

        public virtual void End()
        {
            ScriptableObject.Destroy(this);
        }
        public virtual void LoadData(Save data)
        {
           
        }
        public virtual void LoadDefaultData()
        {

        }
        public virtual void SaveData()
        {

        }
    }
}
