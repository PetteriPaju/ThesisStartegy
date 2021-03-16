using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior
{
    public class RangeDisplayer
    {
        public enum RangeType
        {
            Move,
            Attack
        }
        private GameObject Container;
        private List<GameObject> hightlighters = new List<GameObject>();
        private static RangeDisplayer _main;
        private static RangeDisplayer _secondary;

        public RangeType currentRangeType = RangeType.Attack;
       public static RangeDisplayer _Main
        {
            get
            {
                if (_main == null) _main = new RangeDisplayer();
                return _main;
            }
        }
        public static RangeDisplayer _Secondary{

            get
            {
                if (_secondary == null) _secondary = new RangeDisplayer();
                return _secondary;
            }

            }

        public void DisplayRange(List<GridPosition> posses, RangeType rType)
        {
            currentRangeType = rType;
            Clear();
            Container = new GameObject();
            foreach (GridPosition pos in posses) {
                GridPosition pos2 = pos.Clone();
                GameObject go = GameObject.Instantiate(UIManager._instance.rangeTilePrefabs[(int)rType]);             
                go.transform.position = GameGrid.currentGrid.GetPositionOf(pos2.y, pos2.x, pos2.z) + GameGrid.BlockDirections.UP/2;
                pos2.z--;
                go.GetComponent<SpriteRenderer>().sortingOrder = InputHandlerer.GetSortingOrder(pos2) + 1;
                hightlighters.Add(go);
                go.transform.parent = Container.transform;
                    }
        }


        public void Clear()
        {

            GameObject.Destroy(Container);
            hightlighters = new List<GameObject>();
        }
        public void SetShow(bool status)
        {
            if (Container)
            Container.SetActive(status);
        }


    }
}
