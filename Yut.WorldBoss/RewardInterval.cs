using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yut.WorldBoss
{
    [Serializable]
    public class RewardInterval
    {
        private static readonly List<RewardInterval> defaultRewards = new List<RewardInterval>()
        {
            new RewardInterval(1,1,new List<ItemPair>(){ new ItemPair(14,1,100)}),
            new RewardInterval(2,2,new List<ItemPair>(){ new ItemPair(14,1,100)}),
            new RewardInterval(3,3,new List<ItemPair>(){ new ItemPair(14,1,100)}),
            new RewardInterval(4,10,new List<ItemPair>(){ new ItemPair(14,1,100)}),
            new RewardInterval(11,100,new List<ItemPair>(){ new ItemPair(14,1,100)}),
        };
        public static List<RewardInterval> DefaultRewards => new List<RewardInterval>(defaultRewards);
        public byte Start;
        public byte End;
        public List<ItemPair> Rewards;
        public RewardInterval() { }
        public RewardInterval(byte start, byte end, List<ItemPair> rewards)
        {
            Start = start;
            End = end;
            Rewards = rewards;
        }
        public bool InInterval(byte num)
        {
            if (Start == End)
                return num == Start;
            return num >= Start && num <= End;
        }
    }
}
