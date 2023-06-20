using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Yut.DataModule;

namespace Yut.WorldBoss
{
    public class ZombieRegion
    {
        public string BossType;
        public uint BossHealth;
        public byte MaxMinions;
        public List<Minion> Minions;
        public string BossTable;
        public string MinionTable;
        public ZombieRegion() { }
        public ZombieRegion(string bossType, uint bossHealth, byte maxMinions, List<Minion> minions, string bossTable, string minionTable)
        {
            BossType = bossType;
            BossHealth = bossHealth;
            MaxMinions = maxMinions;
            Minions = minions;
            BossTable = bossTable;
            MinionTable = minionTable;
        }
    }
}
