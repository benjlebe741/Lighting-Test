using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lighting_Test
{
    internal class MovingSprite
    {
        public Rectangle body;
        public int[] xySpeed;
        public int[] xyDirection;
        public Color color;
        public int depth;

        public MovingSprite(Rectangle _body, int[] _xySpeed, int[] _xyDirection, Color _color, int _depth) 
        {
            body = _body;
            xySpeed = _xySpeed;
            xyDirection = _xyDirection;
            color = _color;
            depth = _depth;
        }
    }
}
