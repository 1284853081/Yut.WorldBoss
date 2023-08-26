using System.Collections.Generic;

namespace Yut.WorldBoss
{
    public class StateConfig
    {
        public string BossName;
        public ItemPair Ticket;
        public ushort PrepareSeconds;
        public ushort FightingSeconds;
        public ushort RewardSeconds;
        public uint MinRewardDamage;
        public float SkillRefreshSeconds;
        public List<RewardInterval> Rewards;
        public byte MaxPlayers;
        public StateConfig() { }
        public StateConfig(string bossName, ItemPair ticket, ushort prepareSeconds, ushort fightingSeconds, ushort rewardSeconds, uint minRewardDamage, float skillRefreshSeconds, List<RewardInterval> rewards, byte maxPlayers)
        {
            BossName = bossName;
            Ticket = ticket;
            PrepareSeconds = prepareSeconds;
            FightingSeconds = fightingSeconds;
            RewardSeconds = rewardSeconds;
            MinRewardDamage = minRewardDamage;
            SkillRefreshSeconds = skillRefreshSeconds;
            Rewards = rewards;
            MaxPlayers = maxPlayers;
        }
    }
}
