
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SpriteFontPlus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DogCat
{

    public class Main : Game
    {
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Random random = new Random();
        public static Main instance;
        public static SpriteFont font;

        public const int SquareSize = 48;
        public static int score = 0;
        public static bool GameOver = false;
        static Point boardSize = new Point(10, 10);
        public static Vector2 screenSize = new Vector2(boardSize.X * SquareSize, (boardSize.Y+1) * SquareSize);

        bool notPressed = true; //used for mouse clicking logic
        bool paused = false;
        bool spaceOff = true; // used for space pressing logic
        int gameOverTimer; //used to keep track of the animated ending
        int difficulty = 0;
        int removals = 3;
        int maxRemovals = 3;
        int timer = 0;
        int direction = 1; //used to alternate cats and dogs spawning

        Point CurrentTile;
        MouseState mouse;
        Point mouseStart;

        Texture2D Grass;
        Texture2D cursor;
        Texture2D[] AnimalSprites = new Texture2D[3];
        Texture2D UI;
        Texture2D ArrowSprite;

        public static List<Arrow> arrows = new List<Arrow>();
        public static List<Animal> animals = new List<Animal>();
        public static Animal BadDog; //used for ending
        public static Animal ScaredKitty; //used for ending
        public static SoundEffect[] sounds = new SoundEffect[5];

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = (int)screenSize.X;
            graphics.PreferredBackBufferHeight = (int)screenSize.Y;
            //graphics.IsFullScreen = true;
        }
        
        protected override void Initialize()
        {
            base.Initialize();
        }
       
        
        protected override void LoadContent()
        {
            instance = this;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //here we use spritefontplus to load a font
            font = TtfFontBaker.Bake(File.ReadAllBytes(@"C:\\Windows\\Fonts\arial.ttf"), 25, 1024, 1024, new[] { CharacterRange.BasicLatin, CharacterRange.Latin1Supplement, CharacterRange.LatinExtendedA, CharacterRange.Cyrillic }).CreateSpriteFont(GraphicsDevice);
            //load sprites
            AnimalSprites[0] = Content.Load<Texture2D>("Dog");
            AnimalSprites[1] = Content.Load<Texture2D>("Cat");
            AnimalSprites[2] = Content.Load<Texture2D>("Zoey");
            Grass = Content.Load<Texture2D>("Grass");
            cursor = Content.Load<Texture2D>("Cursor");
            ArrowSprite = Content.Load<Texture2D>("Arrow");
            UI = Content.Load<Texture2D>("UI");
            //load sounds
            sounds[0] = Content.Load<SoundEffect>("Meow");
            sounds[1] = Content.Load<SoundEffect>("WoofWoof");
            sounds[2] = Content.Load<SoundEffect>("Growl");
            sounds[3] = Content.Load<SoundEffect>("Bark");
            sounds[4] = Content.Load<SoundEffect>("Panic");
        }


        protected override void UnloadContent()
        {

        }

        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            
            mouse = Mouse.GetState();
            //pause logic
            if(GameOver)
            {
                paused = true;
            }
            else if(Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if(spaceOff)
                {
                    paused = !paused;
                }
                spaceOff = false;
            }
            else
            {
                spaceOff = true;
            }
            //arrow placing/removing logic
            if(mouse.LeftButton == ButtonState.Pressed)
            {
                if(notPressed)
                {
                    mouseStart = mouse.Position;
                    CurrentTile = new Point(mouse.Position.X / SquareSize, mouse.Position.Y / SquareSize);
                }
                notPressed = false;
            }
            else if(!notPressed)
            {
                notPressed = true;
                bool noArrow = true;
                foreach (Arrow arrow in arrows)
                {
                    if (arrow.position == CurrentTile)
                    {
                        noArrow = false;
                        break;
                    }
                }
                if (noArrow)
                {
                    if (mouse.Position.X < mouseStart.X)
                    {
                        new Arrow(CurrentTile, -1);
                    }
                    else if(mouse.Position.X > mouseStart.X)
                    {
                        new Arrow(CurrentTile, 1);
                    }
                }
            }
            else if(mouse.RightButton == ButtonState.Pressed && removals >0)
            {
                CurrentTile = new Point(mouse.Position.X / SquareSize, mouse.Position.Y / SquareSize);
                for(int i = 0; i < arrows.Count; i++)
                {
                    if(arrows[i].position == CurrentTile)
                    {
                        arrows.RemoveAt(i);
                        removals--;
                    }
                }
            }
            //active updating logic
            if(!paused)
            {
                timer++;
                //modify diffculty
                if(timer % 300 == 0 && difficulty < 60)
                {
                    difficulty++;
                    
                }
                //restore removals over time
                if( timer % 1200 == 0)
                {
                    if (removals < maxRemovals)
                    {
                        removals++;
                    }
                }
                //spawning logic
                if (random.Next(75 - difficulty) == 0)
                {
                    int lane = random.Next(10);
                    direction = (direction == 1 ? -1 : 1);
                    bool valid = true;
                    foreach(Animal animal in animals)
                    {
                        if((int)animal.position.X/SquareSize == lane && ((animal.position.Y < 4 * SquareSize && direction == 1) || (animal.position.Y > 6 * SquareSize && direction == -1)))
                        {
                            valid = false;
                        }
                    }
                    if(valid)
                    {
                        new Animal(direction, lane);
                    }
                    
                }
                //makes animals move see Animal.cs
                for(int a = 0; a < animals.Count; a++)
                {
                    Animal animal = animals[a];
                    animal.Update();
                    
                }
            }
            //The ending
            if(GameOver)
            {
                if(gameOverTimer > 120)
                {
                    if(Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        arrows.Clear();
                        animals.Clear();
                        timer = 0;
                        difficulty = 0;
                        gameOverTimer = 0;
                        GameOver = false;
                        score = 0;
                        removals = 3;
                    }
                    if(gameOverTimer % 30 == 0)
                    {
                        sounds[3].Play();
                    }
                    foreach(Animal animal in animals)
                    {
                        float maxDistance = 1000;
                        Animal target = null;
                        foreach(Animal other in animals)
                        {
                            if(other.direction != animal.direction && (animal.position-other.position).Length()<maxDistance)
                            {
                                maxDistance = (animal.position - other.position).Length();
                                target = other;
                            }
                        }
                        if(target!= null)
                        {
                            float rotation = Functions.ToRotation(animal.position - target.position);
                            animal.position += Functions.PolarVector(2 * animal.direction, rotation);
                        }
                    }
                }
                else
                {
                    if (gameOverTimer == 1)
                    {
                        sounds[2].Play();
                    }
                    if (gameOverTimer < 60)
                    {
                        ScaredKitty.position.Y -= 1;
                        if (gameOverTimer == 30)
                        {
                            sounds[4].Play();
                        }
                    }
                    else
                    {
                        ScaredKitty.position.Y -= 8;
                    }
                }
               
                gameOverTimer++;

            }
            base.Update(gameTime);
        }



        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            //draw the green tiles
            for (int x = 0; x < boardSize.X / 2; x++)
            {
                for (int y = 0; y < boardSize.Y / 2; y++)
                {
                    spriteBatch.Draw(Grass, new Vector2(x * 96, y * 96));
                }
            }

            //draw all the arrows
            foreach (Arrow arrow in arrows)
            {
                spriteBatch.Draw(ArrowSprite, new Vector2(arrow.position.X * 48, arrow.position.Y * 48), effects: (arrow.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally));
            }

            //draw all the animals
            foreach (Animal animal in animals)
            {
                //pick sprite
                Texture2D face = AnimalSprites[0];
                if (animal.direction == 1)
                {

                    if (animal.special)
                    {
                        face = AnimalSprites[2];
                    }
                    else
                    {
                        face = AnimalSprites[1];
                    }
                }
                spriteBatch.Draw(face, animal.position - new Vector2(face.Width, face.Height) * .5f + //draw animal
                    (gameOverTimer< 120 && animal == BadDog ?  new Vector2(Main.random.Next(-4, 4), Main.random.Next(-4, 4)) : Vector2.Zero) //makes the bad dog shake at the end
                    );
            }

            //cursor
            spriteBatch.Draw(cursor, position: mouse.Position.ToVector2());
            //black region at the bottom
            spriteBatch.Draw(UI, position: Vector2.UnitY * (screenSize.Y - UI.Height));

            //middle text
            if (paused)
            {
                string pauseText = "Pawsed";
                if(gameOverTimer>120)
                {
                    pauseText = "Press Sacebar to restart.";
                }
                else if(GameOver)
                {
                    pauseText = "Game Over!";
                }
                Vector2 textSize = font.MeasureString(pauseText);
                spriteBatch.DrawString(font, pauseText, new Vector2((screenSize.X - textSize.X) * .5f, screenSize.Y - 24 - (textSize.Y * .5f)), Color.Brown);
            }
            if (gameOverTimer < 120)
            {
                //score display
                string dispalyScore = "Score: " + score;
                Vector2 scoreSize = font.MeasureString(dispalyScore);
                spriteBatch.DrawString(font, dispalyScore, new Vector2((screenSize.X * .8f) - (scoreSize.X * .5f), screenSize.Y - 24 - (scoreSize.Y * .5f)), Color.Brown);

                //Removal display
                string dispalyRemovals = "Removals: " + removals;
                Vector2 removalSize = font.MeasureString(dispalyRemovals);
                spriteBatch.DrawString(font, dispalyRemovals, new Vector2((screenSize.X * .2f) - (removalSize.X * .5f), screenSize.Y - 24 - (removalSize.Y * .5f)), Color.Brown);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
    

}
