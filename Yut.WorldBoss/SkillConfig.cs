using System.Collections.Generic;

namespace Yut.WorldBoss
{
    public class SkillConfig
    {
        public ushort ShieldSeconds;
        public byte FireDamage;
        public byte ShieldDamage;
        public byte VirusDamage;
        public byte FlyDamage;
        public ushort ExplosionDamage;
        public byte BaptismDamage;
        public uint HealAmount;
        public List<SkillPair> SkillPairs;
        public SkillConfig() { }
        public SkillConfig(ushort shieldSeconds, byte fireDamage, byte shieldDamage, byte virusDamage, byte flyDamage, ushort explosionDamage, byte baptismDamage, uint healAmount, List<SkillPair> skillPairs)
        {
            ShieldSeconds = shieldSeconds;
            FireDamage = fireDamage;
            ShieldDamage = shieldDamage;
            VirusDamage = virusDamage;
            FlyDamage = flyDamage;
            ExplosionDamage = explosionDamage;
            BaptismDamage = baptismDamage;
            HealAmount = healAmount;
            SkillPairs = skillPairs;
        }
    }
}
