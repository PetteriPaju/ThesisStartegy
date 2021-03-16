using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
namespace SengokuWarrior
{
    [Serializable]
    public class Character : IDamageReceiver
    {

        public CharacterBody prefab;
    
        [SerializeField]
        private bool isUnique = false;

        private bool[] directionFlips = new bool[] { true, false, false, true };

        [NonSerialized]
        private bool spawnOverride = false;
        public string _Name = "New Character";
        public UID id;
        public GridPosition SpawnPositon= new GridPosition();
        public GameGrid.BlockDirections.FaceDirections SpawnDirection = GameGrid.BlockDirections.FaceDirections.SE;
        [System.NonSerialized]
        public static List<Character> allCharacters = new List<Character>();

        public static List<Character> allCharactersAlive {
            get { return allCharacters.Where(s => s.isAlive).ToList(); }
        }


        public bool isunique
        {
            get
            {
                return isUnique;
            }
        }

        protected bool attacking = false;


        public int Faceddirection = 1;
        [NonSerialized]
        public UnityEngine.Events.UnityEvent OnUpdate = new UnityEngine.Events.UnityEvent();
        [NonSerialized]
        public UnityEngine.Events.UnityAction OnPathUpdate;
        [NonSerialized]
        public UnityEngine.Events.UnityAction OnPathComplete;
        [NonSerialized]
        protected bool isMoving = false;
        [NonSerialized]
        public CharacterBody body;

        public MissionCondition SpawnCondition = null;

        protected bool walkingPath = false;
        private int currentPathIndex = 0;

       
        private GridPosition GoToPos = new GridPosition(0, 20, 10);
        private List<GridPosition> currentPath;

        protected bool canPlayStepSound = true;

        private bool spawned = false;
        public bool isSpawned
        {
            get
            {
                return spawned;
            }
        }
        [System.NonSerialized]
        public bool[] TurnActionsPreformed = new bool[2] { false, false };

        private int team = -1;

        public Sprite UIIcon;
    
        public List<GridPosition> onStartPath = new List<GridPosition>();

        public Stats stats = new Stats();
        public Inventory inventroy = new Inventory();

        public List<Skill> Skills = new List<Skill>();

       [System.NonSerialized]
        public GridPosition Position = new GridPosition();
        [System.NonSerialized]
        public GridPosition TruePosition;
        [System.NonSerialized]
        public InputHandlerer inputeHandlerer;

        public Vector3 _topOffset = new Vector3(0, 0.4f, 0);
        public List<Vector3> _FeetOffset = new List<Vector3>();

        public AIBehaviour aiBehaviour;

        public int TeamNumber
        {
            get
            {
                return team;
            }
            set
            {
                if (value != team)
                {
                    if (value != -1)
                    {
                        if (Team.allTeams.Count !=0 && Team.allTeams.Count-1 >= team && team != -1) 
                        Team.allTeams[team].Members.Remove(this);

                        if (Team.allTeams.Count != 0 && value <= Team.allTeams.Count-1)
                            Team.allTeams[value].AddToTeam(this);

                        team = value;
                    }
                }
            }
        }

        public void SetTeam(int team)
        {
            this.team = team;
        }
        public Transform GetTransform()
        {
            return body.transform;
        }

        public bool isAvailable()
        {
            if (!isAlive) return false;
            if (body == null) return false;
            return true;
        }
       
        public Character()
        {
            if (this.id == null) this.id = new UID();
            this._Name = "New Character";
        }

        public Character(bool isUnique)
        {
            if (this.id == null) this.id = new UID();
            this._Name = "New Character";
            this.isUnique = isUnique;
        }

        public Character(string name)
        {
            if (this.id == null) this.id = new UID();
            this._Name = name;
        }


        public Animator anim
        {
            get
            {
                return body.animator;
            }
        }

        public Character Clone(bool unique)
        {

          //  Debug.Log("Character cloned");
            Character newChara = ObjectCopier.Clone<Character>(this);
            if (!spawned)
                newChara.SpawnDirection = this.SpawnDirection;
            else
                newChara.SpawnDirection = (GameGrid.BlockDirections.FaceDirections)this.Faceddirection;

            return newChara;
            
        }

