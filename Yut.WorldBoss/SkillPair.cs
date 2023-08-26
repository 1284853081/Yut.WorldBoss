using System.Collections.Generic;

namespace Yut.WorldBoss
{
    public class SkillPair
    {
        private static readonly List<SkillPair> defaultSkillPairs = new List<SkillPair>()
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
        public static List<SkillPair> DefaultSkillPair => new List<SkillPair>(defaultSkillPairs);
        public string Key;
        public string SkillName;
        public float SkillRange;
        public SkillPair() { }
        public SkillPair(string key, string skillName, float skillRange)
        {
            Key = key;
            SkillName = skillName;
            SkillRange = skillRange;
        }
    }
}
