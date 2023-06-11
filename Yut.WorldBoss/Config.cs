using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Yut.WorldBoss
{
    public class Config :  IRocketPluginConfiguration
    {
        public byte RefreshHour;
        public byte RefreshMinute;
        public ushort UIKey;
        public string BossIcon;
        public string BossName;
        public ushort PhantomSeconds;
        public ItemPair Ticket;
        public ushort PrepareSeconds;
        public ushort FightingSeconds;
        public ushort RewardSeconds;
        public uint MinRewardDamage;
        public float LeaderboardRefreshSeconds;
        public byte PrepareNoticeSeconds;
        public List<string> PhantomType;
        public ZombieRegion Region;
        public List<RewardInterval> Rewards;
        public Vector3 BossRefreshPoint;
        public void LoadDefaults()
        {
            RefreshHour = 0;
            RefreshMinute = 0;
            UIKey = 40010;
            BossIcon = "Your boss icon URL";
            BossName = "世界BOSS";
            PhantomSeconds = 10;
            Ticket = new ItemPair(14, 1);
            PrepareSeconds = 30;
            FightingSeconds = 1800;
            RewardSeconds = 60;
            MinRewardDamage = 100;
            LeaderboardRefreshSeconds = 1f;
            PrepareNoticeSeconds = 10;
            PhantomType = new List<string>()
            {
                "ACID",
                "BOSS_ELECTRIC",
                "BOSS_WIND",
                "BOSS_FIRE",
                "BOSS_ALL",
                "BOSS_MAGMA",
                "SPIRIT",
                "BOSS_SPIRIT",
                "BOSS_NUCLEAR",
                "DL_BLUE_VOLATILE",
                "DL_RED_VOLATILE",
                "BOSS_ELVER_STOMPER",
                "BOSS_KUWAIT"
            };
            List<Minion> minions = new List<Minion>()
            {
                new Minion("NORMAL", 100),
                new Minion("SPRINTER", 50)
            };
            Region = new ZombieRegion("BOSS_WIND",10000,5,minions,"BOSS","Minion");
            Rewards = new List<RewardInterval>()
            {
                new RewardInterval(1,1,new List<ItemPair>(){ new ItemPair(14,1)}),
                new RewardInterval(2,2,new List<ItemPair>(){ new ItemPair(14,1)}),
                new RewardInterval(3,3,new List<ItemPair>(){ new ItemPair(14,1)}),
                new RewardInterval(4,10,new List<ItemPair>(){ new ItemPair(14,1)}),
                new RewardInterval(11,100,new List<ItemPair>(){ new ItemPair(14,1)}),
            };
            BossRefreshPoint = Vector3.zero;
        }
    }
}