        public void SoftClone(Character chara)
        {
         //   Debug.Log("Character soft cloned");
            this._Name = chara._Name;
            List<int> equipbables = chara.stats.CanEquipWeapons.Select(item => (int)item).ToList();
            this.stats = new Stats(chara._Name, chara.stats.Level, chara.stats.BaseHP, chara.stats.BaseMP, chara.stats.BaseAttack, chara.stats.BaseDef, chara.stats.BaseSpeed, chara.stats.Gear.Clone(), equipbables, this);
            Inventory newInventory = new Inventory(chara.inventroy.Items.Select(item => (ItemStack)item.Duplicate()).ToList());
            this.inventroy = newInventory;
            this.onStartPath = chara.onStartPath.Select(item => item.Clone()).ToList();
            this.UIIcon = chara.UIIcon;
            this.prefab = chara.prefab;
        }

        public void LoadSaveData(Save.SavedCharacterGamePlay SavedChara)
        {
            this.isUnique = SavedChara.isUnique;       
            List<int> equipbables = SavedChara.SavedStats.CanEquipWeapons.Select(item => (int)item).ToList();
            this.stats = new Stats(this._Name, 0, SavedChara.SavedStats.BaseHP, SavedChara.SavedStats.BaseMP, SavedChara.SavedStats.BaseAttack, SavedChara.SavedStats.BaseDef, SavedChara.SavedStats.BaseSpeed, SavedChara.SavedStats.Gear.Clone(), equipbables, this);
            
            this.stats.SetCurrentHP(SavedChara.currentHp);
            this.stats.SetCurrentMP(SavedChara.currentMP);

            Inventory newInventory = new Inventory(SavedChara.SavedInventory.Items.Select(item =>item.Duplicate()).ToList());
            this.inventroy = newInventory;
            this.spawned = SavedChara.isSpawned;
            this.TurnActionsPreformed = SavedChara.TurnActionsPreformed;
            this.onStartPath = SavedChara.onStartPath.Select(item => item.Clone()).ToList();
            this.Position = SavedChara.currentPosition.Clone();
            this.SpawnDirection = (GameGrid.BlockDirections.FaceDirections)SavedChara.SpawnDirection;
            if (SavedChara.isSpawned)
            {
                this.SpawnPositon = SavedChara.currentPosition.Clone();
                spawnOverride = true;
            }



        }

        public void LoadSaveData(Save.SavedCharacter SavedChara)
        {

            this.isUnique = SavedChara.isUnique;
            List<int> equipbables = SavedChara.SavedStats.CanEquipWeapons.Select(item => (int)item).ToList();
            this.stats = new Stats(this._Name, 0, SavedChara.SavedStats.BaseHP, SavedChara.SavedStats.BaseMP, SavedChara.SavedStats.BaseAttack, SavedChara.SavedStats.BaseDef, SavedChara.SavedStats.BaseSpeed, SavedChara.SavedStats.Gear.Clone(), equipbables, this);
            Inventory newInventory = new Inventory(SavedChara.SavedInventory.Items.Select(item => (ItemStack)item.Duplicate()).ToList());
            this.inventroy = newInventory;

        }

        public bool isAlive
        {
            get
            {
                return stats.CurrentHP > 0 && isSpawned;
            }
        }

        public GridPosition getGridPosition()
        {
            return Position;
        }

        public Stats getStats()
        {
            return this.stats;
        }
    
        public bool RecieveAttack(AttackMessage message)
        {
            stats.AddHealth(message.Health);
            body.hpBar.UpdateBar((float)stats.CurrentHP/(float)stats.CalculatedHP);
            UIManager._instance.CreateDamageIndicator(body.transform.position,(int)message.Health, Color.red);

           
            anim.Play("Hit");
            if (getStats().CurrentHP <= 0)
            {
                anim.SetBool("isDead", true);
                Die();
                return true;
            }
            else {
              //  AudioManager._instance.PlayClip("pain");
                body.StartCoroutine(HitInvinsibility());
                return false;
            }
        }
        public void ActivateSkill(int i, Player activator)
        {
            Skills[i].Activate(activator);
        }

        public Vector3 topOffset
        {
            get{return _topOffset;}
        }

        public Vector3 PositionOnGrid(Vector3 worldPos)
        {

            return worldPos - GameGrid.currentGrid.GetTile(Position).centerPoint.transform.localPosition - topOffset + _FeetOffset[Faceddirection] - GameGrid.BlockDirections.UP * Position.z;


        }


