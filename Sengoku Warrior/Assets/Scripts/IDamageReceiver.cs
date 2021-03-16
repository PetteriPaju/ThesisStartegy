using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SengokuWarrior
{
    public interface IDamageReceiver : ITargetable
    {
        Stats getStats();
        bool RecieveAttack(AttackMessage message);
    }
}
