using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior
{
    [CreateAssetMenu(fileName = "Skill", menuName = "Create Skill/Skill", order = 1)]
    [System.Serializable]
    public class Skill: ScriptableObject {

        public bool StopTime = true;
        public int SkillRange = 5;

        public int Uses = 0;


        private Player activator;

        protected List<GameTile> tilesInRange = new List<GameTile>();
        protected List<Character> targets = new List<Character>();
        protected List<GameTile> targetTiles = new List<GameTile>();

        private int currenTarget = 0;

        private bool tap = false;
        public GameObject effect;

        public virtual void Activate(Player activator)
        {

            this.activator = activator;
            if (this.Uses <= 0) return;
            targets.Clear();
            currenTarget = 0;
            activator.anim.updateMode = AnimatorUpdateMode.UnscaledTime;
            activator.OnUpdate.RemoveAllListeners();
            activator.OnUpdate.AddListener(this.UpdateFunction);
            UpdateTargets();
          if (StopTime) GameManager._intance.SlowDown();

        }

        public void SetRangeColor(List<GameTile> tiles, Color color)
        {

            foreach(GameTile tile in tiles)
            {
                tile.Srenderer.color = color;
            }


        }

        protected void UpdateTargets()
        {
            ResetColors(tilesInRange);
           // tilesInRange = GameGrid.currentGrid.toTiles(GameGrid.currentGrid.GetVisibleTiles(activator.Position, activator.GetForward(), 5,activator.DebugMode, activator == Player._instance));

            if (TimerBar.ShownAbar != null)
            {
                TimerBar.ShownAbar.Hide();
                TimerBar.ShownAbar = null;
            }

            targets = activator.GetEnemiesAtRange(SkillRange);
            targetTiles.Clear();

          //  foreach (Enemy target in targets)
           // {
            //    GameGrid.AddToListIfNotNull<GameTile>(GameGrid.currentGrid.GetTile(target.TruePosition), targetTiles);
           // }


            SetRangeColor(tilesInRange, Color.red);
            SetRangeColor(targetTiles, Color.blue);


            if (targets.Count != 0) {
                GameGrid.currentGrid.GetTile(targets[currenTarget].TruePosition).Srenderer.color = Color.yellow;
                activator.FaceTarget(targets[currenTarget].TruePosition);

            }

        }


        public void ResetColors(List<GameTile> tiles)
        {

            foreach (GameTile tile in tiles)
            {
                if (tile != null)
                tile.Srenderer.color = tile.CurrentColor;
            }



        }
        public virtual void UpdateFunction()
        {

            if (Input.GetButtonDown("Skill")) Cancel();
        

            if (Input.GetAxisRaw("Horizontal") != 0 && targets.Count > 1)
            {
                if (!tap)
                {
                    tap = true;

                    currenTarget++;
                    if (currenTarget >= targets.Count)
                        currenTarget = 0;


                    ResetColors(tilesInRange);
                    SetRangeColor(tilesInRange, Color.red);
                    SetRangeColor(targetTiles, Color.blue);

                    if (TimerBar.ShownAbar != null)
                    {
                        TimerBar.ShownAbar.Hide();
                        TimerBar.ShownAbar = null;
                    }

                    if (targets.Count != 0)
                    {
                        GameGrid.currentGrid.GetTile(targets[currenTarget].TruePosition).Srenderer.color = Color.yellow;
                        activator.FaceTarget(targets[currenTarget].Position);

                    }
                }
            }
            else tap = false;


            if (Input.GetButtonDown("Attack"))
            {
                if (targets.Count != 0)
                {
                    this.Uses--;
                    AudioManager._instance.PlayClip("Swing");
                   
                    Cancel();
                    activator.anim.Play("Attack");

                    GameObject _effect = GameObject.Instantiate(effect);
                    _effect.transform.position = activator.body.transform.position;
                    iTween.MoveTo(_effect, iTween.Hash("position", targets[currenTarget].body.transform.position, "time", 0.25f));
                    GameObject.Destroy(_effect, 0.33f);
                    activator.body.StartCoroutine(HitDelay());
                  

                }
            }


        }

        private IEnumerator HitDelay()
        {
            yield return new WaitForSeconds(0.25f);

            targets[currenTarget].Die();

        }
        public virtual void Cancel()
        {
            ResetColors(tilesInRange);
            if (TimerBar.ShownAbar != null)
            {
                TimerBar.ShownAbar.Hide();
                TimerBar.ShownAbar = null;
            }
            AudioManager._instance.MusicPlayer.pitch = 1f;
            activator.OnUpdate.RemoveListener(UpdateFunction);
            activator.OnUpdate.AddListener(activator.WalkingUpdate);
            activator.anim.updateMode = AnimatorUpdateMode.Normal;
            GameManager._intance.UnPause();

        }



    }
}