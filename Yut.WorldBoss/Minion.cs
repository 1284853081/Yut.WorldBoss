using System;
using System.Collections.Generic;

namespace Yut.WorldBoss
{
    [Serializable]
    public class Minion
    {
        private static readonly List<Minion> _minions = new List<Minion>()
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
        public static List<Minion> Minions => new List<Minion>(_minions);
        public string type;
        public uint Health;
        public Minion() { }
        public Minion(string type, uint health)
        {
            Health = health;
            this.type = type;
        }
    }
}
