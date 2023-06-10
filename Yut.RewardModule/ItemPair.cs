using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yut.RewardModule
{
    public class ItemPair
    {
        public ushort Id;
        public byte Count;
        public ItemPair() { }
        public ItemPair(ushort id, byte count)
        {
            Id = id;
            Count = count;
        }
    }
}
