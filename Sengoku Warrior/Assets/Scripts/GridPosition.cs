using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior
{
    [System.Serializable]
    public class GridPosition
    {


        public int x = 0;
        public int y = 0;
        public int z = 0;

        public void Clear()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public static GridPosition zero = new GridPosition(0, 0, 0);


        public GridPosition Clone()
        {
            GridPosition newPos = new GridPosition();
            newPos.x = this.x;
            newPos.y = this.y;
            newPos.z = this.z;
            return newPos;
        }
        public GridPosition() { }
        public GridPosition(int z, int y, int x)
        {

            this.z = z;
            this.y = y;
            this.x = x;
        }

        public static GridPosition operator +(GridPosition a, GridPosition b)
        {
            a = a.Clone();

            a.x += b.x;
            a.y += b.y;
            a.z += b.z;

            return a;
        }
        public static GridPosition operator -(GridPosition a, GridPosition b)
        {
            a = a.Clone();

            a.x -= b.x;
            a.y -= b.y;
            a.z -= b.z;

            return a;
        }

        public static GridPosition operator *(GridPosition a, int b)
        {
            a = a.Clone();

            a.x *= b;
            a.y *= b;
            a.z *= b;

            return a;
        }

        public bool Equals(GridPosition other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // You can also use a specific StringComparer instead of EqualityComparer<string>
            // Check out the specific implementations (StringComparer.CurrentCulture, e.a.).
            if (this.x != other.x || this.y != other.y || this.z != other.z)
            {
                return false;
            }


            // To compare the members array, you could perhaps use the 
            // [SequenceEquals][2] method.  But, be aware that [] {"a", "b"} will not
            // be considerd equal as [] {"b", "a"}

            return true;

        }



        public override string ToString()
        {
            return "x :" + this.x + ", y: " + this.y + ", z: " + this.z;
        }

        public static GridPosition UP
        {
            get
            {
                return new GridPosition(1, 0, 0);
            }
        }

        public static GridPosition Left(GridPosition forward)
        {

            GridPosition left = new GridPosition();

            if (Mathf.Abs(forward.x) != Mathf.Abs(forward.y))
            {
                if (Mathf.Abs(forward.x) > Mathf.Abs(forward.y))
                {

                    if (forward.x > 0) left.y = -1;
                    else left.y = 1;


                }
                else {
                    if (forward.y > 0) left.x = 1;
                    else left.x = -1;
                }
            }

            return left;
        }
        public static GridPosition Reverse(GridPosition forward)
        {
            forward.x *= -1;
            forward.z *= -1;
            forward.y *= -1;

            return forward;
        }


        public GridPosition GetReversed()
        {
            GridPosition newpos = this.Clone();
            return Reverse(newpos);
        }

        public void Set(GridPosition pos)
        {
            this.x = pos.x;
            this.y = pos.y;
            this.z = pos.z;
        }

        public int Distance(GridPosition B)
        {
            return (int)Mathf.Abs((int)Mathf.Abs(B.x-this.x)+ (int)Mathf.Abs(B.y-this.y)+ (int)Mathf.Abs((B.z-this.z)));
        }

    }
}