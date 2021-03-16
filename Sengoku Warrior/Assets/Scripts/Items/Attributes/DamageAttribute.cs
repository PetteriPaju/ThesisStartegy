using System;
using System.Collections.Generic;

namespace SengokuWarrior
{
    public class DamageAttribute : ItemAttribute
    {
        public override int Priority() { return 9; }
        public float dmg = 5;

        public override float floatValue(){ return dmg;}
    }
}
