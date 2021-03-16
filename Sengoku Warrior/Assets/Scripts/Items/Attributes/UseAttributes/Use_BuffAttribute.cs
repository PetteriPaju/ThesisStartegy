using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SengokuWarrior
{
 
    public class Use_BuffAttribute : UseAttribute
    {
        public float[] buffs = new float[5] { 0, 0, 0, 0, 0 };
        public int turnTime = 1;

        public override bool Use(Character chara)
        {
            return this.destoryOnUse;
        }

    }
}

