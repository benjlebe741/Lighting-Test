using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lighting_Test
{
    internal class Level
    {
        public List<Light> lights = new List<Light>();
        public List<int> tileSprites = new List<int>();
        public List<int> depths = new List<int>();

        public Level(List<int> _tileSprites, List<int> _depths, List<Light> _lights) 
        {
            lights = _lights;
            tileSprites = _tileSprites;
            depths = _depths;
        }
    }
}
