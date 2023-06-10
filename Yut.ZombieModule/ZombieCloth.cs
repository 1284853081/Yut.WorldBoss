using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yut.ZombieModule
{
    [Serializable]
    public class ZombieCloth
    {
        public ushort shirt;
        public ushort pants;
        public ushort hat;
        public ushort gear;
        public ZombieCloth() { }
        public ZombieCloth(ushort shirt, ushort pants, ushort hat, ushort gear)
        {
            this.shirt = shirt;
            this.pants = pants;
            this.hat = hat;
            this.gear = gear;
        }
    }
}
