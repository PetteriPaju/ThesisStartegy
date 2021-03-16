using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SengokuWarrior
{
    public abstract class UseAttribute:ItemAttribute
    {
        public bool destoryOnUse = true;
        public override int Priority()
        {
            return 99;
        }

        public virtual bool Use(Character chara) { return destoryOnUse; }

    }
}
