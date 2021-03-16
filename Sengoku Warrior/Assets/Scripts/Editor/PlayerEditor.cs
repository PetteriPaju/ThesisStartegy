using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SengokuWarrior
{
    /*
    [CustomEditor(typeof(PlayerBody))]
    class PlayerEditor : Editor
    {

        void OnSceneGUI()
        {
            // get the chosen game object
            PlayerBody t = target as PlayerBody;

            if (t == null || t.GetData() == null || !Application.isPlaying)
                return;


            // grab the center of the parent
            Vector3 center = -GameGrid.BlockDirections.UP*t.GetData().Position.z + t.GetData().FeetPosisionOnGrid(t.transform.position) - GameGrid.BlockDirections.SE + GameGrid.verticalOffset + t.GetData()._FeetOffset[t.GetData().Faceddirection];
            Handles.color = Color.red;


            Handles.DrawLine(center + Vector3.zero, center + GameGrid.BlockDirections.SE);

            Handles.color = Color.white;

        }
    }
    */

}