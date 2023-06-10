using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Yut.ZombieModule;

namespace Yut.WorldBoss
{
    public class Config : ZombieConfig,  IRocketPluginConfiguration
    {
        public byte RefreshHour;
        public byte RefreshMinute;
        public ItemPair Ticket;
        public Vector3 BossRefreshPoint;
        public ushort PrepareSeconds;
        public ushort FightingSeconds;
        public ushort RewardSeconds;
        public uint MinRewardDamage;
        public List<RewardInterval> Rewards;
        public void LoadDefaults()
        {
            ZombieCloth cloth = new ZombieCloth(0,0,0,0);
            List<Minion> minions = new List<Minion>()
            {
                new Minion("NORMAL", 100)
            };
            List<ZombieCloth> cloths = new List<ZombieCloth>()
            {
                new ZombieCloth(0,0,0,0)
            };
            RefreshHour = 0;
            RefreshMinute = 0;
            Ticket = new ItemPair(0, 1);
            regions = new List<ZombieRegion>()
            {
                new ZombieRegion("TestRegion","BOSS_FIRE",1000000,cloth,5,minions,cloths)
            };
            Rewards = new List<RewardInterval>()
            {
                new RewardInterval(1,1,new List<ItemPair>(){ new ItemPair(14,1)}),
                new RewardInterval(2,2,new List<ItemPair>(){ new ItemPair(14,1)}),
                new RewardInterval(3,3,new List<ItemPair>(){ new ItemPair(14,1)}),
                new RewardInterval(4,10,new List<ItemPair>(){ new ItemPair(14,1)}),
                new RewardInterval(11,100,new List<ItemPair>(){ new ItemPair(14,1)}),
            };
            BossRefreshPoint = Vector3.zero;
            MinRewardDamage = 100000;
        }
    }
}
