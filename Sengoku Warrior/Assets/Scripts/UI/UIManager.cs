using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace SengokuWarrior
{
    public class UIManager : MonoBehaviour
    {

        public UnityEngine.UI.Text HealthBar;
        public Canvas worldCanvas;
        public static UIManager _instance;

        public UI_element WinScreen;
        public UI_element LoseScreen;
        public UnityEngine.CanvasGroup FadeScreen;
        public GameObject WayPointMarker;
        public TargetIndicator TargetMarker;
        public UnityEngine.UI.Button[] MenuButtons;
        public UI_element[] UI_Windows;
        public GameObject MenuPanel;
        public HPbar hpBarPrefab;
        public GameObject[] rangeTilePrefabs;
        public UnitList unitList;

        public GiveItemDialog itemDialog;
        public TurnPassedIndicator turnIndicator;
        public TextDisplay TextDisplay;
        public DamageIndicator damageIndicatorPrefap;

        public WeaponHitEmitter HitEmitterPrefab;


        private bool UILock = false;




        public static UI_element currentWindow;

        public enum WindowType
        {
            GameUI,
            Inventory,
            MainMenu
        }


        public void UpdateUI()
        {

           // HealthBar.text = Player._instance.getStats().CurrentHP.ToString();
        }

        public void HideAll()
        {
            foreach(UI_element win in UI_Windows)
            {
                win.Hide();
            }
        }
        public void SetUnitList(List<Character> lst)
        {
            unitList.gameObject.SetActive(true);
            unitList.SetData(lst);
        }
        public void ClearUnitList()
        {
            unitList.Clear();
        }
        public void DisplayText(string text)
        {
            TextDisplay.Show(text);
        }
        public void CreateHPbar(CharacterBody body)
        {
          HPbar newbar =  GameObject.Instantiate<HPbar>(hpBarPrefab);
          newbar.transform.SetParent(worldCanvas.transform);
          body.hpBar = newbar;
          newbar.transform.position = body.HelathBarAnchor.transform.position;
          HPbar.allInstances.Add(newbar);
        }
        public void CreateDamageIndicator(Vector3 pos, int damage, Color color)
        {
            DamageIndicator indicator = GameObject.Instantiate<DamageIndicator>(damageIndicatorPrefap);
            indicator.transform.SetParent(worldCanvas.transform);
            indicator.transform.position = pos + new Vector3(0, 0.5f);
            indicator.Show(damage.ToString(), color);
        }
        public GiveItemDialog GetItemDialog()
        {
            return this.itemDialog;
        }

        public void CreateWeaponEffect(int i, Vector3 pos)
        {
            WeaponHitEmitter emitter = GameObject.Instantiate<WeaponHitEmitter>(HitEmitterPrefab);
            emitter.transform.position = pos;
            emitter.SetSorting(i);


        }

        public void SetButtonsActive(bool[] states)
        {
            for(int i = 0; i<states.Length; i++)
            {
                MenuButtons[i].interactable = states[i];
            }
        }

        public void FocusMenuPanel(bool focus = true)
        {
            if (focus) {
                EventSystem.current.SetSelectedGameObject(MenuButtons[3].gameObject);
            }
            else EventSystem.current.SetSelectedGameObject(null);
        }
        // Use this for initialization
        void Awake()
        {
            WinScreen.Hide();
            LoseScreen.Hide();
            _instance = this;
        }

        public void InitNewMission()
        {
            WinScreen.Hide();
            LoseScreen.Hide();
            HPbar.DestroyAllInstances();
            unitList.Clear();

            while (currentWindow != null)
            {
                currentWindow.Hide();
            }
        }

        public void ShowWindow(WindowType _type)
        {
            UI_Windows[(int)_type].Show(currentWindow);
        }



        public void ShowEndScreen(GameEndType endtype)
        {
            if (currentWindow) currentWindow.Hide();
            switch (endtype)
            {
                case GameEndType.Win:
                    WinScreen.Show(null);
                    break;
                case GameEndType.Lose:
                    LoseScreen.Show(null);
                    break;
            }
        }

        public TargetIndicator CreateTargetIndicator(GridPosition position, Color color, bool display)
        {
            TargetIndicator go = GameObject.Instantiate<TargetIndicator>(TargetMarker);
            go.SetTarget(position);
            go.SetColor(color);
            
            return go; 
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
