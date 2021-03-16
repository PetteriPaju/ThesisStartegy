using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace SengokuWarrior
{
    [CreateAssetMenu(fileName = "DefaultAI", menuName = "AI/CreateDefaultAI", order = 1)]
    public class DefaultAI : AIBehaviour
    {

        public override void Think(AIHandlerer handlerer, GameGrid currentGrid)
        {
            if (!handlerer.body.GetData().isAvailable()) return;

            if (handlerer.body.charadata.GetAllEnemies().Count == 0) return;
            
            Character target = handlerer.body.GetData();

            int attackRange = target.stats.attackRange;
            if (!target.TurnActionsPreformed[1])
            {
                attackRange += target.stats.movementPoints;
            }
            //Get all tiles that unit can walk to
            List<GridPosition> tilesInWalkRange = GameGrid.currentGrid.GetAllWalkableTileInRange(target.Position, target.stats.movementPoints, false);
            //Get all tile that are within units wal+attack range
            List<GridPosition> tilesInAttackRange = GameGrid.currentGrid.GetAllWalkableTileInRange(target.Position,  attackRange, true);
       
            //Get every unit that exist inside attack range
            List<Character> enemiesInsideAttackRange = target.EnemiesInsideRange(tilesInAttackRange);
            enemiesInsideAttackRange = Character.sortByPredictedDamage(target, enemiesInsideAttackRange);


            if (target.TurnActionsPreformed[0] && target.TurnActionsPreformed[1])
            {
                GameManager._intance.turnManager.EndTurnWait();
                return;
            }

            if (!target.TurnActionsPreformed[0] && !target.TurnActionsPreformed[1])
            {

                if (enemiesInsideAttackRange.Count != 0)
                {
                    Character found = null;
                    for (int i = 0; i < enemiesInsideAttackRange.Count; i++)
                    {
                        if (found != null) break;
                        foreach (GridPosition pos in tilesInAttackRange)
                        {
                            if (GameGrid.currentGrid.CanISeeTile(enemiesInsideAttackRange[i].Position, target.stats.attackRange, pos))
                            {
                                Debug.Log("Enemy was inside attackRange!");
                                found = enemiesInsideAttackRange[i];
                                break;
                            }
                        }

                    }


                    if (found != null)
                    {

                        if (target.TurnActionsPreformed[1])
                        {
                            Target.SetTarget(found);
                            AttackAction action = new AttackAction(target);

                            action.Execute();
                        }
                        else
                        {
                            List<GridPosition> PotentialWalktiles = new List<GridPosition>();
                            PotentialWalktiles.Add(target.Position);
                            //Get closest attackable tile!
                            if ((int)(Mathf.Abs(target.Position.x - found.Position.x)) > target.stats.attackRange || (int)(Mathf.Abs(target.Position.y - found.Position.y)) > target.stats.attackRange || (int)(Mathf.Abs(target.Position.z - found.Position.z)) > target.stats.attackRange)
                            {
                                PotentialWalktiles = GameGrid.currentGrid.GetAllWalkableTileInRange(found.Position, target.stats.attackRange, false);
                                PotentialWalktiles.Sort((a, b) => (target.Position.Distance(a).CompareTo(target.Position.Distance(b))));
                            }

                          //  Debug.Log("Attackking Enemy: " + found._Name);
                            AStar pathfinder = AStar.GetPath(target.Position, PotentialWalktiles[0], GameGrid.currentGrid, null);
                            Target.SetTarget(found);


                            List<GridPosition> path = pathfinder.CellsFromPath();

                            AttackAction action = new AttackAction(target);
                           

                            target.OnPathComplete +=action.Execute;

                          //  Debug.Log("Attack!!!");


                            //  if (pathDraw != null) pathDraw.ClearPath();
                            if (path.Count != 0)
                            {
                                target.TurnActionsPreformed[1] = true;
                                target.body.StartCoroutine(target.MoveAlongPath(path));
                            }
                            else target.OnPathComplete.Invoke();
                        }

               
                    }

                }
                else if (!target.TurnActionsPreformed[1] && target.onStartPath.Count == 0)
                {
           
                    List<Character> allenemies = target.GetAllEnemies();
          
                    List<GridPosition> closest = new List<GridPosition>();
                    bool found = false;
                    for (int i = 0; i < allenemies.Count; i++) {

                            AStar pathfinder = AStar.GetPath(target.Position, allenemies[i].Position, GameGrid.currentGrid,null);
                        if (pathfinder.CellsFromPath().Count < closest.Count || closest.Count == 0)
                        {
                            closest = pathfinder.CellsFromPath();
                            found = true;
                        }
                        
                    }
                    if (closest.Count > 0)
                    {
                        closest.RemoveAt(closest.Count - 1);
                    }


                    if (closest.Count > target.stats.movementPoints)
                    {
                        closest.RemoveRange(target.stats.movementPoints, closest.Count - target.stats.movementPoints);
                    }
                    target.OnPathComplete += GameManager._intance.turnManager.ActionOVer;
                    if (closest.Count != 0 && found)
                    {
                        target.body.StartCoroutine(target.MoveAlongPath(closest));
                        target.TurnActionsPreformed[1] = true;
                    }
                    else target.OnPathComplete.Invoke();
                }
                else
                {

                    if (target.onStartPath.Any()) {

                        List<List<GridPosition>> pathsToWaypoints = new List<List<GridPosition>>();

                        for (int i = 0 ; i< target.onStartPath.Count; i++)
                        {
                            AStar pathfinder = AStar.GetPath(target.Position, target.onStartPath[i], GameGrid.currentGrid,null);
                            pathsToWaypoints.Add(pathfinder.CellsFromPath());
                        }

                        pathsToWaypoints.Sort((a, b) => (a.Count.CompareTo(b.Count)));

                        Debug.Log(pathsToWaypoints.Count);
                        Debug.Log(target.onStartPath.Count);
                        if (!pathsToWaypoints[0].Last().Equals(target.onStartPath[0]))
                        {
                            int indexOfClosest = target.onStartPath.FindIndex(item => item.Equals(pathsToWaypoints[0].Last()));
                           target.onStartPath.RemoveRange(0, indexOfClosest);

                        }


                        if (pathsToWaypoints[0].Count > target.stats.movementPoints)
                        {
                            pathsToWaypoints[0].RemoveRange(target.stats.movementPoints, pathsToWaypoints[0].Count - target.stats.movementPoints);
                        }
                        else
                        {
                            int indexOfClosest = target.onStartPath.FindIndex(item => item.Equals(pathsToWaypoints[0].Last()));
                            target.onStartPath.RemoveAt(indexOfClosest);
                        }
                        target.OnPathComplete += GameManager._intance.turnManager.ActionOVer;
                        target.body.StartCoroutine(target.MoveAlongPath(pathsToWaypoints[0]));
                        target.TurnActionsPreformed[1] = true;


                    }
                    else
                    GameManager._intance.turnManager.EndTurnWait();
                }
            }
            else
            {
                GameManager._intance.turnManager.EndTurnWait();
            }

        }
    }
}
