using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SengokuWarrior
{
    public class AStar
    {

        class Node
        {
            public GridPosition pos;
            public float G;
            public float H;
            public float F;
            public Node parent;

            public Node(GridPosition pos, float G, float F, float H, Node parent)
            {
                this.pos = pos;
                this.G = G;
                this.H = H;
                this.F = F;
                this.parent = parent;
            }
        }

        List<Node> openList;
        List<Node> closeList;
        List<Node> neighbours;
        List<Node> finalPath;
        Node start;
        Node end;
        public GameGrid grid;

        public AStar()
        {
            openList = new List<Node>();
            closeList = new List<Node>();
            neighbours = new List<Node>();
            finalPath = new List<Node>();
        }


        public static AStar GetPath(GridPosition from, GridPosition to, GameGrid grid, List<GridPosition> range)
        {
            AStar pathfinder = new AStar();
            pathfinder.grid = grid;
            pathfinder.FindPath(from, to, 0, false, range, grid);
           
            return pathfinder;
        }

        public void FindPath(GridPosition startCell, GridPosition goalCell, int layer, bool targetCellMustBeFree, List<GridPosition> range, GameGrid grid)
        {
            if (grid.GetTile(goalCell) == null) return;
            start = new Node(startCell, 0, 0, 0, null);
            end = new Node(goalCell, 0, 0, 0, null);
            openList.Add(start);
            bool keepSearching = true;
            bool pathExists = true;

            while ((keepSearching) && (pathExists))
            {
                Node currentNode = ExtractBestNodeFromOpenList();
                if (currentNode == null)
                {
                    pathExists = false;
                    break;
                }
                closeList.Add(currentNode);
                if (NodeIsGoal(currentNode))
                    keepSearching = false;
                else {
                    if (targetCellMustBeFree)
                        FindValidFourNeighbours(currentNode);
                    else
                        FindValidFourNeighboursIgnoreTargetCell(currentNode);


                    if (range != null)
                    {
                        Node[] tempList = neighbours.ToArray();
                        foreach (Node neighbour in tempList)
                        {
                            if (range.Find(find => find.Equals(neighbour.pos)) == null)
                            {
                                neighbours.Remove(neighbour);
                            }

                        }

                    }

                    foreach (Node neighbour in neighbours)
                    {
                        if (FindInCloseList(neighbour) != null)
                            continue;
                        Node inOpenList = FindInOpenList(neighbour);
                        if (inOpenList == null)
                        {
                            openList.Add(neighbour);
                        }
                        else {
                            if (neighbour.G < inOpenList.G)
                            {
                                inOpenList.G = neighbour.G;
                                inOpenList.F = inOpenList.G + inOpenList.H;
                                inOpenList.parent = currentNode;
                            }
                        }
                    }
                }
            }

            if (pathExists)
            {
                Node n = FindInCloseList(end);
                while (n != null)
                {
                    finalPath.Add(n);
                    n = n.parent;
                }
            }
        }


        public List<GridPosition> CellsFromPath()
        {
            List<GridPosition> path = new List<GridPosition>();
            foreach (Node n in finalPath)
            {
                path.Add(n.pos);
            }

            if (path.Count != 0)
            {
                path.Reverse();
                path.RemoveAt(0);
            }
            return path;
        }

        Node ExtractBestNodeFromOpenList()
        {
            float minF = float.MaxValue;
            Node bestOne = null;
            foreach (Node n in openList)
            {
                if (n.F < minF)
                {
                    minF = n.F;
                    bestOne = n;
                }
            }
            if (bestOne != null)
                openList.Remove(bestOne);
            return bestOne;
        }

        bool NodeIsGoal(Node node)
        {
            return ((node.pos.x == end.pos.x) && (node.pos.y == end.pos.y));
        }

        void FindValidFourNeighbours(Node n)
        {
            neighbours.Clear();

            if (grid.isWalkable(n.pos + new GridPosition(0, -1, 0)))
            {

                int jump = grid.GetJumpAmount(n.pos + new GridPosition(0, -1, 0));

                Node vn = PrepareNewNodeFrom(n, 0, -1, jump);
                neighbours.Add(vn);
            }
            if (grid.isWalkable(n.pos + new GridPosition(0, 1, 0)))
            {
                int jump = grid.GetJumpAmount(n.pos + new GridPosition(0, 1, 0));
                Node vn = PrepareNewNodeFrom(n, 0, +1, jump);
                neighbours.Add(vn);
            }
            if (grid.isWalkable(n.pos + new GridPosition(0, 0, -1)))
            {
                int jump = grid.GetJumpAmount(n.pos + new GridPosition(0, 0, -1));

                Node vn = PrepareNewNodeFrom(n, -1, 0, jump);
                neighbours.Add(vn);
            }
            if (grid.isWalkable(n.pos - new GridPosition(0, 0, 1)))
            {
                int jump = grid.GetJumpAmount(n.pos + new GridPosition(0, 0, 1));

                Node vn = PrepareNewNodeFrom(n, 1, 0, jump);
                neighbours.Add(vn);
            }
        }

        /* Last cell does not need to be walkable in the farm game */
        void FindValidFourNeighboursIgnoreTargetCell(Node n)
        {
            neighbours.Clear();
            if (grid.isWalkable(n.pos + new GridPosition(0, -1, 0)) || (n.pos + new GridPosition(0, -1, 0)).Equals(end))
            {
                int jump = grid.GetJumpAmount(n.pos + new GridPosition(0, -1, 0));
                Node vn = PrepareNewNodeFrom(n, 0, -1, jump);
                neighbours.Add(vn);
            }
            if (grid.isWalkable(n.pos + new GridPosition(0, 1, 0)) || (n.pos + new GridPosition(0, 1, 0)).Equals(end))
            {
                int jump = grid.GetJumpAmount(n.pos + new GridPosition(0, 1, 0));
                Node vn = PrepareNewNodeFrom(n, 0, +1, jump);
                neighbours.Add(vn);
            }
            if (grid.isWalkable(n.pos + new GridPosition(0, 0, -1)) || (n.pos + new GridPosition(0, 0, -1)).Equals(end))
            {
                int jump = grid.GetJumpAmount(n.pos + new GridPosition(0, 0, -1));
                Node vn = PrepareNewNodeFrom(n, -1, 0, jump);
                neighbours.Add(vn);
            }
            if (grid.isWalkable(n.pos + new GridPosition(0, 0, 1)) || (n.pos + new GridPosition(0, 0, 1)).Equals(end))
            {
                int jump = grid.GetJumpAmount(n.pos + new GridPosition(0, 0, 1));
                Node vn = PrepareNewNodeFrom(n, 1, 0, jump);
                neighbours.Add(vn);
            }
        }

        Node PrepareNewNodeFrom(Node n, int x, int y, int z)
        {
            GridPosition newpos = new GridPosition(z, y, x);
            Node newNode = new Node(n.pos + newpos, 0, 0, 0, n);
            newNode.G = n.G + MovementCost(n, newNode);
            newNode.H = Heuristic(newNode);
            newNode.F = newNode.G + newNode.H;
            newNode.parent = n;
            return newNode;
        }

        float Heuristic(Node n)
        {
            return Mathf.Sqrt((n.pos.x - end.pos.x) * (n.pos.x - end.pos.x) + (n.pos.y - end.pos.y) * (n.pos.y - end.pos.y) + (n.pos.z - end.pos.z) * (n.pos.z - end.pos.z));
        }

        float MovementCost(Node a, Node b)
        {
            int cost = 1;
            if (a.parent != null)
            {
                if (a.parent.pos.x == a.pos.x)
                {
                    if (b.pos.x != a.parent.pos.x)
                    {


                        cost += 2;
                    }
                }
                else if (a.parent.pos.y == a.pos.y)
                {
                    if (b.pos.y != a.parent.pos.y)
                    {

                        cost += 2;
                    }
                }
            }
            if (a.pos.z != b.pos.z) cost += 2;


            return cost;
            //  return map [b.x, b.y].MovementCost ();
        }

        Node FindInCloseList(Node n)
        {
            foreach (Node nn in closeList)
            {
                if ((nn.pos.x == n.pos.x) && (nn.pos.y == n.pos.y) && (nn.pos.z == n.pos.z))
                    return nn;
            }
            return null;
        }

        Node FindInOpenList(Node n)
        {
            foreach (Node nn in openList)
            {
                if ((nn.pos.x == n.pos.x) && (nn.pos.y == n.pos.y) && (nn.pos.z == n.pos.z))
                    return nn;
            }
            return null;
        }
    }
}