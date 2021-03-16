using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SengokuWarrior
{
    public class GearSlotAttribute : ItemAttribute
    {

        public override int Priority(){return 11;}
                                                 //Helmet,Plate,Leggings,LeftA,RightA,Ring
        public bool[] PossibleSlots = new bool[6] { false, false, false, false, false, false };
        public bool[] UnequipLots = new bool[6]   { false, false, false, false, false, false };

    }
}
