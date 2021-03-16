using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SengokuWarrior
{
    public enum GameEndType
    {
        Win,
        Lose
    }

    public enum ControlsMode
    {
        Disabled,
        MoveTarget,
        AttackTarget,
        View,
        None

    }
     
    public class GameManager : MonoBehaviour
    {
      
        public CharacterBody characterPrefab;
        public Loadable DefaultMission;

        public Loadable current;


        public CharacterTemplateDatabase CharacterDatabase;
        public static GameManager _intance;
        public static bool isPaused = false;
        public static ControlsMode controlsMode = ControlsMode.Disabled;
        private static ControlsMode lastCmode = ControlsMode.None;
        private bool inputBlock = false;

        public ItemDatabase itemDatabase;
        public LoadableDatabase loadables;

        [System.NonSerialized]
        public static List<List<Character>> Teams = new List<List<Character>>();


        private static Loadable missionToBeLoaded;
        public TurnManager turnManager;
        public static Save saveData = null;


        public void Test(int a, int b) { }
        void Awake()
        {
            if (_intance != this) GameObject.Destroy(_intance);
            _intance = this;
            if (saveData != null) LoadSaveData(saveData);
            else {
                saveData = new Save();
                saveData.SaveAllCharacters();
            }
     
        }

        public void LoadSaveData(Save data)
        {
            saveData = data;

            if (data.LoadableId != -1)
            {
                DefaultMission = loadables.GetLoadableById(data.LoadableId);
            }
            LoadScene(DefaultMission);
        }




        public static void LoadScene(Loadable MissionToBeLoaded)
        {
            if (Application.isPlaying)
            {

                TurnManager.TurnNumber = 0;
                TurnManager.currentTeam = 0;
                missionToBeLoaded = Instantiate(MissionToBeLoaded);
                GameManager._intance.current = missionToBeLoaded;


                missionToBeLoaded.original = MissionToBeLoaded;
                CameraTransitionHandlerer.fadeEnded += UnloadLastScene;
                _intance.StartCoroutine( CameraTransitionHandlerer._intance.BeginFadeIn());
            }
        }

        void Start()
        {

            UIManager._instance.HideAll();

            turnManager = new TurnManager();
            LoadScene(DefaultMission);

            //  Player._instance.SpawnPositon = GameGrid.currentGrid.PlayerSpawn;
            // Player._instance.MoveInstant(Player._instance.SpawnPositon);
            AudioManager._instance.PlayMusic("calm");
        }

        public void ResetMission()
        {
            LoadScene(Loadable.CurrentlyLoaded.original);
        }

        public void QuitGame()
        {
            CameraTransitionHandlerer.fadeEnded += () => Application.Quit();
            _intance.StartCoroutine(CameraTransitionHandlerer._intance.BeginFadeIn());
        }

        public void LoadUnityScene(int scene)
        {
            _intance.StartCoroutine(_LoadUnityScene(scene)); 
        }

        private IEnumerator _LoadUnityScene(int scene)
        {
            _intance.StartCoroutine(CameraTransitionHandlerer._intance.BeginFadeIn());
            yield return new WaitUntil(() => !CameraTransitionHandlerer._intance.fadeInProgress());
            UIManager._instance.HideAll();
            UIManager._instance.InitNewMission();
            BattleUIContainer._instance.Hide();
            if (Loadable.CurrentlyLoaded != null)
            {
                Loadable.CurrentlyLoaded.End();
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(Loadable.CurrentlyLoaded.scenePath);
                yield return new WaitUntil(() => !SceneManager.GetSceneByPath(Loadable.CurrentlyLoaded.scenePath).isLoaded);
            }

            SceneManager.LoadSceneAsync(scene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            yield return new WaitUntil(() => SceneManager.GetSceneByBuildIndex(scene).isLoaded);
            _intance.StartCoroutine(CameraTransitionHandlerer._intance.BeginFadeOut());


        }

        private static void UnloadLastScene()
        {
            CameraTransitionHandlerer.fadeEnded -= UnloadLastScene;

            if (Loadable.CurrentlyLoaded != null)
            {
                Loadable.CurrentlyLoaded.End();
            }

            if (Loadable.CurrentlyLoaded != missionToBeLoaded && Loadable.CurrentlyLoaded != null)
            {
                SceneManager.sceneUnloaded += OnSceneUnloaded;
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(Loadable.CurrentlyLoaded.scenePath);
            }
            else if (UnityEngine.SceneManagement.SceneManager.GetSceneByPath(missionToBeLoaded.scenePath).isLoaded)
            {
                SceneManager.sceneUnloaded += OnSceneUnloaded;
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(missionToBeLoaded.scenePath);
            }
            else {

                BeginLoadingScene();
            }
        }

        public static void SetControlMode(ControlsMode newMode)
        {
            lastCmode = controlsMode;
            controlsMode = newMode;
        }
        public static void RestoreLastControlMode()
        {
           controlsMode = lastCmode;

        }
        private static void OnSceneUnloaded(Scene scene)
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            CameraTransitionHandlerer.fadeEnded -= BeginLoadingScene;
            UIManager._instance.HideAll();
            BeginLoadingScene(); 

        }

        private static void BeginLoadingScene()
        {
            SceneManager.LoadSceneAsync(missionToBeLoaded.scenePath, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            GameGrid.currentGrid = Component.FindObjectOfType<GameGrid>();
            SceneManager.sceneLoaded += OnMissionLoaded;
        }

        private static void OnMissionLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnMissionLoaded;
            Loadable.CurrentlyLoaded = missionToBeLoaded;
            if (saveData != null && saveData.LoadableId == missionToBeLoaded.ID) missionToBeLoaded.LoadData(saveData);
            else missionToBeLoaded.LoadDefaultData();
            missionToBeLoaded = null;

            UIManager._instance.InitNewMission();
            Loadable.CurrentlyLoaded.Init();
            CameraTransitionHandlerer.fadeEnded += Loadable.CurrentlyLoaded.Begin;        

            _intance.StartCoroutine(CameraTransitionHandlerer._intance.BeginFadeOut());
           
        }


     

        public void CreateCharacterBody(Character chara, Color overlayColor)
        {
            
            if (chara.prefab) chara.body = GameObject.Instantiate<CharacterBody>(chara.prefab);
            else
                chara.body = GameObject.Instantiate<CharacterBody>(characterPrefab);

            chara.body.GetComponent<SpriteRenderer>().color = overlayColor;
            chara.body.charadata = chara;
            chara.OnStart(chara.body);
        }

        void Update()
        {

            if (controlsMode == ControlsMode.None) return;
            switch (controlsMode)
            {
                case ControlsMode.MoveTarget:
                    turnManager.UpdateCurrentAction(MoveTarget()); 
                break;

                case ControlsMode.AttackTarget:
                    turnManager.UpdateCurrentAction(MoveTarget());
                    break;
                case ControlsMode.View:
                    turnManager.UpdateCurrentAction(MoveTarget());
                    break;
            }
            
            if (controlsMode != ControlsMode.Disabled)
            {
                if (Input.GetButtonDown("Submit"))
                {
                     turnManager.ConfirmCurrentAction();
                }
                if (Input.GetButtonUp("Cancel"))
                {
                    turnManager.Cancel();
                }
            }

        }

        public void ExecuteTurnMenuCommand(MenuCommand command)
        {
            if (controlsMode == ControlsMode.None) return;
            switch (command)
            {
                case MenuCommand.Attack:
                    turnManager.BeginAction(new AttackAction(TurnManager.currentCharacter));
                    break;

                case MenuCommand.Guard:
                    turnManager.EndTurnGuard();
                break;

                case MenuCommand.Wait:
                    turnManager.EndTurnWait();
                break;

                case MenuCommand.Move:
                    turnManager.BeginAction(new MoveAction(TurnManager.currentCharacter));
                    break;

                case MenuCommand.Cancel:
                    turnManager.Cancel();
                break;
                case MenuCommand.Confirm:
                    turnManager.ConfirmCurrentAction();
                    break;
                case MenuCommand.Next:
                    turnManager.CancelAll();
                    turnManager.NextCharacter(1);
                    break;
                case MenuCommand.Previous:
                    turnManager.CancelAll();
                    turnManager.NextCharacter(-1);
                    break;
            }

            StartCoroutine(BlockInput(controlsMode));
        }
        public void ExecuteTurnMenuCommand(int command){ExecuteTurnMenuCommand((MenuCommand)command);}

        private GridPosition MoveTarget()
        {
            GridPosition newPos = Target.CurrentPosition;
            if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
            {
                int selecteddir = InputHandlerer.GetDirectionFromInput();
                GridPosition futurePosition = Target.CurrentPosition.Clone();


                if (selecteddir == 3) futurePosition.y++;
                else if (selecteddir == 1) futurePosition.y--;
                else if (selecteddir == 2) futurePosition.x++;
                else if (selecteddir == 4) futurePosition.x--;

                GridPosition pos;
                if (GameGrid.currentGrid.CanBeWalked(Target.CurrentPosition, futurePosition, out pos) != GameGrid.WalType.CannotWalk)
                {
                    newPos = pos;
                }
          }

            return newPos;
        }

        public static IEnumerator BlockInput(ControlsMode original)
        {
            controlsMode = ControlsMode.Disabled;
            yield return new WaitForSeconds(0.15f);
            controlsMode = original;
        }


        public void EndGame(GameEndType type)
        {
            UIManager._instance.ShowEndScreen(type);    
        }

        public void SlowDown()
        {
            Time.timeScale = 0;
        }
        public void UnPause()
        {
            Time.timeScale = 1;
        }

        public void OnStepTest()
        {
            Debug.Log("Hello");
        }

        public void OnActionTest()
        {
            Debug.Log("Hello Action!");
        }

    }
}