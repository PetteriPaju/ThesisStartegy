using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SengokuWarrior {
    public class UnitViewer : MonoBehaviour {


        public Text Name;
        public Image Portrait;
        public HPbar HpBar;
        public HPbar HpPreview;

    public void RefreshView(Character chara)
    {
            if (chara == null)
            {
                Clear();
                return;
            }
      Name.text = chara._Name;
      Portrait.sprite = chara.UIIcon;
      HpBar.UpdateBar(chara.stats.CurrentHP / chara.stats.CalculatedHP);
      
    }
    
    public void SetDamagePreview(Stats TargetStats, Stats AttackerStats)
    {
            float val = TargetStats.CurrentHP / TargetStats.CalculatedHP;
            float val2 = AttackerStats.HPAfterAttack(TargetStats) / TargetStats.CalculatedHP;

            HpBar.UpdateBar(val);
            HpPreview.UpdateBar(val2);
            HpPreview.gameObject.SetActive(true);

            HpBar.gameObject.AddComponent<UIImageFlicker>();



    }
    public void HideDamagePreview()
        {
            HpPreview.gameObject.SetActive(false);
            if (HpBar.GetComponent<UIImageFlicker>())
            {
                Component.Destroy(HpBar.GetComponent<UIImageFlicker>());
            }
        }


    public void Clear()
        {
          Name.text = string.Empty;
          Portrait.sprite = null;
          HideDamagePreview();
          HpBar.UpdateBar(0);
          
        }


    }
}
