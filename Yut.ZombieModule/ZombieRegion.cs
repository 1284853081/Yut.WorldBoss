using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yut.ZombieModule
{
    public class ZombieRegion
    {
        public string RegionName;
        public string BossType;
        public uint BossHealth;
        public ZombieCloth BossCloth;
        public byte MaxMinions;
        public List<Minion> Minions;
        public List<ZombieCloth> MinionCloths;
        public ZombieRegion() { }
        public ZombieRegion(string regionName, string bossType, uint bossHealth, ZombieCloth bossCloth, byte maxMinions, List<Minion> minions, List<ZombieCloth> minionCloths)
        {
            RegionName = regionName;
            BossType = bossType;
            BossHealth = bossHealth;
            BossCloth = bossCloth;
            MaxMinions = maxMinions;
            Minions = minions;
            MinionCloths = minionCloths;
        }
    }
}
