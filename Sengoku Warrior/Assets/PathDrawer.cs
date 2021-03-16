using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace SengokuWarrior
{
     public class PathDrawer
    {
        private LinkedList<GridPosition> path;
        private List<GameObject> wayPoints = new List<GameObject>();
        
        public GameObject Indicator
        {
            get
            {
                if (wayPoints.Count != 0) return wayPoints[wayPoints.Count - 1];
                else return null;
            }
        }

        public GridPosition Last
        {
            get
            {
                return path.Last.Value;
            }
        }

        public LinkedListNode<GridPosition> LastNode
        {
            get
            {
                return path.Last;
            }
        }
        public void Begin(GridPosition startPos)
        {

            ClearPath();
            path = new LinkedList<GridPosition>();
        }

        public void ShowPath(List<GridPosition> posList)
        {
            ClearPath();
            path = new LinkedList<GridPosition>();

            for (int i = 0; i< posList.Count; i++)
            {
                path.AddLast(posList[i]);
            }

            DrawPath(true);
          
            }
        public TargetIndicator CreateIndicator(GridPosition pos, Color color)
        {

            TargetIndicator targerIndicator = UIManager._instance.CreateTargetIndicator(pos,color,true);
            return targerIndicator;
        }

        public void SetLastWaypointGraphic(GameObject go)
        {
            bool setCamera = false;
            if (CameraFollow.target == wayPoints[wayPoints.Count - 1])
            {
                setCamera = true;
            }

            GameObject.Destroy(wayPoints[wayPoints.Count - 1]);
            wayPoints[wayPoints.Count - 1] = go;

            if (setCamera)CameraFollow.target = go.transform;
       
        }

        public void InsertFirst(GridPosition pos)
        {
            if (!path.Contains(pos))
            {
                path.AddFirst(pos);
            }
        }
        public void RemoveFirst()
        {
            path.RemoveFirst();
            DrawPath(false);
        }
        public void RemoveLast()
        {
            path.RemoveLast();
            DrawPath(false);
        }
        public bool AddPath(GridPosition nextPos)
        {
            if (path.Count > 1)
            {
                if (nextPos.Equals(path.Last.Previous.Value))
                {
                    path.RemoveLast();
                    DrawPath();
                    return true;
                }
                else if (ContainPoint(nextPos))
                {
                    Debug.Log("Contain point");
                    return false;
                }
            }

                path.AddLast(nextPos);
                DrawPath();
                return true;

        }

        private bool ContainPoint(GridPosition pos)
        {
            var currentNode = path.First;
            while (currentNode != null)
            {

                if (currentNode.Value.Equals(pos)) return true;
                currentNode = currentNode.Next;
            }

            return false;
        }

        public List<GridPosition> GetPath()
        {
            List<GridPosition> lst = new List<GridPosition>();
            var currentNode = path.First;
            while (currentNode != null)
            {
                lst.Add(currentNode.Value);
                currentNode = currentNode.Next;

            }
            return lst;
        }

        public void DrawPath(bool cameraFollow = true)
        {
            ClearPath();
    
                var currentNode = path.First;
                while (currentNode != null){
                 
                  Vector3 tilePos =  GameGrid.currentGrid.GetTile(currentNode.Value).transform.position + GameGrid.BlockDirections.UP;

                    GameObject point = GameObject.Instantiate(UIManager._instance.WayPointMarker, tilePos, Quaternion.identity) as GameObject;
                    point.GetComponent<SpriteRenderer>().sortingOrder = GameGrid.currentGrid.GetTile(currentNode.Value).GetComponent<SpriteRenderer>().sortingOrder + 1;
                    wayPoints.Add(point);

                    if (currentNode == path.Last)
                    {
                        point.GetComponent<SpriteRenderer>().color = Color.red;
                        if(cameraFollow)
                        CameraFollow.target = point.transform;
                    }

                    currentNode = currentNode.Next;
                    
                }

            
        }

        public void ClearPath()
        {
            foreach(GameObject go in wayPoints)
            {
                GameObject.Destroy(go);
            }
            wayPoints.Clear();
        }
        public void Destroy()
        {
            ClearPath();
            path = new LinkedList<GridPosition>();
        }


    }
}
