using Rocket.API;
using System.Collections.Generic;
using UnityEngine;

namespace Yut.WorldBoss
{
    public class Config : IRocketPluginConfiguration
    {
        public ushort HKey;
        public ushort NKey;
        public float LeaderboardRefreshSeconds;
        public byte PrepareNoticeSeconds;
        public Vector3 BossRefreshPoint;
        public Color SkillNoticeColor;
        public List<Refresh> Refreshs;
        public List<ModeConfig> ModeConfigs;
        public void LoadDefaults()
        {
            HKey = 34309;
            NKey = 34310;
            LeaderboardRefreshSeconds = 1;
            PrepareNoticeSeconds = 10;
            BossRefreshPoint = Vector3.zero;
            SkillNoticeColor = Color.blue;
            Refreshs = new List<Refresh>()
            {
                new Refresh(0,0,"normal")
            };
            List<Minion> minions = new List<Minion>()
            {
                new Minion("NORMAL", 7500),
                new Minion("SPRINTER", 5000),
                new Minion("SPIRIT", 7500),
                new Minion("CRAWLER", 12000),
                new Minion("BURNER", 7500),
                new Minion("MEGA", 7500),
                new Minion("FLANKER_STALK", 10000),
                new Minion("FLANKER_FRIENDLY", 10000),
                new Minion("DL_RED_VOLATILE", 10000),
                new Minion("DL_BLUE_VOLATILE", 10000),
                new Minion("BOSS_WIND", 20000),
                new Minion("BOSS_SPIRIT", 20000),
                new Minion("BOSS_NUCLEAR", 20000),
                new Minion("BOSS_MAGMA", 20000),
                new Minion("BOSS_KUWAIT", 30000),
                new Minion("BOSS_FIRE", 20000),
                new Minion("BOSS_ELVER_STOMPER", 20000),
                new Minion("BOSS_ELECTRIC", 20000),
                new Minion("BOSS_ALL", 60000),
                new Minion("ACID", 7500),
            };
            List<RewardInterval> rewards = new List<RewardInterval>()
            {
                new RewardInterval(1,1,new List<ItemPair>(){ new ItemPair(14,1,100)}),
                new RewardInterval(2,2,new List<ItemPair>(){ new ItemPair(14,1,100)}),
                new RewardInterval(3,3,new List<ItemPair>(){ new ItemPair(14,1,100)}),
                new RewardInterval(4,10,new List<ItemPair>(){ new ItemPair(14,1,100)}),
                new RewardInterval(11,100,new List<ItemPair>(){ new ItemPair(14,1,100)}),
            };
            List<SkillPair> skillPairs = new List<SkillPair>()
            {
                new SkillPair("burn","烈焰灼烧", 5),
                new SkillPair("thud","巨石震击", 50),
                new SkillPair("stomp","闪现冲击", 100),
                new SkillPair("explosion","爆炸袭击",100),
                new SkillPair("explosion2","环爆",25),
                new SkillPair("breath","欲火焚身",100),
                new SkillPair("virus","病毒洗礼",100),
                new SkillPair("fly","升天",100),
                new SkillPair("acid","酸液边界",30),
                new SkillPair("acid2","酸液星芒",60),
                new SkillPair("flash","致命闪光",30),
                new SkillPair("heal","治疗",0),
                new SkillPair("boulder","陨石",20),
                new SkillPair("boulder2","陨石2",20),
                new SkillPair("baptism","圣魂洗礼",100),
                new SkillPair("baptism2","灵魂浩劫",100),
                new SkillPair("shield","反弹",0),
            };
            ModeConfigs = new List<ModeConfig>()
            {
                new ModeConfig()
                {
                    Mode = "normal",
                    Region = new ZombieRegion("BOSS_WIND",5000000,10,minions,"WorldBoss","WorldBossMini"),
                    StateConfig = new StateConfig("世界BOSS",new ItemPair(14,1,100),30,1800,60,50000,10,rewards,8),
                    SkillConfig = new SkillConfig(12, 10, 10, 10, 50, 70, 100, 50000, skillPairs)
                }
            };
        }
    }
}
