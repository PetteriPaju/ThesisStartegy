using UnityEngine;

namespace SengokuWarrior
{
    public class Options : MonoBehaviour
    {

        public static Options _instance;

        public float CharacterMovementSpeed
        {
            get
            {
                return MovementSpeedNormal ? CharacterMovementSlow : CharacterMovementFast;
            }
        }

        public float CharacterMovementFast = 0.15f;
        public float CharacterMovementSlow = 0.25f;

        public bool MovementSpeedNormal = true;

        void Awake()
        {
            if (_instance != this) GameObject.Destroy(_instance);
            _instance = this;
        }

    }
}
