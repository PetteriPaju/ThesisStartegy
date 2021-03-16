using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SengokuWarrior
{
    public class Use_RecoverAttribute : UseAttribute
    {
        public float[] Recover = new float[2] { 10, 10 };
        public bool[] enabled = new bool[2] { false, false };
        public override bool Use(Character chara)
        {
            if (enabled[(int)Stats.StatType.HP]) chara.stats.AddHealth(Recover[(int)Stats.StatType.HP]);
            if (enabled[(int)Stats.StatType.MP]) chara.stats.DepleteMP(-Recover[(int)Stats.StatType.MP]);

            Debug.Log("Use");
            return destoryOnUse;
        }
    }
}
