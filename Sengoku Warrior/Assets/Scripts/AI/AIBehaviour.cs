using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior {
    public abstract class AIBehaviour : ScriptableObject {

        public abstract void Think(AIHandlerer handlerer, GameGrid currentGrid);
       
}
}
