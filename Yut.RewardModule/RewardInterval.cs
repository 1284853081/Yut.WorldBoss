using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yut.RewardModule
{
    [Serializable]
    public class RewardInterval
    {
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
    }
}
