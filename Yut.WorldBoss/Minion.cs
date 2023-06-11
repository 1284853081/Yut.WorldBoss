using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yut.WorldBoss
{
    [Serializable]
    public class Minion
    {
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
