using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior
{
    [System.Serializable]
    public class Player : Character
    {

        public static Player _instance;

        public override void OnStart(CharacterBody body)
        {

            base.OnStart(body);

            _instance = this;

         //   body.StartCoroutine(MoveAlongPath(AStar.GetPath(Position, new GridPosition(1, 5, 5)).CellsFromPath));
           
        }

        public override void Die()
        {
            base.Die();

            GameManager._intance.EndGame(GameEndType.Lose);

            body.CancelInvoke();
            body.StopAllCoroutines();
        }

        public override void WalkingUpdate()
        {
            if (!isAlive) return;
            if (walkingPath) return;
            if (isMoving) return;

            int selecteddir = 0;
            if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
            {         
                //  body.transform.position += GameGrid.BlockDirections.NE * (Input.GetAxis("Vertical") + 0.5f*Input.GetAxisRaw("Vertical") ) / 15;
                //  body.transform.position += GameGrid.BlockDirections.SE * (Input.GetAxis("Horizontal") + 0.5f * Input.GetAxisRaw("Horizontal")) / 15;

                Vector2 speed = new Vector2(0, 0);
                int lastDir = anim.GetInteger("dir");

                // Detect when control-keys are pressed or released and adjust the backside of character
                if (Input.GetAxisRaw("Horizontal") != 0) speed.x = Input.GetAxisRaw("Horizontal");
                if (Input.GetAxisRaw("Vertical") != 0) speed.y = Input.GetAxisRaw("Vertical");

                if (Mathf.Abs(speed.x) > Mathf.Abs(speed.y)) selecteddir = speed.x > 0 ? 3 : 1;
                else if (Mathf.Abs(speed.x) < Mathf.Abs(speed.y)) selecteddir = speed.y > 0 ? 2 : 4;

                if (selecteddir == 0) selecteddir = Faceddirection;

                if (lastDir != selecteddir)
                {
                    Faceddirection = selecteddir;
                }

                GridPosition futurePosition = Position.Clone();

                anim.SetInteger("dir", (int)Faceddirection);
                
                if (selecteddir == 3) futurePosition.y++;
                else if (selecteddir == 1) futurePosition.y--;
                else if (selecteddir == 2) futurePosition.x++;
                else if (selecteddir == 4) futurePosition.x--;



                if (GameGrid.currentGrid.isInBounds(futurePosition))
                {


                    GridPosition selectedTile;
                    GameGrid.WalType canWalk = GameGrid.currentGrid.CanBeWalked(Position, futurePosition, out selectedTile);
                    body.StartCoroutine(MoveTo(selectedTile));

                }

           
                
                
      
                

                  

                }
            else
            {
                anim.SetBool("move", false);
            }
            inputeHandlerer.UpdateInput(selecteddir);

   
            if (Input.GetButtonDown("Skill"))
            {
            //    GameManager._intance.ActivateSkill(0, this);
            }



        }
    }

}