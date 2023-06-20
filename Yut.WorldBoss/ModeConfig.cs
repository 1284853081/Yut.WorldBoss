using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yut.WorldBoss
{
    public class ModeConfig
    {
        private static readonly ModeConfig defaultConfig = new ModeConfig()
        {
            Mode = "normal",
            Region = new ZombieRegion("BOSS_WIND", 5000000, 10, Minion.Minions, "WorldBoss", "WorldBossMini"),
            StateConfig = new StateConfig("世界BOSS", new ItemPair(14, 1, 0), 30, 1800, 60, 50000, 10, RewardInterval.DefaultRewards, 10),
            SkillConfig = new SkillConfig(12, 10, 10, 10, 50, 70, 100, 50000, SkillPair.DefaultSkillPair)
        };
        public static ModeConfig Default => defaultConfig;
        public string Mode;
        public ZombieRegion Region;
        public StateConfig StateConfig;
        public SkillConfig SkillConfig;
        public ModeConfig() { }
        public ModeConfig(string mode, ZombieRegion region, StateConfig stateConfig, SkillConfig skillConfig)
        {
            Mode = mode;
            Region = region;
            StateConfig = stateConfig;
            SkillConfig = skillConfig;
        }
    }
}
