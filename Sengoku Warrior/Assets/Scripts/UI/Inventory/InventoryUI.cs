using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
namespace SengokuWarrior
{
    public class InventoryUI : UI_element
    {

        public List<UnityEngine.Events.UnityAction> onCancelActions = new List<UnityEngine.Events.UnityAction>();
        private Inventory inventory;              
        private Gear gear = null;

        private List<Button> buttons;

        public GridLayoutGroup buttonContainer;
        public InventoryStatsLabel[] StatLabels;

        public Image characterSprite;
        public Text characterName;


        public Inventory_ItemList_Button InventorybuttonPrefab;
        public GearButton GearButtonPrefab;
        public PopUpMenu DropDownMenu;
        public ItemInfo itemInfo;
        public GearDisplayer gearInfo;
        public Shop_CharacterSelector CharacterSelector;
        public Text MoneyIndicator;


        public static InventoryUI _instance;


        public override void Init()
        {
            _instance = this;
            MoneyIndicator.text = GameManager.saveData.PlayerMoney.ToString();
            DropDownMenu.gameObject.SetActive(false);
            onCancelActions.Clear();
            onCancelActions.Add(Hide);
            CharacterSelector.Initialize(Team.allTeams[TurnManager.currentCharacter.TeamNumber].members, () => Refresh());
            inventory = TurnManager.currentCharacter.inventroy;
            gear = TurnManager.currentCharacter.stats.Gear;
          //  characterSprite.sprite = TurnManager.currentCharacter.UIIcon;
           // characterName.text = TurnManager.currentCharacter._Name;
            GenerateUI();
           // UpdateStatLabels();
            GameManager.SetControlMode(ControlsMode.None);

        }

        public override void Hide()
        {
            base.Hide();
            GameManager.RestoreLastControlMode();
        }

        public override void Update()
        {
            if (Input.GetButtonUp("Cancel"))
            {
                if (onCancelActions.Any())
                {
                    UnityEngine.Events.UnityAction act = onCancelActions.Last();
                    onCancelActions.Remove(onCancelActions.Last());
                    act.Invoke();
                   
                }
            }
        }

        void Refresh()
        {
            inventory = CharacterSelector.SelectedCharacter.inventroy;
            gear = CharacterSelector.SelectedCharacter.stats.Gear;
            itemInfo.Clear();
            GenerateUI();
        }


        private void UpdateStatLabels()
        {
            return;
            TurnManager.currentCharacter.stats.CalculateStats();
            StatLabels[(int)Stats.StatType.HP].value.text = TurnManager.currentCharacter.stats.CurrentHP.ToString() + "/" + TurnManager.currentCharacter.stats.CalculatedHP.ToString();
            StatLabels[(int)Stats.StatType.MP].value.text = TurnManager.currentCharacter.stats.CurrentMP.ToString() + "/" + TurnManager.currentCharacter.stats.CalculatedMP.ToString();
            StatLabels[(int)Stats.StatType.Attack].value.text = TurnManager.currentCharacter.stats.CalculatedAtt.ToString();
            StatLabels[(int)Stats.StatType.Defence].value.text = TurnManager.currentCharacter.stats.CalculatedDef.ToString();
            StatLabels[(int)Stats.StatType.Speed].value.text = TurnManager.currentCharacter.stats.CalculatedSpeed.ToString();

        }

