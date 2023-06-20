using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yut.WorldBoss
{
    public class Refresh : IEquatable<Refresh>
    {
        private static readonly Refresh zero = new Refresh(0, 0, "Normal");
        public static Refresh Zero => zero;
        public byte Hour;
        public byte Minute;
        public string Mode;
        public Refresh() { }
        public Refresh(byte hour, byte minute,string mode)
        {
            Hour = hour;
            Minute = minute;
            Mode = mode;
        }
        public override string ToString()
            => $"{Hour}:{Minute}";
        public override bool Equals(object obj)
        {
            return Equals(obj as Refresh);
        }
        public bool Equals(Refresh other)
        {
            return other != null &&
                   Hour == other.Hour &&
                   Minute == other.Minute &&
                   Mode == other.Mode;
        }
        public override int GetHashCode()
        {
            int hashCode = -1199057214;
            hashCode = hashCode * -1521134295 + Hour.GetHashCode();
            hashCode = hashCode * -1521134295 + Minute.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Mode);
            return hashCode;
        }
        public static implicit operator TimeSpan(Refresh time)
            => new TimeSpan(time.Hour,time.Minute,0);
        public static bool operator <(Refresh l, Refresh r)
            => l.Hour * 60 + l.Minute < r.Hour * 60 + r.Minute;
        public static bool operator >(Refresh l, Refresh r)
            => l.Hour * 60 + l.Minute > r.Hour * 60 + r.Minute;
    }
}
