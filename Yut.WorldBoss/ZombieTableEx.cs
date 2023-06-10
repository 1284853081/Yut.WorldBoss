using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yut.ZombieModule;

namespace Yut.WorldBoss
{
    public static class ZombieTableEx
    {
        public static void AddSuit(this SDG.Unturned.ZombieTable table,ZombieCloth cloth)
        {
            table.addCloth(0, cloth.shirt);
            table.addCloth(1, cloth.pants);
            table.addCloth(2, cloth.hat);
            table.addCloth(3, cloth.gear);
        }
    }
}
