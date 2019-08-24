using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogCat
{
    public class Animal
    {
        public int direction;
        public Vector2 position;
        public int horizontalOffset = 0;
        public bool special = false; //Zoey
        const int upperAnimalLimit = -24; // the top offscreen region where cats spawn
        const int lowerAnimalLimit = 504; // the bottom offscreen region where dogs spawn
        public Animal(int direction, int lane)
        {
            float yPos = lowerAnimalLimit;
            this.direction = direction;
            if (direction == 1)
            {
                yPos = upperAnimalLimit;
                Main.sounds[0].Play(); //meow
            }
            else
            {
                Main.sounds[1].Play(); //woof woof
            }

            position = new Vector2(lane * 48 + 24, yPos);
            if(Main.random.Next(20) == 0) //if it's a cat this will cause a 1/20 chance of making it Zoey
            {
                special = true;
            }

            Main.animals.Add(this);
        }
        public void Update()
        {
            //arrow logic
            foreach (Arrow arrow in Main.arrows)
            {
                if (position == (arrow.position.ToVector2() * Main.SquareSize + new Vector2(24, 24)))
                {
                    horizontalOffset = Main.SquareSize * arrow.direction;//trigger movement
                }

            }

            //movement logic
            if (horizontalOffset>0)
            {
                position.X++;
                horizontalOffset--;
            }
            else if(horizontalOffset<0)
            {
                position.X--;
                horizontalOffset++;
            }
            else
            {
                position.Y += direction;
            }

            //loop around the screen
            if(position.X > Main.screenSize.X)
            {
                position.X -= Main.screenSize.X;
            }
            if (position.X < 0)
            {
                position.X += Main.screenSize.X;
            }

            //when an animal makes it across
            if((direction ==1 && position.Y > lowerAnimalLimit) || (direction == -1 && position.Y < upperAnimalLimit))
            {
                Main.animals.Remove(this);
                Main.score++;
            }

            //checks if a dog and cat hit each other
            foreach(Animal animal in Main.animals)
            {
                if(direction == -1 && animal.direction == 1 && (animal.position -position).Length()<6)
                {
                    Main.GameOver = true;
                    Main.BadDog = this;
                    Main.ScaredKitty = animal;
                    Main.arrows.Clear();

                }
            }
           

        }
    }
}
