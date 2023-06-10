using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yut.ZombieModule
{
    public class ZombieConfig
    {
        public List<ZombieRegion> regions;
        public ZombieConfig() { }
        public ZombieConfig(List<ZombieRegion> regions)
        {
            this.regions = regions ?? new List<ZombieRegion>();
        }
    }
}
