using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SengokuWarrior
{
  public abstract class TurnAction
    {

        protected TurnAction secondary;
        protected Character activator;

        public virtual void Begin()
        {

        }
        
        public virtual void Execute()
        {

        }

        public virtual void Cancel()
        {
            GameManager.SetControlMode(ControlsMode.Disabled);
            TurnManager.currentCharacter.OnPathUpdate = null;
            TurnManager.currentCharacter.OnPathComplete = null;
            CameraFollow.target = TurnManager.currentCharacter.body.transform;
            Target.CurrentPosition = TurnManager.currentCharacter.Position;
            TurnManager.currentCharacter.anim.SetBool("move", false);
            RangeDisplayer._Main.Clear();
            RangeDisplayer._Secondary.Clear();
            Target.isOutside = false;
            Target.Clear();
            BattleUIContainer.RefreshUnitView(activator);
            TurnManager.performingAction = false;
        }

        public virtual void Update(GridPosition pos)
        {
            if (secondary != null) secondary.Update(pos);
        }

        public virtual void End()
        {
            TurnManager._instance.ActionOVer();
        }

        public virtual void Pause()
        {

        }

        public virtual void Resume()
        {
            secondary = null;
        }

        protected void ViewControls(GridPosition pos, bool rangeDisplayControls = true)
        {
            if (CameraFollow.target != Target.Indicator.transform) CameraFollow.target = Target.Indicator.transform;

            Target.Move(pos, Color.blue, true);

            if (rangeDisplayControls)
                RangeDisplayControls();
        }

        protected void RangeDisplayControls()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Character chara = Character.GetCharacterAtTile(Target.CurrentPosition);

                if (chara != null && chara != TurnManager.currentCharacter)
                {

                    RangeDisplayer.RangeType rType = RangeDisplayer._Secondary.currentRangeType == RangeDisplayer.RangeType.Move ? RangeDisplayer.RangeType.Attack : RangeDisplayer.RangeType.Move;
                    int range = RangeDisplayer._Secondary.currentRangeType != RangeDisplayer.RangeType.Move ? chara.stats.movementPoints : chara.stats.movementPoints + chara.stats.attackRange;
                    RangeDisplayer._Main.SetShow(false);
                    RangeDisplayer._Secondary.DisplayRange(GameGrid.currentGrid.GetAllWalkableTileInRange(chara.Position, range, true), rType);

                }
                else
                {
                    RangeDisplayer._Main.SetShow(true);



                    RangeDisplayer._Secondary.SetShow(false);
                }
            }
        }


    }


    public class MoveAction  : TurnAction
    {

        private PathDrawer pathDraw;

        public static List<GridPosition> currentMovementRange;
        private int LastDirection = -1;

        public MoveAction(Character act)
        {
            activator = act;
        }


        public override void Begin()
        {
            base.Begin();

            Target.isOutside = false;
            activator.anim.Play("Idle", 0);

            pathDraw = new PathDrawer();
            pathDraw.Begin(activator.Position);
            Target.CurrentPosition = activator.Position;

            currentMovementRange = GameGrid.currentGrid.GetAllWalkableTileInRange(activator.Position, activator.stats.movementPoints, true);
            RangeDisplayer._Main.DisplayRange(currentMovementRange, RangeDisplayer.RangeType.Move);

            GameManager.SetControlMode(ControlsMode.MoveTarget);
            UIManager._instance.FocusMenuPanel(false);
            UIManager._instance.SetButtonsActive(new bool[] { false, false, false, false, true, true, true, true,false,false });

        }

        public override void Execute()
        {
            base.Execute();

            if (pathDraw.GetPath().Count < 1) return;
            activator.anim.Play("Walk", 0);
            RangeDisplayer._Main.Clear();
            RangeDisplayer._Secondary.Clear();
            UIManager._instance.ClearUnitList();
            activator.TurnActionsPreformed[1] = true;
            GameManager.SetControlMode(ControlsMode.Disabled);
            Target.Clear();
            TurnManager.performingAction = true;
            List<GridPosition> path = pathDraw.GetPath();

            if (Character.isOtherCharacterInTile(path[path.Count - 1], activator))
            {
                path.RemoveAt(path.Count - 1);
                pathDraw.RemoveLast();
                activator.OnPathComplete += new AttackAction(activator).Execute;
            }
            else
            {
                activator.OnPathComplete += Cancel;
                activator.OnPathComplete += End;
            }
            activator.OnPathUpdate += pathDraw.RemoveFirst;

            //  if (pathDraw != null) pathDraw.ClearPath();
            activator.body.StartCoroutine(activator.MoveAlongPath(path));
            UIManager._instance.SetButtonsActive(new bool[] { false, false, false, false, false, false, false, false, false });
        }


        public override void End()
        {
            base.End();
            activator.anim.Play("Idle", 0);
        }

        public override void Cancel()
        {
            base.Cancel();      
            if (pathDraw != null)pathDraw.Destroy();
        }


        public override void Update(GridPosition pos)
        {

            base.Update(pos);
            if (secondary != null) return;
         
            RangeDisplayControls();

            if (pos.Equals(Target.CurrentPosition)) return;

            int selecteddir = InputHandlerer.GetDirectionFromInput();

            if (pathDraw.GetPath().Count == 0) activator.SetAnimatorDirection(selecteddir);



            //Disable walking throught enemy
            if (pathDraw.GetPath().Count > 0)
            {
                if (Character.isOtherCharacterInTile(pathDraw.Last.Clone(), activator))
                {

                    if (pathDraw.GetPath().Count > 1)
                    {

                        if (!pathDraw.LastNode.Previous.Value.Equals(pos))
                        {
                            GameManager._intance.StartCoroutine(GameManager.BlockInput(GameManager.controlsMode));
                            return;
                        }
                    }
                }

            }


            //Are we inside of character movement range
            if (currentMovementRange.Exists(item => item.Equals(pos)))
            {
                //Did we enter the movement range?
                if (!Target.isOutside)
                {
                    Target.Clear();


                    bool characterInTile = Character.isCharacterInTile(pos);

                    if (characterInTile)
                    {
                        Character chara = Character.GetCharacterAtTile(pos);

                        if (chara != activator)
                        {
                            if (chara.TeamNumber != activator.TeamNumber && !activator.TurnActionsPreformed[0])
                            {
                                if (pathDraw.AddPath(pos))
                                {
                                    AudioManager._instance.PlayClip("Click_Standard_03");
                                    Target.Move(pos, Color.red);
                                    BattleUIContainer._instance.UnitView.SetDamagePreview(chara.stats,activator.stats);
                                    pathDraw.SetLastWaypointGraphic(Target.Indicator.gameObject);
                                    LastDirection = selecteddir;
                                
                                }


                            }

                        }
                        else
                        {
                            //Return to same tiel as character
                            pathDraw.Destroy();
                            Target.CurrentPosition = activator.Position;
                        }
                    }
                    else
                    {
                        //No characters in tile

                        if (pathDraw.AddPath(pos))
                        {
                            AudioManager._instance.PlayClip("Click_Standard_03");
                            Target.CurrentPosition = pos.Clone();
                            LastDirection = selecteddir;
                        }
                        BattleUIContainer.RefreshUnitView(activator);
                        BattleUIContainer._instance.UnitView.HideDamagePreview();
                    }
                }
                else
                {               
                    //If we return to walkarea from outside
 
                }

            }
            else
            {
                if (!Target.isOutside)
                {
                    AudioManager._instance.PlayClip("Click_Standard_03");
                    secondary = new ViewAction(this, currentMovementRange);
                    Target.Move(pos,Color.white);
                    TurnManager._instance.AddCancel(secondary.Cancel);
                }
                //Move outside walkable range
                Target.isOutside = true;
            }
            GameManager._intance.StartCoroutine(GameManager.BlockInput(GameManager.controlsMode));
        }

        public override void Resume()
        {
            base.Resume();
            GridPosition lastPoint;


            //If we cancelled via button
            if (currentMovementRange.Exists(item => item.Equals(Target.CurrentPosition)))
            {
                AStar pathfind = AStar.GetPath(pathDraw.GetPath()[0], Target.CurrentPosition, GameGrid.currentGrid, currentMovementRange);
                List<GridPosition> path = pathfind.CellsFromPath();
                path.Insert(0, pathDraw.GetPath()[0]);
                pathDraw.ShowPath(path);
                Target.isOutside = false;
                Target.Clear();
            }
            else {
                Target.isOutside = false;
                if (pathDraw.GetPath().Count != 0)
                {
                    lastPoint = pathDraw.Last;
                }
                else
                {
                    lastPoint = activator.Position;
                }

                Character chara = Character.GetCharacterAtTile(lastPoint);
                if (chara != null)
                {
                    Target.Move(lastPoint, Color.red);
                    pathDraw.SetLastWaypointGraphic(Target.Indicator.gameObject);
                }
                else
                {
                    Target.Move(lastPoint, Color.white, false);
                }

            }
           


            RangeDisplayer._Main.SetShow(true);
            RangeDisplayer._Secondary.SetShow(false);
            this.secondary = null;
      

        }

    }

    
    public class AttackAction : TurnAction
    {

    public static List<GridPosition> currentMovementRange;
   
        public AttackAction(Character act)
        {
            activator = act;
        }

        public override void Begin()
        {
            base.Begin();
            Target.isOutside = false;
            List<Character> allAliveMinusMe = activator.GetEnemiesAtRange(activator.stats.attackRange);
            allAliveMinusMe.Remove(activator);
            if (activator.aiBehaviour == null)
            {
                UIManager._instance.unitList.gameObject.SetActive(true);
                UIManager._instance.SetUnitList(allAliveMinusMe);
            }
            if (allAliveMinusMe.Count != 0)
            {
             
                BattleUIContainer._instance.UnitView.SetDamagePreview(allAliveMinusMe[0].stats, activator.stats);
                BattleUIContainer.RefreshUnitView(allAliveMinusMe[0]);
            }

            Target.SetTargetList(allAliveMinusMe);

            GameManager.SetControlMode(ControlsMode.AttackTarget);
            List<GridPosition> Range = GameGrid.currentGrid.GetAllWalkableTileInRange(activator.Position, activator.stats.attackRange, true);
            RangeDisplayer._Main.DisplayRange(Range, RangeDisplayer.RangeType.Attack);


            UIManager._instance.FocusMenuPanel(false);
            UIManager._instance.SetButtonsActive(new bool[] { false, false, false, false, true, true, false,false,false });

        }

        public override void Execute()
        {
            base.Execute();
            UIManager._instance.unitList.gameObject.SetActive(false);
            RangeDisplayer._Main.Clear();
            TurnManager.performingAction = true;
            UIManager._instance.ClearUnitList();
            activator.TurnActionsPreformed[0] = true;
            activator.body.StartCoroutine(AttackMotion());
        }

        private System.Collections.IEnumerator AttackMotion()
        {
            activator.anim.Play("Idle", 0);
            Character attackTarget = Character.GetCharacterAtTile(Target.CurrentPosition);
            if (attackTarget != null)
            {
                // attackTarget.TakeDamage(currentCharacter.stats.CalculatedAtt);

                activator.FaceTarget(Target.CurrentPosition);
                attackTarget.FaceTarget(activator.Position);

            }

          
            if (attackTarget != null)
            {
              
                activator.anim.Play("Attack", 0);
                activator.FaceTarget(Target.CurrentPosition);
                GameManager.SetControlMode(ControlsMode.Disabled);
                Target.Clear();
                UIManager._instance.SetButtonsActive(new bool[] { false, false, false, false, false, false, false, false, false,false });
                yield return new WaitForSeconds(0.30f);

                int sorting = 0;
                if (attackTarget.Position.y < activator.Position.y || attackTarget.Position.x > activator.Position.x)
                    sorting = attackTarget.body.sRenderer.sortingOrder +1;
                else
                    sorting = attackTarget.body.sRenderer.sortingOrder-1;

                
             
              
             
                yield return new WaitForSeconds(0.05f);
                attackTarget.RecieveAttack(new AttackMessage(-activator.stats.DamageTo(attackTarget.stats)));
                UIManager._instance.CreateWeaponEffect(sorting, attackTarget.body.transform.position + GameGrid.BlockDirections.UP / 2);
                AudioManager._instance.PlayClip("Starhit");
                yield return new WaitForSeconds(0.75f);
                TurnManager.performingAction = false;
                Cancel();
                End();
            }
            else
            {
                yield return new WaitForSeconds(0.75f);
                Debug.Log("No chaarcter on target");
                Debug.Log(Target.CurrentPosition);
                Cancel();
                End();
            }
        }

        public override void Update(GridPosition pos)
        {
            base.Update(pos);

            if (secondary != null) return;        
                if (pos.Equals(Target.CurrentPosition)) return;

                int selecteddir = InputHandlerer.GetDirectionFromInput();
            if (selecteddir == 1) {
                Target.PreviousTarget();
                BattleUIContainer._instance.UnitView.SetDamagePreview(Character.GetCharacterAtTile(Target.CurrentPosition).stats, activator.stats);
            }
            else if (selecteddir == 3) {
                Target.NextTarget();
                BattleUIContainer._instance.UnitView.SetDamagePreview(Character.GetCharacterAtTile(Target.CurrentPosition).stats, activator.stats);
            }
              


                GameManager._intance.StartCoroutine(GameManager.BlockInput(GameManager.controlsMode));       
        }

        public override void Cancel()
        {
            RangeDisplayer._Main.Clear();
            BattleUIContainer._instance.UnitView.HideDamagePreview();
            UIManager._instance.unitList.gameObject.SetActive(false);
            base.Cancel();
        }


    }

    public class ViewAction : TurnAction
    {

        private TurnAction lastAction;
        private List<GridPosition> activeRange;   
        public ViewAction(TurnAction lastAction, List<GridPosition> activeRange)
        {
            this.lastAction = lastAction;
            this.activeRange = activeRange;
        }

        public override void Update(GridPosition pos)
        {
            base.Update(pos);
            if (pos.Equals(Target.CurrentPosition)) return;
            RangeDisplayControls();
            ViewControls(pos, false);
            GameManager._intance.StartCoroutine(GameManager.BlockInput(GameManager.controlsMode));


     
            if (activeRange.Exists(item => item.Equals(pos)))
            {
                Cancel();
                TurnManager._instance.RemoveCancel(this.Cancel);
                Target.isOutside = false;
            }


        }

        public override void Begin()
        {
            Target.Indicator.SetShow(true);
            lastAction.Pause();
        }

        public override void Cancel()
        {
            Target.Indicator.SetShow(false);
            BattleUIContainer.RefreshUnitView(activator);
            lastAction.Resume();
        }

    }
}