        private void GenerateUI()
        {
            DestroyChildren(buttonContainer.transform);

            int _i = 0;
            for (int i=0; i<inventory.Items.Count; i++)
            {
                _i = i;
                Inventory_ItemList_Button btn = GameObject.Instantiate<Inventory_ItemList_Button>(InventorybuttonPrefab);
                RectTransform transform = btn.GetComponent<RectTransform>();
                btn.transform.SetParent(buttonContainer.transform,false);
                btn.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);           
                btn.onClick.RemoveAllListeners();

                ButtonArgument equipArgument = new ButtonArgument("Equip");          
                NewItem itm = inventory.Items[_i].item;
                equipArgument.onClickCallback.Add(() => OpenMiniMenuInventory(itm));
                btn.SetData(inventory.Items[_i],equipArgument);

            }
           
            
            for (int q = 0; q < gear.items.Length; q++)
            {
                int _q = q;
                gearInfo.buttons[q].onClick.RemoveAllListeners();
                if (gear.items[q])
                {
                    gearInfo.buttons[q].interactable = true;
                    ButtonArgument unequipArgument = new ButtonArgument("Unequip");
                    unequipArgument.onClickCallback.Add(() => OpenMiniMenuStats(new IntMessage(_q)));
                    NewItem itm = gear.items[q];
   
                    gearInfo.buttons[q].SetData(itm, unequipArgument);

                }
                else
                {
                    gearInfo.buttons[q].Clear();
                    gearInfo.buttons[q].interactable = false;

                    if (gear.Names[q])
                    {

                        if (gear.Names[q].GetAttribute<CommonAttributes>())
                            gearInfo.buttons[q].InventorySprite.sprite = gear.Names[q].GetAttribute<CommonAttributes>().GetSprite();
                    }
                    else
                        gearInfo.buttons[q].InventorySprite.sprite = null;
                }
            }
           

        }
        
        private class IntMessage { public int i = 0; public IntMessage(int i) { this.i = i; } }

        private void OpenMiniMenuInventory(NewItem itm)
        {
            List<ButtonArgument> buttonArguments = new List<ButtonArgument>();
            onCancelActions.Remove(DropDownMenu.Hide);
            onCancelActions.Add(DropDownMenu.Hide);
            if (itm.GetAttribute<UseAttribute>() != null)
            {
                ButtonArgument UseArgument = new ButtonArgument("Use");


                UseArgument.onClickCallback.Add(() => DropDownMenu.Hide());
                UseArgument.onClickCallback.Add(() => itm.GetAttribute<UseAttribute>().Use(TurnManager.currentCharacter));

                if (itm.GetAttribute<UseAttribute>().destoryOnUse)
                    UseArgument.onClickCallback.Add(() => inventory.DepleteItem(itm));

                UseArgument.onClickCallback.Add(() => GenerateUI());
                UseArgument.onClickCallback.Add(() => UpdateStatLabels());


                buttonArguments.Add(UseArgument);
            }

            if (itm.GetAttribute<GearSlotAttribute>() != null)
            {

               ButtonArgument GearArgument = new ButtonArgument("Equip");

                WeaponAttribute weaponAttr = itm.GetAttribute<WeaponAttribute>();
                if (weaponAttr)
                {
                    GearArgument.Interactable = TurnManager.currentCharacter.stats.CanEquipWeapons.Contains((int)weaponAttr.WeaponType) ? true : false;
                }

                GearArgument.onClickCallback.Add(() => DropDownMenu.Hide());
                GearArgument.onClickCallback.Add(() => gear.Equip(itm, inventory));
                GearArgument.onClickCallback.Add(() => UpdateStatLabels());
                GearArgument.onClickCallback.Add(() => GenerateUI());

                buttonArguments.Add(GearArgument);

            }


            ButtonArgument DropArgument = new ButtonArgument("Drop");
            DropArgument.onClickCallback.Add(() => DropDownMenu.Hide());
            DropArgument.onClickCallback.Add(() => inventory.DepleteItem(itm));
            DropArgument.onClickCallback.Add(() => GenerateUI());

            buttonArguments.Add(DropArgument);


            DropDownMenu.Initialize(buttonArguments, null);
        }

        private void OpenMiniMenuStats(IntMessage msg)
        {
            NewItem itm = gear.items[msg.i];
            if (itm)
            {
                onCancelActions.Remove(DropDownMenu.Hide);
                onCancelActions.Add(DropDownMenu.Hide);

                ButtonArgument Argument = new ButtonArgument("Unequip");
                Argument.onClickCallback.Add(() => DropDownMenu.Hide());
                Argument.onClickCallback.Add(() => gear.Unequip(msg.i, inventory));
                Argument.onClickCallback.Add(() => GenerateUI());
                Argument.onClickCallback.Add(() => UpdateStatLabels());

                DropDownMenu.Initialize(new List<ButtonArgument>() { Argument }, null);
            }
            else
                Debug.Log("No Item : " + msg.i.ToString());

        }

        private void OpenMiniGear(NewItem itm, int slot)
        {

        }
        private void DestroyChildren(Transform transform)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

    }
}
