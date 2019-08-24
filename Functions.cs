using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogCat
{
    public class Functions
    {
        public static Vector2 PolarVector(float radius, float theta)
        {
            return (new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius);
        }
        public static float ToRotation(Vector2 v)
        {
            return (float)Math.Atan2((double)v.Y, (double)v.X);
        }
    }
}
