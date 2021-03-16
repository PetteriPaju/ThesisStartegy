using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior
{
    public class GameTile : MonoBehaviour
    {
        public GameObject centerPoint;

        public AudioClip[] stepSounds;

        public AudioClip jumpAudio;

        public UnityEngine.Events.UnityEvent doOnAction = null;
        public UnityEngine.Events.UnityEvent doOnStep = null;

        private bool OnStepsDone = false;
        private bool onActionDone = false;

        private Vector3 offset = Vector3.zero;

        public SpriteRenderer Srenderer;
        public bool isWalkable = true;

        public List<SpriteRenderer> overlays = new List<SpriteRenderer>();
        public Color DefaultColor = Color.white;
        public Color CurrentColor = Color.white;

        // Use this for initialization
        void Start()
        {
            Srenderer.color = DefaultColor;
            CurrentColor = Srenderer.color;
        }

     

        public Vector3 CenterPoint
        {
            get
            {
                return centerPoint.transform.position;
            }
        }

        public Vector3 Offset
        {
            get
            {
                return offset;
            }
        }


        public void CalculateOffset(GridPosition pos)
        {
            Vector3 zeropos = GameGrid.BlockDirections.SE * pos.y + GameGrid.BlockDirections.NE * pos.x + GameGrid.BlockDirections.UP * pos.z;

            offset = transform.localPosition - zeropos;

        }

        public bool hasEndEvents()
        {
            return doOnStep.GetPersistentEventCount() != 0 && !OnStepsDone;
        }

        public void EndEventsDone()
        {
            OnStepsDone = true;
        }
        public void ActionEventsDone()
        {
            onActionDone = true;
        }

        public bool hasActionEvents()
        {
            return doOnAction.GetPersistentEventCount() != 0 && !onActionDone;
        }

        public void SetData(GameTileSerialized info)
        {
            this.centerPoint.transform.localPosition = info.centerPoint;
            this.stepSounds = info.stepSounds;
            this.jumpAudio = info.jumpAudio;
            this.isWalkable = info.isWalkable;
        }

        public void AddOverLay(GameTile tile)
        {
            GameObject go = new GameObject();
            go.AddComponent<SpriteRenderer>();
            go.GetComponent<SpriteRenderer>().sortingOrder = tile.Srenderer.sortingOrder + 1;
            go.transform.localPosition = tile.gameObject.transform.localPosition;
            go.transform.SetParent(tile.gameObject.transform);
            tile.overlays.Add(go.GetComponent<SpriteRenderer>());
            go.name = "overlay";

            #if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create object");
            #endif
        }

    }
}
