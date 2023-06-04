using System;
using System.Collections.Generic;
using System.Drawing;
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
        public List<Entity> entities = new List<Entity>();
        public List<Color> OverlayColors = new List<Color>();

        public int[] vineInfo = new int[9]
            {
        50, //largestSize
        1,  //largestSubtraction
        10, //redAddition
        50, //greenAddition
        20, //blueAddition
        5,  //bioluminescenceSize

        4,  //sampleSuppressor

        45, //chanceOfSpawn
        90, //chanceOfRemoval
    };

        public Level(List<int> _tileSprites, List<int> _depths, List<Light> _lights, List<Entity> _entities, int[] _vineInfo) 
        {
            lights = _lights;
            tileSprites = _tileSprites;
            depths = _depths;
            entities = _entities;
            
            vineInfo = _vineInfo;
        }
    }
}
