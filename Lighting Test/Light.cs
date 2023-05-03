using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lighting_Test
{
    internal class Light
    {
        //The physical light source: really worrying about the x and y coordinates of the light. (the light doesnt change based on size, so width and height can be 0).
        public Rectangle body;

        //The color of the light
        public int[] rgbAffectors = new int[3];

        //The depth of the light
        public int depth;

        //Numbers affecting the circles around the light:
            //When does the rgb value get lighter than what it normally would be?
        public int lightenPoint;
            //When does the rgb value top out at brightness wise?    
        public int maxBright;
            //When does the rgb value get darker than what it normally would be?
        public int darkenPoint;
            //When does the rgb value skip completely to black?
        public int blackPoint;

        //Flicker speed
        public int flickerSpeed;

        public double[] rgbStorage = new double[3] { 0, 0, 0 };

        //CREATING PHYSICAL LIGHTS:
        public Light(Rectangle _body, int[] _rgbAffectors, int _lightenPoint, int _maxBright, int _darkenPoint, int _blackPoint, int _flickerSpeed, int _depth) 
        {
            body = _body;
            
            rgbAffectors = _rgbAffectors;

            depth = _depth;

            lightenPoint = _lightenPoint;
            maxBright = _maxBright;
            darkenPoint = _darkenPoint;
            blackPoint = _blackPoint;
            
            flickerSpeed = _flickerSpeed;
        }
    }
}
