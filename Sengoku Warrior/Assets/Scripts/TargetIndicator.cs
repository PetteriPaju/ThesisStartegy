using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior {
    public class TargetIndicator : MonoBehaviour {

        public List<LayerSortingGroup> Sprites = new List<LayerSortingGroup>();
        // Use this for initialization

        private static List<TargetIndicator> allTargets = new List<TargetIndicator>();
        public GridPosition currentPosition;

        public void SetTarget(GridPosition pos)
        {
                currentPosition = pos;
                gameObject.transform.position = GameGrid.currentGrid.GetPositionOf(pos.y,pos.x,pos.z) + GameGrid.BlockDirections.UP;
                SetSorting(InputHandlerer.GetSortingOrder(pos) + 5);
        }

        public void SetShow(bool status)
        {
            gameObject.SetActive(status);
        }
        public static void DestroyAll()
        {
            foreach (TargetIndicator target in allTargets)
            {
                target.Destroy();
            }
        }
        public void Destroy()
        {
            GameObject.Destroy(this.gameObject);    
        }
        void OnDestroy()
        {
            allTargets.Remove(this);
        }
        void Awake() {
            allTargets.Add(this);
        }


        public void SetSorting(int i)
        {
            for (int q = 0; q < Sprites.Count; q++)
            {
                Sprites[q].SetSorting(i);
            }
        }

        public void SetColor(Color color)
        {
            for (int q = 0; q < Sprites.Count; q++)
            {
                Sprites[q].renderer.color = color;
            }
        }

        [System.Serializable]
        public class LayerSortingGroup
        {
            public SpriteRenderer renderer;
            public int Sorting_Order = 0;

            public void SetSorting(int value)
            {
                renderer.sortingOrder = value + Sorting_Order;
            }



        }
    }

}