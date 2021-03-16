using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace SengokuWarrior
{
    [ExecuteInEditMode]
    public class GameGrid : MonoBehaviour
    {

        public enum WalType
        {
            CanWalk,
            CannotWalk,
            JumpDown,
            JumUp
        }
 
        //   private Color[] DebubColors = new Color[] { Color.red, Color.blue, Color.cyan, Color.magenta, Color.yellow, Color.magenta, Color.green,  Color.yellow, Color.magenta, Color.green, Color.yellow, Color.magenta, Color.green}; 

        public Color TransparentColor = Color.white;
        public GameTile BlockPrefab;
        public GameObject OverlayPrefab;

        public int rows = 15;
        public int columns = 5;

        public int SortingLayerIDSpace = 5;
        public int[] GridSortingWeights = new int[3] { 60, 30, 26};

        public Material EditorSelectedMaterial;

       // public GridPosition AlarmTile;
        public GridPosition PlayerSpawn;

        public GameObject[,] grid = new GameObject[15, 5];
        public List<GameTileLayer> layers = new List<GameTileLayer>(1);

        private List<GameTile> VisibleTiles = new List<GameTile>();
        public static Vector3 ActorOffset = new Vector3(0, 0.785f, 0);
        public static Vector3 verticalOffset = new Vector3(0, 0.1f, 0);

        public static GameGrid currentGrid;

        public static class BlockDirections
        {
            public enum FaceDirections
            {
                NW,
                NE,
                SE,
                SW
            }

            public static Vector3 UP = new Vector3(0, 0.725f, 0);
            public static Vector3 Down = new Vector3(0, -0.725f, 0);
            public static Vector3 NW = new Vector3(-0.4965f, 0.2526f, 0);
            public static Vector3 NE = new Vector3(0.4965f, 0.2526f, 0);
            public static Vector3 SE = new Vector3(0.4965f, -0.2526f, 0);
            public static Vector3 SW = new Vector3(-0.4965f, -0.2526f, 0);

            public static Vector3 GetDirection(int dir)
            {
                    switch (dir)
                    {
                        case 1: return NW;
                        case 2: return NE;
                        case 3: return SE;
                        case 4: return SW;
                        case 5: return UP;
                        case 6: return Down;
                    }
                
                return Vector3.zero;

            }
        }

        public bool isInBounds(GridPosition pos)
        {

            return (pos.x >= 0 && pos.x < columns && pos.y >= 0 && pos.y < rows && pos.z >= 0 && pos.z < layers.Count);
           
        }

        public Vector3 CenterPointOnGrid(GridPosition pos)
        {

            GameTile tile = GetTile(pos);


            if (tile != null) return tile.CenterPoint;
            else
            {
                return gameObject.transform.position + (pos.x * BlockDirections.NE + pos.y * BlockDirections.SE + pos.z * BlockDirections.UP);
            }

        
           // if (tile) return tile.transform.position + GameGrid.BlockDirections.UP  - verticalOffset;
           // else return Vector3.zero;
        }
      
        public bool isWalkable(GridPosition pos)
        {

            if (!isInBounds(pos)) return false;
            if (HasTileOnTop(pos) && HasTileOnTop(pos + new GridPosition(1, 0, 0))) return false;
            if (GetTile(pos) == null && GetJumpAmount(pos) != -1) return false;
            if (GetTile(pos) != null && !GetTile(pos).isWalkable) return false;

            return true;
        }

        public List<GameTile> drawcircle(int x0, int y0, int z0, int radius, int vertical)
        {
    
            List<GameTile> tileInRange = new List<GameTile>();
            GridPosition checkpos = new GridPosition(z0, y0, x0);
            for (int z = z0 - vertical; z <= z0 + vertical; z++)
            {
                if (z < 0) continue;
                if (z > layers.Count - 1) continue;

                for (int y = -radius; y <= radius; y++)
                {
                    for (int x = -radius; x <= radius; x++)
                        if (x * x + y * y <= radius * radius)
                        {
                            if (DoLine(x0, y0, z0, x0 + x, y + y0, z0 + z))
                            {
                                AddToListIfNotNull<GameTile>(GetTile(new GridPosition(z, y + y0, x0 + x)), tileInRange);

                            }



                        }
                }
            }
            return tileInRange;

        }

        public List<GridPosition> GetPositiosnInrange(int x0, int y0, int z0, int radius, int vertical)
        {

            List<GridPosition> tileInRange = new List<GridPosition>();
            GridPosition checkpos = new GridPosition(z0, y0, x0);

            for (int z = z0 - vertical; z <= z0 + vertical; z++)
            {
                if (z < 0) continue;
                if (z > layers.Count - 1) continue;

                for (int y = -radius; y <= radius; y++)
                {
                    for (int x = -radius; x <= radius; x++)
                        if (x * x + y * y <= radius * radius)
                        {
                            if (DoLine(x0, y0, z0, x0 + x, y + y0, z0 + z))
                            {
                                AddToListIfNotNull<GridPosition>(new GridPosition(z, y + y0, x0 + x), tileInRange);

                            }
                        }
                }
            }
            return tileInRange;

        }
        public int GetJumpAmount(GridPosition pos)
        {
            if (HasTileOnTop(pos) && CanBeJumpedTo(1, pos)) return 1;

            if (GetTile(pos) == null)
            {
                if (GetHighestJumpablePosition(pos).z < pos.z)
                {
                    return GetHighestJumpablePosition(pos).z - pos.z;
                }
            }
            else return 0;

            return -1;
        }

        public GameTile GetTile(GridPosition pos)
        {
            if (!isInBounds(pos)) return null;
            return layers[pos.z].Rows[pos.y].Tiles[pos.x];
        }

        public GameTile GetTile(int x, int y, int z)
        {
            GridPosition pos = new GridPosition(z, y, x);

            if (!isInBounds(pos)) return null;
            return layers[pos.z].Rows[pos.y].Tiles[pos.x];
        }

        public bool CanISeeTile(GridPosition pos,int range, GridPosition tile, bool Debug= false)
        {
            List<GridPosition> allVisibleTiles = GetPositiosnInrange(pos.x, pos.y, pos.z, range, range);


            foreach (GridPosition position in allVisibleTiles)
            {
                if (position.Equals(tile))
                {
                    return true;
                }
            }
            return false;
        }

        public void SetFOV(GridPosition pos, GridPosition forward, int[] ranges)
        {
            List<GridPosition> tiles = GetVisibleTiles(pos, forward, 5);

            foreach (GridPosition position in tiles)
            {
                GameTile tl = GetTile(position);
                VisibleTiles.Add(tl);
            }

        }

        public GridPosition GetHighestTileBellow (GridPosition frompos)
        {
      
            if (!HasTilesVertical(frompos)) return null;

            GridPosition pos = frompos.Clone();
            for (int i = pos.z; pos.z >= 0; pos.z--)
            {
                if (GetTile(pos)) return pos;
            }


            return null;
        }


        public List<GameTile> toTiles(List<GridPosition> posses)
        {
            List<GameTile> tiles = new List<GameTile>();

            foreach(GridPosition pos in posses)
            {
                AddToListIfNotNull<GameTile>(GetTile(pos), tiles);
            }

            return tiles;
        }

        public List<GridPosition>GetAllWalkableTileInRange(GridPosition pos, int range, bool includeOccupied)
        {
            List<GridPosition> tiles = new List<GridPosition>();

            for (int z = pos.z - range; z <= pos.z + range; z++)
            {
                if (z < 0) continue;
                if (z > layers.Count - 1) continue;

                for (int y = -range; y <= range; y++)
                {
                    for (int x = -range; x <= range; x++)
                        if (x * x + y * y <= range * range)
                        {

                            GridPosition position = new GridPosition(z, y + pos.y, x + pos.x);
                            GameTile tile = GetTile(position);
                            if (tile == null) continue;
                            if (HasTileOnTop(position)) continue;                              
                          if(!includeOccupied && Character.isCharacterInTile(position)) continue;
                           tiles.Add(position);
                        }
                }
            }

            return tiles;
        }

        public List<GridPosition> GetVisibleTiles(GridPosition pos, GridPosition forward, int range, bool debug = false, bool playermode = false)
        {
            List<GridPosition> _visibleTiles = new List<GridPosition>();
            List<GridPosition> BannedTiles = new List<GridPosition>();

            GridPosition checkpos = pos.Clone();
            forward = new GridPosition(0, 0, 1);
            int faceRange = range;
            for (int up = 0; up < range + range + 1; up++)
            {
                for (int iforward = 0; iforward < faceRange + range + 1; iforward++)
                {

                    GridPosition tempPos = checkpos.Clone() +
                        forward.GetReversed() * range +

                        (forward * iforward) +

                        GridPosition.Left(forward) * range +

                        (GridPosition.UP * (range + 1)).GetReversed() +

                        GridPosition.UP * up;

                    if (tempPos.x == pos.x && forward.x != 0 && range != -1) { continue; }
                    if (tempPos.y == pos.y && forward.y != 0 && range != -1) { continue; }

                    for (int right = 0; right < range * 2 + 1; right++)
                    {

                        GridPosition Bpos = tempPos.Clone() + GridPosition.Left(forward).GetReversed() * right;
                        GameTile tile = GetTile(Bpos);
                        if (tile)
                        {
                            if (DoLine(checkpos.x, checkpos.y, checkpos.z, Bpos.x, Bpos.y, Bpos.z))
                            {
                                _visibleTiles.Add(Bpos);
                            }

                        }

                        if (HasTileOnTop(Bpos))
                        {

                            if (checkpos.x > Bpos.x && checkpos.y < Bpos.y)
                            {

                                int[,] positions = { { 0, 1, 0 }, { 0, 1, -1 }, { 0, 0, -1 } };
                          
                                for (int i = 0; i < positions.GetLength(0) ; i++)
                                {
                                    GridPosition newPosition = Bpos + new GridPosition(positions[i, 0], positions[i, 1], positions[i, 2]);
                                    if (GetTile(newPosition)) BannedTiles.Add(newPosition);
                                }

                            }
                            if (checkpos.x > Bpos.x && checkpos.y > Bpos.y)
                            {

                                int[,] positions = { { 0, -1, 0 }, { 0, -1, -1 }, { 0, 0, -1 } };

                                for (int i = 0; i < positions.GetLength(0) ; i++)
                                {

                                    GridPosition newPosition = Bpos + new GridPosition(positions[i, 0], positions[i, 1], positions[i, 2]);
                                    if (GetTile(newPosition)) BannedTiles.Add(newPosition);
                                }

                            }
                            if (checkpos.x < Bpos.x && checkpos.y < Bpos.y)
                            {
                                int[,] positions = { { 0, 1, 0 }, { 0, 1, 1 }, { 0, 0, 1 } };

                                for (int i = 0; i < positions.GetLength(0) ; i++)
                                {
                                    GridPosition newPosition = Bpos + new GridPosition(positions[i, 0], positions[i, 1], positions[i, 2]);
                                    if (GetTile(newPosition)) BannedTiles.Add(newPosition);
                                }

                            }
                            if (checkpos.x < Bpos.x && checkpos.y > Bpos.y)
                            {
                                int[,] positions = { { 0, -1, 0 }, { 0, -1, 1 }, { 0, 0, 1 } };

                                for (int i = 0; i < positions.GetLength(0) ; i++)
                                {
                                    GridPosition newPosition = Bpos + new GridPosition(positions[i, 0], positions[i, 1], positions[i, 2]);
                                    if (GetTile(newPosition)) BannedTiles.Add(newPosition);
                                }


                            }

                            if (InRange(checkpos, Bpos, 3))
                            {

                                if (checkpos.x == Bpos.x && checkpos.y < Bpos.y)
                                {
                                 
                                    int[,] positions = { { 0, 1, 0 }, { 0, 1, -1 }, { 0, 1, 1 }, { 0, 0, 1 }, { 0, 0, -1 } };

                                    for (int i = 0; i < positions.GetLength(0); i++)
                                    {

                                        GridPosition newPosition = Bpos + new GridPosition(positions[i, 0], positions[i, 1], positions[i, 2]);
                                        if (GetTile(newPosition)) BannedTiles.Add(newPosition);
                                    }

                                }
                                if (checkpos.x == Bpos.x && checkpos.y > Bpos.y)
                                {
                                    int[,] positions = { { 0, -1, 0 }, { 0, -1, 1 }, { 0, -1, -1 }, { 0, 0, -1 }, { 0, 0, 1 } };

                                    for (int i = 0; i < positions.GetLength(0) ; i++)
                                    {
                                        GridPosition newPosition = Bpos + new GridPosition(positions[i, 0], positions[i, 1], positions[i, 2]);
                                        if (GetTile(newPosition)) BannedTiles.Add(newPosition);
                                    }

                                }

                                if (checkpos.x > Bpos.x && checkpos.y == Bpos.y)
                                {
                                    int[,] positions = { { 0, -1, 0 }, { 0, -1, -1 }, { 0, 0, -1 }, { 0, 1, 0 }, { 0, 1, -1 } };

                                    for (int i = 0; i < positions.GetLength(0) ; i++)
                                    {
                                        GridPosition newPosition = Bpos + new GridPosition(positions[i, 0], positions[i, 1], positions[i, 2]);
                                        if (GetTile(newPosition)) BannedTiles.Add(newPosition);
                                    }

                                }
                                if (checkpos.x < Bpos.x && checkpos.y == Bpos.y)
                                {
                                    int[,] positions = { { 0, 1, 0 }, { 0, 1, 1 }, { 0, 0, 1 }, { 0, -1, 0 }, { 0, -1, 1 } };

                                    for (int i = 0; i < positions.GetLength(0) ; i++)
                                    {
                                        GridPosition newPosition = Bpos + new GridPosition(positions[i, 0], positions[i, 1], positions[i, 2]);
                                        if (GetTile(newPosition)) BannedTiles.Add(newPosition);
                                    }

                                }

                            }


                        }
                    }


                }

            }

            if (playermode)
            {
                List<GridPosition> adjancedPositions = GetAdjancedPositions(pos);


                foreach (GridPosition gp in adjancedPositions)
                {

                    if (GetTile(gp) == null)
                    {
                    _visibleTiles.Add(GetHighestTileBellow(gp));
                    }
                }


            }

            List<GridPosition> diagonals = GetDialognaldPositions(pos);

            foreach (GridPosition diag in diagonals)
            {

                if (HasTileOnTop(diag))
                {
                    List<GridPosition> corners = GetDialognaldPositions(diag);

                
                        foreach (GridPosition corner in corners)
                    {

                
                        if (GetTile(corner) != null)
                        {
                       
                        }
                  
                        BannedTiles.Add(corner);


                        do
                        {
                            _visibleTiles.Remove(corner);
                        } while (_visibleTiles.Contains(corner));
                    
                    }

                }
            }

            foreach (GridPosition tile in BannedTiles)
            {
               
                for (int i = 0; i<_visibleTiles.Count; i++)
                {
                        if (tile.Equals(_visibleTiles[i])) _visibleTiles.RemoveAt(i);
                }

            }

            foreach (GridPosition tile in _visibleTiles)
            {
                if (tile != null)
                {
                    GameTile tl = GetTile(tile);
                    if (tl)
                    {
                        VisibleTiles.Add(tl);
                        if (debug) tl.Srenderer.color *= Color.red;


                    }
                }
            }

            return _visibleTiles;
        }


        public List<GridPosition> GetAdjancedPositions(GridPosition pos)
        {

            List<GridPosition> adjanced = new List<GridPosition>();

            GridPosition temp = pos.Clone();

            temp.x++;
            if (isInBounds(temp)) adjanced.Add(temp.Clone());
            temp.y++;
            if (isInBounds(temp)) adjanced.Add(temp.Clone());
            temp.x--;
            if (isInBounds(temp)) adjanced.Add(temp.Clone());
            temp.x--;
            if (isInBounds(temp)) adjanced.Add(temp.Clone());
            temp.y--;
            if (isInBounds(temp)) adjanced.Add(temp.Clone());
            temp.y--;
            if (isInBounds(temp)) adjanced.Add(temp.Clone());
            temp.x++;
            if (isInBounds(temp)) adjanced.Add(temp.Clone());
            temp.x++;
            if (isInBounds(temp)) adjanced.Add(temp.Clone());
            temp.y++;
            if (isInBounds(temp)) adjanced.Add(temp.Clone());


            return adjanced;
        }
        public List<GridPosition> GetDialognaldPositions(GridPosition pos)
        {

            List<GridPosition> adjanced = new List<GridPosition>();

            GridPosition temp = pos.Clone();

            temp.x++;
            if (isInBounds(temp)) adjanced.Add(temp.Clone());
            temp.y++;
            temp.x--;
            if (isInBounds(temp)) adjanced.Add(temp.Clone());
            temp.x--;
            temp.y--;
            if (isInBounds(temp)) adjanced.Add(temp.Clone());
            temp.y--;
            temp.x++;
            if (isInBounds(temp)) adjanced.Add(temp.Clone());

  


            return adjanced;
        }

        public static void Swap<T>(ref T x, ref T y)
        {
            T tmp = y;
            y = x;
            x = tmp;
        }

        public bool DoLine(int x1, int y1, int z1, int x2, int y2, int z2)
        {

            int i, dx, dy, dz, l, m, n, x_inc, y_inc, z_inc, err_1, err_2, dx2, dy2, dz2;
            int[] point = new int[] { x1, y1, z1 };
            dx = x2 - x1;
            dy = y2 - y1;
            dz = z2 - z1;
            x_inc = (dx < 0) ? -1 : 1;
            l = Mathf.Abs(dx);
            y_inc = (dy < 0) ? -1 : 1;
            m = Mathf.Abs(dy);
            z_inc = (dz < 0) ? -1 : 1;
            n = Mathf.Abs(dz);
            dx2 = l << 1;
            dy2 = m << 1;
            dz2 = n << 1;

            GridPosition lastDirection = new GridPosition(0, 0, 0);
            GridPosition currentDirection = new GridPosition(0, 0, 0);
            if ((l >= m) && (l >= n))
            {
                err_1 = dy2 - l;
                err_2 = dz2 - l;
                currentDirection = new GridPosition(0, 0, x_inc);
                for (i = 0; i <= l; i++)
                {
                    if (GetTile(new GridPosition(point[2]+z_inc,point[1],point[0]))) return false;
                  
                    if (err_1 > 0)
                    {
                        point[1] += y_inc;
                        err_1 -= dx2;
                        currentDirection.y = y_inc;
                    }
                    if (err_2 > 0)
                    {
                        point[2] += z_inc;
                        err_2 -= dx2;
                        currentDirection.z = z_inc;
                    }
                    err_1 += dy2;
                    err_2 += dz2;
                    point[0] += x_inc;
                    if (currentDirection.z != 0)
                    {
                        GridPosition diff = new GridPosition(point[2], point[1], point[0]) - lastDirection;

                     //   if (GetTile(diff)) return false;

                    }
                    lastDirection = currentDirection.Clone();
                }
             
            }
            else if ((m >= l) && (m >= n))
            {
                err_1 = dx2 - m;
                err_2 = dz2 - m;
                currentDirection = new GridPosition(0, y_inc, 0);
                for (i = 0; i <= m; i++)
                {
                    if (GetTile(new GridPosition(point[2] + z_inc, point[1], point[0]))) return false;

                
                    if (err_1 > 0)
                    {
                        point[0] += x_inc;
                        err_1 -= dy2;
                        currentDirection.x = x_inc;
                    }
                    if (err_2 > 0)
                    {
                        point[2] += z_inc;
                        err_2 -= dy2;
                        currentDirection.z = z_inc;
                    }
                    err_1 += dx2;
                    err_2 += dz2;
                    point[1] += y_inc;

                    if (currentDirection.z != 0)
                    {
                        GridPosition diff = new GridPosition(point[2], point[1], point[0]) - lastDirection;

                     //   if (GetTile(diff)) return false;

                    }
                    lastDirection = currentDirection.Clone();
                }
               
            }
            else {
                err_1 = dy2 - n;
                err_2 = dx2 - n;
                currentDirection = new GridPosition(z_inc, 0, 0);
                for (i = 0; i <= n; i++)
                {
                    if (GetTile(new GridPosition(point[2]+ z_inc, point[1], point[0]))) return false;
                
                    if (err_1 > 0)
                    {
                        point[1] += y_inc;
                        err_1 -= dz2;
                        currentDirection.y = y_inc;
                    }
                    if (err_2 > 0)
                    {
                        point[0] += x_inc;
                        err_2 -= dz2;
                        currentDirection.x = x_inc;
                    }
                    err_1 += dy2;
                    err_2 += dx2;
                    point[2] += z_inc;

                    if (currentDirection.z != 0)
                    {
                        GridPosition diff = new GridPosition(point[2], point[1], point[0]) - lastDirection;
                     //   if (GetTile(diff)) return false;
                    }

                    lastDirection = currentDirection.Clone();
                }

       

            
            }

            return true;
        }
        public GameTile plot(int a, int b, int z = 0)
        {


            GridPosition currentPos = new GridPosition(z, b, a);

           // if (!GameGrid.currentGrid.isInBounds(currentPos)) return false;
           // if (GameGrid.currentGrid.HasTileOnTop(currentPos)) return false;

            return GetTile(currentPos);
        }

        public bool InRange(GridPosition APos, GridPosition BPos, int range)
        {

            GridPosition diff = APos - BPos;

            if (Mathf.Abs(diff.x) <= range && Mathf.Abs(diff.y) <= range)
            {
                return true;
            }
            return false;

        }
        private bool Crossed(GridPosition APos, GridPosition BPos)
        {

            if ((BPos.x == APos.x + 1 || BPos.x == APos.x - 1) && (BPos.y == APos.y || BPos.y == APos.y)) return true;
            else if ((BPos.y == APos.y + 1 || BPos.y == APos.y - 1) && (BPos.x == APos.y || BPos.x == APos.x)) return true;

            return false;

        }

        public static void AddToListIfNotNull<T>(T obj, List<T> list)
        {
            if (obj != null)
                list.Add(obj);
        }


        private List<GameTile> GetTilesAtDirection(int forwardRange, int sideWaysRange, int zRange, GridPosition startPos, GridPosition forward, GridPosition direction)
        {


            List<GameTile> forwardTiles = new List<GameTile>();

            for (int x = 0; x < sideWaysRange + 1; x++)
            {


                GridPosition sidewaysPos = startPos.Clone();

                for (int z = 0; z < zRange + 1; z++)
                {
                    GridPosition forwardpos = sidewaysPos.Clone();

                    for (int i = 0; i < forwardRange + 1; i++)
                    {

                        if (GetTile(forwardpos) != null) forwardTiles.Add(GetTile(forwardpos));
                        if (GetTile(forwardpos + GridPosition.UP) != null)
                        {

                            if (forwardpos == startPos + forward) { Debug.Log("inFront!"); break; }
                            else if (InRange(startPos, forwardpos, 0))
                            {
                                Debug.Log("On sides!");
                                forwardpos += forward;
                                continue;
                            }
                            else
                                break;


                        }

                        forwardpos += forward;

                    }
                    sidewaysPos += GridPosition.UP;
                }

                startPos += direction;

            }

            return forwardTiles;
        }

        public bool HasTileOnTop(GridPosition pos)
        {

            if (!isInBounds(pos)) return false;

            if (pos.z < layers.Count - 1)
            {
                if (layers[pos.z + 1].Rows[pos.y].Tiles[pos.x] == null) return false;
                else return true;

            }
            return false;

        }


        public bool HasTileOnTop(int x, int y, int z)
        {

            if (z < layers.Count - 1)
            {
                if (layers[z + 1].Rows[y].Tiles[x] == null) return false;
                else return true;
            }
            return false;

        }

        public bool CanBeJumpedTo(int maxHeight, GridPosition pos)
        {

            int jumpHeight = 0;
            int i = 0;
            for (i = pos.z; i < layers.Count - 1; i++)
            {
                if (HasTileOnTop(pos.x, pos.y, i))
                {
                    jumpHeight++;

                    if (jumpHeight > maxHeight)
                        return false;
                }
                else
                    break;
            }
            pos.z = i;

            if (!GetTile(pos).isWalkable) return false;

            return true;
        }

        public static int ReverseSide(int side)
        {
            int newSide = 0;
            switch (side)
            {
                case 1: newSide = 3; break;
                case 2: newSide = 4; break;
                case 3: newSide = 1; break;
                case 4: newSide = 2; break;
            }
            return newSide;
        }
        public WalType CanBeWalked(GridPosition currrentpos, GridPosition pos, out GridPosition tile, bool skipGap = false)
        {
            tile = null;

            if (!isInBounds(pos)) return WalType.CannotWalk;
            if (!HasTilesVertical(pos.Clone())) return WalType.CannotWalk;


            if (!HasTileOnTop(pos))
            {
                if (GetTile(pos.x, pos.y, pos.z) != null && GetTile(pos.x, pos.y, pos.z).isWalkable)
                {
                    tile = pos;
                    return WalType.CanWalk;
                }

            }
            else
            {
                if (CanBeJumpedTo(1, pos))
                {
                    tile = pos;


                    GameTile roof = GetTile(currrentpos.x, currrentpos.y, currrentpos.z + 2);
                    if (roof == null && GetTile(pos.x, pos.y, pos.z).isWalkable)
                    {
 
                        return WalType.JumUp;
                    }


                }
            }

            if (pos.z > 0)
            {

                tile = GetHighestJumpablePosition(pos);
                //ERWERTWETRYYRRYEWYREYRE
                if (GetHighestJumpablePosition(pos).z > currrentpos.z && GetTile(tile) != null)
                {
                    GameTile roof = GetTile(currrentpos.x, currrentpos.y, currrentpos.z + 2);
                    if (roof == null && GetTile(tile).isWalkable)
                    {

                        return WalType.JumUp;
                    }

                }
                else if (GetTile(tile) != null && GetTile(tile).isWalkable)
                {
                    return WalType.JumUp;
                }

            }

            if (skipGap)
            {
                GridPosition direction = currrentpos - pos;
                GridPosition tempPos = currrentpos.Clone();
                while (GameGrid.currentGrid.isInBounds(tempPos))
                {
                    tempPos += direction;

                    tile = GetHighestJumpablePosition(pos);

                    if (tile != null) return WalType.CanWalk;
                    
                }
            }

            return WalType.CannotWalk;


        }

        public GridPosition GetHighestJumpablePosition(GridPosition pos)
        {
            GridPosition newpos = pos.Clone();
            List<GameTile> tiles = GetAllTilesVertical(newpos, 0, false);
            bool tileFound = false;
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] != null)
                {
                    tileFound = true;
                    newpos.z = i;
                }
                else if (tiles[i] == null && tileFound)
                {
                    return newpos;
                }
            }

            return newpos;
        }

        public GameTile GetHighestJumpable(GridPosition pos)
        {

            GridPosition newpos = pos;

            List<GameTile> tiles = GetAllTilesVertical(newpos, 0, false);

            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] != null)
                {
              
                    newpos.z = i;
                }

            }

            return layers[newpos.z].Rows[newpos.y].Tiles[newpos.x];
        }

        public Vector3 GetWalkPoint(GridPosition pos)
        {
            return GetTile(pos).CenterPoint;
        }

        public GridPosition FindTile(GameTile tile)
        {
            for (int l = 0; l < layers.Count; l++)
            {

                for (int r = 0; r < layers[l].Rows.Count; r++)
                {
                    for (int c = 0; c < layers[l].Rows[r].Tiles.Count; c++)
                    {
                        if (layers[l].Rows[r].Tiles[c] == tile) return new GridPosition(l, r, c);
                            
                    }
                }

            }

            return new GridPosition(-1, -1, -1);
        }

        public List<GameTile> GetTilesVertical(GridPosition pos, int from = 0, int to = 0, bool includeEmpty = true)
        {
            if (to > layers.Count) to = layers.Count;

            List<GameTile> tiles = new List<GameTile>();
            if (from > layers.Count) return tiles;
            for (int z = from; z < to+1; z++)
            {
                if (includeEmpty || GetTile(pos.x, pos.y, z) != null)
                    tiles.Add(GetTile(pos.x, pos.y, z));
            }
         
            return tiles;

        }

        public List<GameTile> GetAllTilesVertical(GridPosition pos, int from = 0, bool includeEmpty = true)
        {

            List<GameTile> tiles = new List<GameTile>();
            if (from > layers.Count) return tiles;

            for (int z = from; z < layers.Count+1; z++)
            {

                if (includeEmpty || GetTile(pos.x, pos.y, z) != null)
                    tiles.Add(GetTile(pos.x, pos.y, z));
            }

            return tiles;

        }

        public bool HasTilesVertical(GridPosition pos, int from = 0)
        {
            if (from > layers.Count) return false;
            for (int z = from; z < layers.Count; z++)
            {
                if (GetTile(pos.x, pos.y, z) != null) return true;
            }
            return false;
        }

        public static List<GameTile> ToGameTiles(List<GridPosition> list, GameGrid grid)
        {

            List<GameTile> tiles = new List<GameTile>();

            foreach(GridPosition pos in list)
            {
                AddToListIfNotNull<GameTile>(grid.GetTile(pos), tiles);
            }

            return tiles;
        }
        public void Initialize()
        {
            if (layers.Count == 0)
                layers.Add(new GameTileLayer(rows, columns));
        }

        void Awake()
        {
            currentGrid = this;
            Player._instance = null;
            Character.allCharacters.Clear();
        }
        // Use this for initialization
        void Start()
        {

            for (int l = 0; l < layers.Count; l++)
            {

                for (int r = 0; r < layers[l].Rows.Count; r++)
                {
                    for (int c = 0; c < layers[l].Rows[r].Tiles.Count; c++)
                    {
                        if (layers[l].Rows[r].Tiles[c] != null)
                            layers[l].Rows[r].Tiles[c].CalculateOffset(new GridPosition(l, r, c));
                    }
                }

            }

        }

        public void CreateEmptyLayer(int index)
        {
            layers.Insert(index, new GameTileLayer(rows, columns));
        }
        public void CreateBlock(int r, int c, int z)
        {
            GameObject go = GameObject.Instantiate(BlockPrefab.gameObject, gameObject.transform, false);
            go.GetComponent<SpriteRenderer>().sortingOrder = (r * GridSortingWeights[1]) - c * GridSortingWeights[0] + z * GridSortingWeights[2];
            go.transform.localPosition = GetPositionOf(r, c, z);

            layers[z].Rows[r].Tiles[c] = go.GetComponent<GameTile>();
            go.name = "z[" + z + "], c[" + c + "]  r[," + r + "]";

        }


        public void FillLayer(int z, bool onlyEmpty = true)
        {

            for (int r = 0; r < layers[z].Rows.Count; r++)
            {
                for (int c = 0; c < layers[z].Rows[r].Tiles.Count; c++)
                {
                    if (layers[z].Rows[r].Tiles[c] == null)
                    {
                        CreateBlock(r, c, z);
                    }

                }
            }


        }

        public void ResetGrid()
        {
            for (int l = 0; l < layers.Count; l++)
            {

                for (int r = 0; r < layers[l].Rows.Count; r++)
                {
                    for (int c = 0; c < layers[l].Rows[r].Tiles.Count; c++)
                    {
                        if (layers[l].Rows[r].Tiles[c] != null)
                            DestroyBlock(r, c, l);
                    }
                }

            }
        }



        public void DestroyBlock(int r, int c, int z)
        {

            if (layers[z].Rows[r].Tiles[c] != null)
                GameObject.DestroyImmediate(layers[z].Rows[r].Tiles[c].gameObject);

            layers[z].Rows[r].Tiles[c] = null;
        }

        public void DestroyBlock(GameTile tile)
        {

            GridPosition pos = FindTile(tile);
          
            if (pos.Equals(new GridPosition(-1, -1, -1))) return;

            layers[pos.z].Rows[pos.y].Tiles[pos.x] = null;


            #if UNITY_EDITOR
                 UnityEditor.Undo.DestroyObjectImmediate(tile.gameObject);
            #else
                 DestroyImmediate(tile.gameObject);
             #endif

        }

        public void ResetSortingOrder()
        {
            for (int l = 0; l < layers.Count; l++)
            {

                for (int r = 0; r < layers[l].Rows.Count; r++)
                {
                    for (int c = 0; c < layers[l].Rows[r].Tiles.Count; c++)
                    {
                        if (layers[l].Rows[r].Tiles[c] != null)
                        {
                            layers[l].Rows[r].Tiles[c].Srenderer.sortingOrder = (r * GridSortingWeights[1]) - c * GridSortingWeights[0] + l * GridSortingWeights[2];
                            foreach (SpriteRenderer rend in layers[l].Rows[r].Tiles[c].overlays)
                            {
                                rend.sortingOrder = layers[l].Rows[r].Tiles[c].Srenderer.sortingOrder + 1;
                            }
                        }
                    }
                }

            }
        }

        public Vector3 GetPositionOf(int r, int col, int l)
        {
            return BlockDirections.UP * l + BlockDirections.SE * r + BlockDirections.NE * col;
        }

        public GridPosition GetPositionOf(Vector3 worldPos, GridPosition currentPosition)
        {
            GridPosition pos = new GridPosition();

            Vector3 curpos = GetPositionOf(currentPosition.x,currentPosition.y,currentPosition.z);
            Vector3.Distance(worldPos, curpos);


            return pos;
           
        }
        public void CreateBlock(Vector3 pos)
        {
            GameObject go = GameObject.Instantiate(BlockPrefab.gameObject, gameObject.transform, false);
            go.transform.localPosition = pos;
        }
    }



}
