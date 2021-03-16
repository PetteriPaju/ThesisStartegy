using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace SengokuWarrior
{
    public enum MenuCommand
    {
        Wait,
        Attack,
        Guard,
        Move,
        Cancel,
        Confirm,
        Next,
        Previous,
        Inventory,
        Menu
    }
    [Serializable]
    public class TurnManager
    {
        public static Character currentCharacter;
        private static int turnNum = 0;
        public static int currentTeam = 0;
        private List<Character> currentCharacters = new List<Character>();
        public static GridPosition TargetPosition;
        private UnityEngine.Events.UnityAction onConfirm;

        public List<UnityEngine.Events.UnityAction> onCancelActions;
        public bool doingAction = false;
        private TurnAction currentAction;
        public static TurnManager _instance;

        public static bool performingAction = false;


        private List<MissionEventContainer> currentEvents = new List<MissionEventContainer>();


        public static int TurnNumber
        {
            get
            {
                return turnNum;
            }

            set
            {
                turnNum = value;
            }
        }



        public TurnManager()
        {
            _instance = this;
        }


        private List<Team> listOfTeams;
        
        private List<Character> sortBySpeed(bool excludeDead = true)
        {
            List<Character> listedBySpeed = Character.allCharacters;
            listedBySpeed.Sort((a, b) => (a.stats.TurnsMade.CompareTo(b.stats.TurnsMade)));
            return listedBySpeed;
        }

        public void Initialize()
        {
            listOfTeams = Mission.currentMission.Teams;
            UIManager._instance.ShowWindow(UIManager.WindowType.GameUI);
            UIManager._instance.SetButtonsActive(new bool[] { false, false, false, false, false, false, false, false, false,false });
            currentCharacters = new List<Character>();
            currentCharacters.AddRange(listOfTeams[currentTeam].GetAliveMembersWithTurnPending());
            currentCharacter = currentCharacters[0];
            BattleUIContainer.RefreshUnitView(currentCharacter);
            GameManager._intance.StartCoroutine(BeginAfterTurnDisplay());
        }

        private IEnumerator BeginAfterTurnDisplay()
        {


            currentEvents = Mission.currentMission.GetTrueEvents();
            if (currentEvents.Count != 0)
            {
                GameManager._intance.StartCoroutine(DoEvents());
            }
            yield return new WaitUntil(() => currentEvents.Count == 0);

            UIManager._instance.turnIndicator.Show(listOfTeams[currentTeam].TeamName, listOfTeams[currentTeam].TeamColor);

            yield return new WaitUntil(() => !UIManager._instance.turnIndicator.gameObject.activeSelf);
  

            BeginTurn();
        }

        public void NextCharacter(int dir)
        {

            int currentIndex = currentCharacters.IndexOf(currentCharacter);
            currentIndex += dir;


            if (currentIndex > currentCharacters.Count - 1)
            {
                currentIndex = 0;
            }
            else if (currentIndex < 0)
            {
                currentIndex = currentCharacters.Count-1;
            }

            onConfirm = null;
            currentCharacter = currentCharacters[currentIndex];
            CameraFollow.target = currentCharacter.body.transform;

            GameManager.SetControlMode(ControlsMode.Disabled);

            if (currentCharacter.aiBehaviour == null)
            {
                currentCharacter.anim.Play("Jump");
                UIManager._instance.MenuPanel.gameObject.SetActive(true);
                UIManager._instance.FocusMenuPanel(true);
                UIManager._instance.SetButtonsActive(new bool[] { true, !currentCharacter.TurnActionsPreformed[0], !currentCharacter.TurnActionsPreformed[0], !currentCharacter.TurnActionsPreformed[1], false, false, currentCharacters.Count > 1, currentCharacters.Count > 1, true, true });
            }
            else
            {
                currentCharacter.aiBehaviour.Think(currentCharacter.body.aiHandelerer, GameGrid.currentGrid);
                UIManager._instance.MenuPanel.gameObject.SetActive(false);
            }
        }

        public void BeginTurn()
        {
          
            doingAction = false;
            UIManager._instance.ClearUnitList();
            onConfirm = null;
            onCancelActions = new List<UnityEngine.Events.UnityAction>();         
            currentCharacter = currentCharacters[0];
         
            CameraFollow.target = currentCharacter.body.transform;
            BattleUIContainer.RefreshUnitView(currentCharacter);
            GameManager.SetControlMode(ControlsMode.Disabled);

            if (currentCharacter.aiBehaviour == null)
            {
                currentCharacter.anim.Play("Jump");
                UIManager._instance.MenuPanel.gameObject.SetActive(true);
                UIManager._instance.FocusMenuPanel(true);
                UIManager._instance.SetButtonsActive(new bool[] { true, !currentCharacter.TurnActionsPreformed[0], !currentCharacter.TurnActionsPreformed[0], !currentCharacter.TurnActionsPreformed[1], false, false, currentCharacters.Count > 1, currentCharacters.Count > 1, true, true });
            }
            else
            {
                currentCharacter.aiBehaviour.Think(currentCharacter.body.aiHandelerer, GameGrid.currentGrid);
                UIManager._instance.MenuPanel.gameObject.SetActive(false);
            }

        }

        public void AddCancel(UnityEngine.Events.UnityAction method)
        {
            onCancelActions.Insert(0,method);
        }

        public void RemoveCancel(UnityEngine.Events.UnityAction method)
        {
            onCancelActions.Remove(method);
        }



        public void Cancel()
        {
           if(onCancelActions != null && onCancelActions.Count != 0)
            {
                if (onCancelActions[0] != null) onCancelActions[0].Invoke();

                onCancelActions.RemoveAt(0);
            }
        }

        public void CancelAll()
        {
            foreach(UnityEngine.Events.UnityAction action in onCancelActions)
            {
                if (action != null) action.Invoke();
            }

            onCancelActions.Clear();
            currentAction = null;
            UIManager._instance.SetButtonsActive(new bool[] { true, !currentCharacter.TurnActionsPreformed[0], !currentCharacter.TurnActionsPreformed[0], !currentCharacter.TurnActionsPreformed[1], false, false, currentCharacters.Count > 1, currentCharacters.Count > 1, true, true });

        }

        public void CancelCurrentAction()
        {
            doingAction = false;
            currentAction.Cancel();
            currentAction = null;

            UIManager._instance.FocusMenuPanel(true);
            UIManager._instance.SetButtonsActive(new bool[] { true, !currentCharacter.TurnActionsPreformed[0] , !currentCharacter.TurnActionsPreformed[0], !currentCharacter.TurnActionsPreformed[1], false, false, currentCharacters.Count > 1, currentCharacters.Count > 1, true, true });
        }

        public void ConfirmCurrentAction()
        {
         
            if (onConfirm != null) {
                onConfirm.Invoke();
                onConfirm = null;
            }
            CameraFollow.target = currentCharacter.body.transform;
        }

        public void UpdateCurrentAction(GridPosition pos)
        {
            if (currentAction != null) currentAction.Update(pos);
        }

        public void BeginAction(TurnAction action)
        {
            onCancelActions.Add(CancelCurrentAction);
            onConfirm = action.Execute;
            currentAction = action;
            currentAction.Begin();
         
        }

        public void ActionOVer()
        {
            GameManager._intance.StartCoroutine(WaitForAction());
        }

        public bool CheckForWin()
        {
            bool hasEnded = false;
            if (Mission.currentMission.WinDondition != null)
            {
                if (Mission.currentMission.WinDondition.Check())
                {
                    hasEnded = true;
                    GameManager._intance.EndGame(GameEndType.Win);
                }
                else
                {
                    if (Mission.currentMission.LoseCondition != null)
                    {
                        if (Mission.currentMission.LoseCondition.Check())
                        {
                            hasEnded = true;
                            GameManager._intance.EndGame(GameEndType.Lose);
                        }
                    }
                    else
                    {
                        Debug.Log("No Lose conditions");
                    }
                }
            }
            else
            {
                Debug.Log("No Victory conditions");
            }
            return hasEnded;
        }

        public IEnumerator WaitForAction()
        {
            yield return new WaitUntil(() =>!performingAction);

            onCancelActions.Remove(CancelCurrentAction);
            onConfirm = null;
            TargetPosition = null;

            currentEvents = Mission.currentMission.GetTrueEvents();
            if (currentEvents.Count != 0)
            {
                GameManager._intance.StartCoroutine(DoEvents());            
            }
            yield return new WaitUntil(() => currentEvents.Count == 0);


            UIManager._instance.FocusMenuPanel(true);
            UIManager._instance.SetButtonsActive(new bool[] { true, !currentCharacter.TurnActionsPreformed[0], !currentCharacter.TurnActionsPreformed[0], !currentCharacter.TurnActionsPreformed[1], false, false, currentCharacters.Count > 1, currentCharacters.Count > 1, true,true });

            if (!CheckForWin())
            {
                if (!currentCharacter.isAlive) TurnOver();
                else
                if (currentCharacter.aiBehaviour != null) currentCharacter.aiBehaviour.Think(currentCharacter.body.aiHandelerer, GameGrid.currentGrid);
            }
        }


        public IEnumerator DoEvents()
        {
            for (int i=0; i < currentEvents.Count; i++)
            {
                MissionEventContainer container = currentEvents[i];
                container.DoneCurrent = false;
                GameManager._intance.StartCoroutine(container.DoAll());
                yield return new WaitUntil(() => container.DoneCurrent);
            }
    
            currentEvents.Clear();

        }

        public void EndTurnWait()
        {
            GameManager.SetControlMode(ControlsMode.Disabled);
            TurnOver();
        }
        public void EndTurnGuard()
        {
            EndTurnWait();
        }
        public void TurnOver()
        {

            UIManager._instance.SetButtonsActive(new bool[] { false, false, false, false, false, false, false, false, false,false });
            UIManager._instance.FocusMenuPanel(false);
            GameManager._intance.StartCoroutine(WaitForActionBeforeEnd());

        }
        public IEnumerator WaitForActionBeforeEnd()
        {
            yield return new WaitUntil(() => !performingAction);
            yield return new WaitUntil(() => currentEvents.Count == 0);
            Target.Clear();
            currentCharacter.stats.TurnsMade++;
            currentCharacters.Remove(currentCharacter);
            bool teamChanged = false;

            if (currentCharacters.Count == 0)
            {
                List<Character> thisTeam = new List<Character>();
                thisTeam.AddRange(listOfTeams[currentTeam].GetAliveMembers());

                for (int i = 0; i<thisTeam.Count; i++)
                {
                    thisTeam[i].TurnActionsPreformed[0] = false;
                    thisTeam[i].TurnActionsPreformed[1] = false;
                }
            }

            while (currentCharacters.Count == 0 && Character.allCharactersAlive.Count > 0)
            {
                currentTeam++;
                if (currentTeam > listOfTeams.Count - 1)
                {
                    currentTeam = 0;
                    turnNum++;

                    foreach (Team tm in Team.allTeams)
                    {
                        tm.TurnEndedUpdate();
                    }

                }

                currentCharacters = new List<Character>();
                currentCharacters.AddRange(listOfTeams[currentTeam].GetAliveMembers());
                teamChanged = true;   
            }
            currentEvents = Mission.currentMission.GetTrueEvents();
            if (currentEvents.Count != 0)
            {
                GameManager._intance.StartCoroutine(DoEvents());

            }
            yield return new WaitUntil(() => currentEvents.Count == 0);

            if (!CheckForWin())
            {
                if (teamChanged)
                {
                    UIManager._instance.turnIndicator.Show(listOfTeams[currentTeam].TeamName, listOfTeams[currentTeam].TeamColor);
                    yield return new WaitUntil(() => !UIManager._instance.turnIndicator.gameObject.activeSelf);
                }


                BeginTurn();
            }

        }

    }
}