        public GridPosition currentPosition(Vector3 worldPos)
        {
            //   Debug.Log((worldPos.x / exTendX + worldPos.y / exTendY) / 2);
            float x = worldPos.x / GameGrid.BlockDirections.NE.x/2 + worldPos.y / GameGrid.BlockDirections.NE.y/2;
            float y = worldPos.y / GameGrid.BlockDirections.SE.y / 2 + worldPos.x / GameGrid.BlockDirections.SE.x / 2;
           
            int z = Position.z;
            return new GridPosition(z,(int)Mathf.Round(y), (int)Mathf.Round(x));
        }


        public IEnumerator MoveAlongPath(List<GridPosition> lst)
        {

            currentPath = lst;
            yield return new WaitForSeconds(0.25f);
            while (currentPath.Count != 0)
            {
                WalkTo(currentPath[0]);
                currentPath.RemoveAt(0);

                yield return new WaitWhile(() => isMoving);
             
                   
                if (OnPathUpdate != null) OnPathUpdate.Invoke();
            }

            yield return new WaitWhile(() => isMoving);
          
            if (OnPathComplete != null) OnPathComplete.Invoke();


            walkingPath = false;
        }

        public IEnumerator MoveTo(GridPosition pos)
        {
            if (walkingPath) yield return null;

        
            if (pos != null)
            {
                GoToPos = pos;

                yield return new WaitWhile(() => isMoving);
                walkingPath = true;

                AStar pathfinder = AStar.GetPath(Position, GoToPos, GameGrid.currentGrid,null);
                currentPath = pathfinder.CellsFromPath();

                while (currentPath.Count != 0)
                {

                    WalkTo(currentPath[0]);
                    currentPath.RemoveAt(0);

                    yield return new WaitWhile(() => isMoving);
                }

                yield return new WaitWhile(() => isMoving);
                Position = GoToPos.Clone();
                walkingPath = false;
            }
        }

        public Vector3 FeetPosisionOnGrid(Vector3 pos)
        {
            return PositionOnGrid(pos);// + GameGrid.ActorOffset;
        }

        public void FaceTarget(GridPosition pos)
        {
            SetAnimatorDirection(InputHandlerer.GetDirectionTo(Position, pos));
        }

        public GridPosition GetForward()
        {
            return InputHandlerer.GetMovement(Faceddirection);
        }

        public void FaceDirection(GameGrid.BlockDirections.FaceDirections dir)
        {
            SetAnimatorDirection((int)dir);

        }

        public Vector3 GetJumpVector(int height, int amount, int direction)
        {
            Vector3 jumpHeight = ((height + 1) * GameGrid.BlockDirections.UP) + GameGrid.BlockDirections.GetDirection(Faceddirection) * amount / 2;
            return jumpHeight;
        }
        public void SetAnimatorDirection(int dir)
        {
            Faceddirection = dir;

            body.sRenderer.flipX = directionFlips[dir - 1];

            anim.SetInteger("dir", (int)dir);
      }

        public void MoveInstant(GridPosition pos)
        {
            GameTile tile = GameGrid.currentGrid.GetTile(pos);
       
            if (tile)
            {
                body.transform.position = tile.CenterPoint + topOffset;

                Position = pos.Clone();

                TruePosition = Position.Clone();
                body.sRenderer.sortingOrder = InputHandlerer.GetSortingOrder(this);
                body.shadowRenderer.sortingOrder = body.sRenderer.sortingOrder - 1;

            }
            else
            {
                Debug.LogWarning("No Tile At Spawnpoint : " + pos);
            }

        }

        public virtual void OnStart(CharacterBody body)
        {
            body.gameObject.SetActive(false);
            allCharacters.Add(this);

            /*
            if (isUnique)
            {
                Save.SavedCharacter chara = CharacterInfoStorage.GetData(this.id.ToInt());
                if (chara != null)
                {
                    LoadSaveData(chara);
                }
                else
                {
                    CharacterInfoStorage.RegisterData(this);
                }
            }
            */
            inventroy.Init();
            if ((SpawnCondition == null && !isSpawned) || spawnOverride) Spawn();

        }

