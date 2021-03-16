using UnityEngine;

namespace SengokuWarrior
{
   public interface ITargetable
    {
        Transform GetTransform();
        GridPosition getGridPosition();
        bool isAvailable();
    }
}
