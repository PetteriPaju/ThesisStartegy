using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SengokuWarrior
{
    public class BattleUIContainer : UI_element
    {
        public static BattleUIContainer _instance;

        public UnitViewer UnitView;

        public static void RefreshUnitView(Character chara)
        {
            _instance.UnitView.RefreshView(chara);
        }


        void Awake()
        {
            _instance = this;
            UnitView.Clear();
        }


        public override void Show(UI_element _parent)
        {
            base.Show(_parent);
            GameManager.RestoreLastControlMode();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void Update()
        {
           
        }


    }
}