        public void Spawn()
        {
            body.gameObject.SetActive(true);
            spawned = true;
            inputeHandlerer = new InputHandlerer();
            MoveInstant(SpawnPositon);
            FaceDirection(SpawnDirection+1);
            UIManager._instance.CreateHPbar(body);
            body.hpBar.UpdateBar((float)stats.CurrentHP / (float)stats.CalculatedHP);
        }
        public virtual void Die()
        {
            //AudioManager._instance.PlayClip("Die");
            iTween.FadeTo(body.gameObject, iTween.Hash("a", 0, "time", 2f, "delay", 0.5f));
            GameObject.Destroy(body.gameObject, 5f);

        }

        public bool enemiesAtRange()
        {
            return GetEnemiesAtRange(stats.attackRange).Count != 0;
        }
        public List<Character> GetEnemiesAtRange(int range)
        {

            List<GridPosition> tilesInAttackRange = GameGrid.currentGrid.GetPositiosnInrange(this.Position.x,this.Position.y,this.Position.z, range, range);
            //Get every unity that exist inside attack range
            List<Character> enemies = this.EnemiesInsideRange(tilesInAttackRange);

            enemies.Sort((a, b) => (Vector3.Distance(a.body.transform.position, this.body.transform.position).CompareTo(Vector3.Distance(b.body.transform.position, this.body.transform.position))));
            return enemies;

        }

        public static bool isCharacterInTile(GridPosition pos)
        {
            foreach (Character enemy in Character.allCharacters)
            {
                if (enemy.isAlive)
                    if (pos.Equals(enemy.Position)) return true;
            }

            return false;
        }
        public static bool isOtherCharacterInTile(GridPosition pos, Character chara)
        {
            foreach (Character enemy in Character.allCharacters)
            {
                if (enemy.isAlive && enemy != chara)
                    if (pos.Equals(enemy.Position)) return true;
            }

            return false;
        }
        public static bool isOtherTeamInTile(GridPosition pos, Character chara)
        {
            foreach (Character enemy in Character.allCharacters)
            {
                if (enemy.isAlive && enemy != chara && enemy.TeamNumber != chara.TeamNumber)
                    if (pos.Equals(enemy.Position)) return true;
            }

            return false;
        }
        public static Character GetCharacterAtTile(GridPosition pos)
        {
            if (pos == null) return null;
            foreach (Character enemy in Character.allCharacters)
            {
                if (enemy.isAlive)
                    if (pos.Equals(enemy.Position)) return enemy;
            }

            return null;
        }
        public static Character GetEnemyAtTile(GridPosition pos, Character chara)
        {
            if (pos == null) return null;
            foreach (Character enemy in chara.GetAllEnemies())
            {
                if (enemy.isAlive)
                    if (pos.Equals(enemy.Position)) return enemy;
            }

            return null;
        }
        public List<Character> GetAllEnemies(bool includeDead = false)
        {
            List<Character> enemies = new List<Character>();

            for (int i = 0; i < Mission.currentMission.Teams.Count; i++)
            {
                if (i != TeamNumber)
                {
                    enemies.AddRange(Mission.currentMission.Teams[i].GetAliveMembers());
                }
            }
            enemies.Sort((a, b) => (Position.Distance(a.Position).CompareTo(Position.Distance(b.Position))));
            return enemies;
        }
        public List<Character> EnemiesInsideRange(List<GridPosition> lst)
        {
            List<Character> characters = new List<Character>();

            foreach(GridPosition pos in lst)
            {
                GameGrid.AddToListIfNotNull<Character>(GetEnemyAtTile(pos,this), characters);

            }
            characters.Sort((a, b) => (Position.Distance(a.Position).CompareTo(Position.Distance(b.Position))));
            return characters;
        }
        public static List<Character> sortByPredictedDamage(Character attacker, List<Character> targets)
        {
            List<Character> enemies = targets;
            enemies.Sort((a, b) => (attacker.stats.DamageTo(a.stats).CompareTo(attacker.stats.DamageTo(a.stats))));
            return enemies;


        }
        public static List<Character> sortByHPLeftdDamage(Character attacker, List<Character> targets)
        {
            List<Character> enemies = targets;
            enemies.Sort((a, b) => (attacker.stats.HPAfterAttack(a.stats).CompareTo(attacker.stats.HPAfterAttack(a.stats))));
            return enemies;


        }
        protected void WalkTo(GridPosition nextTile)
        {
            TruePosition = Position.Clone();
            int lastDir = Faceddirection;
            Faceddirection = InputHandlerer.GetDirectionTo(Position, nextTile);

            SetAnimatorDirection(Faceddirection);
            if (lastDir != Faceddirection)
            {
             //   Debug.Log("Turn " + Faceddirection);
                anim.Play("Walk");
            }
               
            if (GameGrid.currentGrid.isInBounds(nextTile))
            {


                GridPosition selectedTile;
                GameGrid.WalType canWalk = GameGrid.currentGrid.CanBeWalked(Position, nextTile, out selectedTile);
                if (canWalk == GameGrid.WalType.JumUp || Position.z != nextTile.z)
                {
                    int height = nextTile.z - Position.z;
                    if (height < 0) height = 1;
                    Position = nextTile;

                    StartJump(GameGrid.currentGrid.GetWalkPoint(nextTile) + topOffset, height - 1);
                }
                else if (canWalk == GameGrid.WalType.CanWalk)
                {
                    Position = nextTile;
                    iTween.MoveTo(body.gameObject, iTween.Hash("position", GameGrid.currentGrid.GetTile(selectedTile).CenterPoint + topOffset, "time", Options._instance.CharacterMovementSpeed, "easetype", iTween.EaseType.linear));
                    body.StartCoroutine(MovementMidpoint(Options._instance.CharacterMovementSpeed / 2));
                    body.StartCoroutine(MoveEnabler(Options._instance.CharacterMovementSpeed));

                }
     
            }
        
        }

