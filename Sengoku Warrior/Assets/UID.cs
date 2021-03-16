using System;
using UnityEngine;

namespace SengokuWarrior
{
    [System.Serializable]
    public class UID
    {
        [SerializeField]
        private int id;
        public UID()
        {
            this.id = System.Guid.NewGuid().GetHashCode();
        }
        public UID(int id)
        {
            this.id = id;
        }
        public int ToInt()
        {
            return id;
        }

        public static implicit operator int(UID a)
        {
            return a.id; 
        }
    }

}
