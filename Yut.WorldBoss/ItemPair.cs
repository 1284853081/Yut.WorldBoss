namespace Yut.WorldBoss
{
    public class ItemPair
    {
        public ushort Id;
        public byte Count;
        public byte Chance;
        public ItemPair() { }
        public ItemPair(ushort id, byte count, byte chance)
        {
            Id = id;
            Count = count;
            Chance = chance;
        }
    }
}
