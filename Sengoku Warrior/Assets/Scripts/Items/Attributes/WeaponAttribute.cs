using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SengokuWarrior
{
    public class WeaponAttribute : ItemAttribute
    {
        public override int Priority() { return 10; }
        public int range = 1;
        public Stats.WeaponType WeaponType = Stats.WeaponType.Sword;
        public override int intValue(){return range;}
    }
}
