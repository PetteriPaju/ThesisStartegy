using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior
{
    public class CameraFollow : MonoBehaviour
    {
        bool followPlayer = true;

        //  public GameObject player;       //Public variable to store a reference to the player game object

        private Vector3 velocity = Vector3.zero;
        public Vector3 offset;         //Private variable to store the offset distance between the player and camera
        public static Transform target;
        public static CameraFollow _intance;
        // Use this for initialization
        void Start()
        {

            if (_intance != null)
            {
                GameObject.Destroy(_intance);
            }
            _intance = this;

            //Calculate and store the offset value by getting the distance between the player's position and camera's position.
            if (target ==null)
            StartCoroutine(WaitForPlayer());
            else
            {
                transform.position = target.transform.position + offset;
            }
        }


        void Update()
        {
            if (Input.GetButtonDown("ToggleCamera"))
            {

            }
        }


       
        public void MoveInstant() {
            if (target != null)
            {
                transform.position = target.transform.position + offset;
            }
        }
        private IEnumerator WaitForPlayer()
        {
            yield return new WaitUntil(() => target != null);
            transform.position = target.transform.position + offset;
        }

        // LateUpdate is called after Update each frame
        void LateUpdate()
        {
            // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
     if (target != null && followPlayer)
          
            transform.position = Vector3.SmoothDamp(transform.position, target.transform.position + offset, ref velocity, 0.3f);
        }
    }
}
