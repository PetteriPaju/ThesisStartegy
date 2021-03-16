using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SengokuWarrior
{
    [CreateAssetMenu(menuName = "Tactics/LoadableDatabse", fileName = "LoadableDatabase")]

    public class LoadableDatabase : ScriptableObject
    {
        public List<Loadable> loadables = new List<Loadable>();


        public Loadable GetLoadableById(int id) {

            for (int i =0; i<loadables.Count; i++)
            {
                if (loadables[i])
                {
                    if (loadables[i].ID == id)
                    {
                        return loadables[i];
                    }
                }
            }

            Debug.LogError("No object with id: " + id.ToString() + "was found! Please ensure that the object is a part of LoadableDatabsae!");
            return null;
        }

    }
}
