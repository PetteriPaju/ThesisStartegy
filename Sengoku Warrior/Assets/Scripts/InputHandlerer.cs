using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior
{

    public enum MovementDirections
    {
        NW = 1,
        NE = 2,
        SE = 3,
        SW = 4,
        UP = 5,
        DOWN = 6
    }
    public class InputHandlerer
    {
        public MovementDirections currentDirection = MovementDirections.UP;
        public bool isInputLocked = false;
        public bool isLockVertical = false;
        private int lastInput = 0;

        public int lockedCilmbDirection = 0;
        public int lockedFace = 0;


        public static int Reverse(int i)
        {
            if (i == 1) return 3;
            if (i == 3) return 1;
            if (i == 2) return 4;
            if (i == 4) return 2;
            Debug.LogWarning("Invalid direction given");
            return -1;
        }


        public static int GetDirectionFromInput()
        {
            Vector2 speed = new Vector2(0, 0);

            // Detect when control-keys are pressed or released and adjust the backside of character
            if (Input.GetAxisRaw("Horizontal") != 0) speed.x = Input.GetAxisRaw("Horizontal");
            if (Input.GetAxisRaw("Vertical") != 0) speed.y = Input.GetAxisRaw("Vertical");
            int selecteddir = 0;
            if (Mathf.Abs(speed.x) >= Mathf.Abs(speed.y)) selecteddir = speed.x > 0 ? 3 : 1;
            else if (Mathf.Abs(speed.x) <= Mathf.Abs(speed.y)) selecteddir = speed.y > 0 ? 2 : 4;

            return selecteddir;
        }

        public void UpdateInput(int input)
        {
        }
        public int WalkDirection(int input)
        {
            if (input == 1) return (int)MovementDirections.NW;
            else if (input == 2) return (int)MovementDirections.NE;
            else if (input == 3) return (int)MovementDirections.SE;
            else if (input == 4) return (int)MovementDirections.SW;

            return -1;
        }

        public void LockVerticalDirection(int input, int climdirection)
        {

            isInputLocked = true;
            lockedCilmbDirection = climdirection;
            lastInput = input;
            isLockVertical = true;

        }
      
        public static GridPosition GetMovement(int input)
        {
            GridPosition movementAmount = new GridPosition();



            switch (input)
            {
                case (int)MovementDirections.NW: movementAmount.y--; break;
                case (int)MovementDirections.NE: movementAmount.x++; break;
                case (int)MovementDirections.SE: movementAmount.y++; break;
                case (int)MovementDirections.SW: movementAmount.x--; break;
                case (int)MovementDirections.UP: movementAmount.z++; break;
                case (int)MovementDirections.DOWN: movementAmount.z--; break;
            }


            return movementAmount;
        }
        public static int GetSortingOrder(Character chara)
        {
            return GetSortingOrder(chara.Position);
        }
        public static int GetSortingOrder(GridPosition pos)
        {
            GridPosition tempPos = pos.Clone();
            tempPos.z++;
            return (tempPos.y * GameGrid.currentGrid.GridSortingWeights[1]) - tempPos.x * GameGrid.currentGrid.GridSortingWeights[0] + tempPos.z * GameGrid.currentGrid.GridSortingWeights[2];
        }
        public static int GetDirectionTo(GridPosition A, GridPosition B)
        {

            GridPosition diff = B - A;


            if (diff.x != 0)
            {
                return diff.x > 0 ? 2 : 4;
            }
            else
            {
                return diff.y > 0 ? 3 : 1;
            }

        }

    }
}
