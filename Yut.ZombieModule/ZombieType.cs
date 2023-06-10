using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Yut.ZombieModule
{
    public class ZombieType : IEquatable<ZombieType>
    {
        private static List<ZombieType> list;
        [XmlAttribute]
        private string type;
        public string Type => type;
        public ZombieType(string type)
        {
            this.type = type;
        }
        public static ZombieType ACID => list[0];
        public static ZombieType BOSS_ALL => list[1];
        public static ZombieType BOSS_ELECTRIC => list[2];
        public static ZombieType BOSS_ELVER_STOMPER => list[3];
        public static ZombieType BOSS_FIRE => list[4];
        public static ZombieType BOSS_KUWAIT => list[5];
        public static ZombieType BOSS_MAGMA => list[6];
        public static ZombieType BOSS_NUCLEAR => list[7];
        public static ZombieType BOSS_SPIRIT => list[8];
        public static ZombieType BOSS_WIND => list[9];
        public static ZombieType BURNER => list[10];
        public static ZombieType CRAWLER => list[11];
        public static ZombieType DL_BLUE_VOLATILE => list[12];
        public static ZombieType DL_RED_VOLATILE => list[13];
        public static ZombieType FLANKER_FRIENDLY => list[14];
        public static ZombieType FLANKER_STALK => list[15];
        public static ZombieType MEGA => list[16];
        public static ZombieType NORMAL => list[17];
        public static ZombieType SPIRIT => list[18];
        public static ZombieType SPRINTER => list[19];
        static ZombieType()
        {
            list = new List<ZombieType>
            {
                new ZombieType("NORMAL"),
                new ZombieType("MEGA"),
                new ZombieType("CRAWLER"),
                new ZombieType("SPRINTER"),
                new ZombieType("FLANKER_FRIENDLY"),
                new ZombieType("FLANKER_STALK"),
                new ZombieType("BURNER"),
                new ZombieType("ACID"),
                new ZombieType("BOSS_ELECTRIC"),
                new ZombieType("BOSS_WIND"),
                new ZombieType("BOSS_FIRE"),
                new ZombieType("BOSS_ALL"),
                new ZombieType("BOSS_MAGMA"),
                new ZombieType("SPIRIT"),
                new ZombieType("BOSS_SPIRIT"),
                new ZombieType("BOSS_NUCLEAR"),
                new ZombieType("DL_BLUE_VOLATILE"),
                new ZombieType("DL_RED_VOLATILE"),
                new ZombieType("BOSS_ELVER_STOMPER"),
                new ZombieType("BOSS_KUWAIT"), 
            };
        }
        public static bool CheckValid(string typeStr, out ZombieType type)
        {
            type = list.Find(x => x.type.ToLower() == typeStr.ToLower());
            return type != null;
        }
        public static bool CheckValid(string typeStr,out byte type)
        {
            ZombieType zt = list.Find(x => x.type.ToLower() == typeStr.ToLower());
            type = zt;
            return zt != null;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as ZombieType);
        }
        public bool Equals(ZombieType other)
        {
            return other != null &&
                   type == other.type;
        }
        public override int GetHashCode()
        {
            return 34944597 + EqualityComparer<string>.Default.GetHashCode(type);
        }
        public static implicit operator byte(ZombieType type)
        {
            int ind = list.IndexOf(type);
            if (ind == -1)
                throw new ArgumentException("Wrong type of zombie");
            return (byte)(ind + 1);
        }
    }
}
