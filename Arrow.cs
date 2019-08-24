using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogCat
{
    public class Arrow
    {
        public Point position;
        public int direction;
        public Arrow(Point position, int direction)
        {
            this.position = position;
            this.direction = direction;
            Main.arrows.Add(this);
        }
    }
}
