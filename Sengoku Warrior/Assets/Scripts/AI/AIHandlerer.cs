using UnityEngine;
namespace SengokuWarrior
{
    public class AIHandlerer : MonoBehaviour
    {

        [HideInInspector]public CharacterBody body;
        void Awake()
        {
            body = GetComponent<CharacterBody>();
        }

        public void Think()
        {
            if (body.GetData().aiBehaviour != null)
            body.GetData().aiBehaviour.Think(this, GameGrid.currentGrid);
        }

    }
}