        public virtual void WalkingUpdate()
        {

        }


        protected void StartJump(Vector3 newPos, int jumpheight)
        {
            anim.Play("Jump", 0);
            List<Vector3> calculatedPath = new List<Vector3>();

            Vector3 diff = newPos - body.transform.position;

            Vector3 midpoing = newPos - diff + GetJumpVector(0, 1, anim.GetInteger("dir"));

            calculatedPath.Add(midpoing);
            calculatedPath.Add(newPos);

  


            iTween.MoveTo(body.gameObject, iTween.Hash("path", calculatedPath.ToArray(), "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
            body.shadowRenderer.enabled = false;
            body.StartCoroutine(MovementMidpoint(0.25f));
            body.StartCoroutine(MoveEnabler(0.5f));
            body.StartCoroutine(EndJump(0.3f));
        }

        protected IEnumerator EndJump(float delay)
        {
          
            yield return new WaitForSeconds(delay);
            body.shadowRenderer.enabled = true;
            if (GameGrid.currentGrid.GetTile(Position))
            {
                if (GameGrid.currentGrid.GetTile(Position).jumpAudio)
                {
                    AudioManager._instance.OneShotPlay(GameGrid.currentGrid.GetTile(Position).jumpAudio);
                }
            }
            anim.Play("Walk", 0);    
        }

        protected IEnumerator MovementMidpoint(float time)
        {
            yield return new WaitForSeconds(time);
            GridPosition tempPos = Position.Clone();
            TruePosition = Position.Clone();

         body.GetComponent<SpriteRenderer>().sortingOrder = InputHandlerer.GetSortingOrder(this)+5;
            body.shadowRenderer.sortingOrder = body.sRenderer.sortingOrder - 1;
        }

        protected IEnumerator MovementMidpointAttack(float time)
        {
            yield return new WaitForSeconds(time);
          //  if (targetEnemy != null)targetEnemy.RecieveAttack();
            anim.Play("Attack");
            GridPosition tempPos = Position.Clone();
            TruePosition = Position.Clone();

           body.GetComponent<SpriteRenderer>().sortingOrder = InputHandlerer.GetSortingOrder(this)+5;
            body.shadowRenderer.sortingOrder = body.sRenderer.sortingOrder - 1;
        }

        protected IEnumerator StepSoundEnabler(float duration)
        {
            canPlayStepSound = false;
               yield return new WaitForSeconds(duration);
            canPlayStepSound = true;
        }


        protected IEnumerator MoveEnabler(float time)
        {
            isMoving = true;
            yield return new WaitForSeconds(time);
            isMoving = false;
        }

        protected IEnumerator HitInvinsibility()
        {
            Color orgColor = body.sRenderer.color;
            body.sRenderer.color *= new Color(1, 1, 1, 0.5f);
            isMoving = true;
            yield return new WaitForSecondsRealtime(0.25f);
            isMoving = false;

            body.sRenderer.color = orgColor;
        }


    }
}
