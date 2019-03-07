using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace BreakernoidsGL
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D bgTexture;
        Paddle paddle;
        Ball ball;
        List<Block> blocks = new List<Block>();
        int ballWithPaddle = 0; // For colliding with paddle

        //power ups
        List<PowerUp> powerups = new List<PowerUp>();
        Random random = new Random();
        public double prob = 0.2;
        public bool ballCaught = false;

        List<Ball> balls = new List<Ball>();

        SoundEffect ballBounceSfx, ballHitSfx, deathSfx, powerupSFX;


        //blocks
        
        int[,] blockLayout = new int[,]{
        {5,5,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
        {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
        {4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
        }; 



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            bgTexture = Content.Load<Texture2D>("bg");

            paddle = new Paddle(this);
            paddle.LoadContent();
            paddle.position = new Vector2(512, 740);


            ball = new Ball(this);
            ball.LoadContent();
            ball.position = paddle.position;
            ball.position.Y -= ball.Height + paddle.Height;



            for (int i = 0; i < blockLayout.GetLength(1); i++)
            {
                for (int j = 0; j < blockLayout.GetLength(0); j++)
                { 
                    Block tempBlock = new Block((BlockColor)blockLayout[j, 0], this);
                    tempBlock.LoadContent();
                    tempBlock.position = new Vector2(64 + i * 64, 100 + j * 32);
                    blocks.Add(tempBlock);
                }
            }


            ballBounceSfx = Content.Load<SoundEffect>("ball_bounce");
            ballHitSfx = Content.Load<SoundEffect>("ball_hit");
            deathSfx = Content.Load<SoundEffect>("death");
            powerupSFX = Content.Load<SoundEffect>("powerup");




        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;


            float pX1 = paddle.position.X;
            paddle.Update(deltaTime);
            float ballStick = paddle.position.X - pX1; // For when ballCaugh figure out the x position of paddle to move ball with it


            foreach (Ball b in balls)
            {
                if (ballCaught)
                {
                    ball.position.X += ballStick;
                }
                b.Update(deltaTime);
                CheckCollisions(b);
            }


            
            ball.Update(deltaTime);



            
            CheckForPowerUps();
            RemovePowerUp();


            foreach(PowerUp p in powerups)
            {
                p.Update(deltaTime);
            }


            if (!balls.Any()) // https://stackoverflow.com/questions/18867180/check-if-list-is-empty-in-c-sharp
            {
                SpawnBall();
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DeepSkyBlue);


            // TODO: Add your drawing code here

            base.Draw(gameTime);
            spriteBatch.Begin();
            spriteBatch.Draw(bgTexture, new Vector2(0, 0), Color.White);
            paddle.Draw(spriteBatch);
            

            //loop for drawing blocks
            foreach (Block b in blocks)
            {
                b.Draw(spriteBatch);
            }


            //loop for drawing powerups
            foreach (PowerUp p in powerups)
            {
                p.Draw(spriteBatch);
            }

            foreach (Ball b in balls)
            {
                b.Draw(spriteBatch);
            }

            spriteBatch.End();

        }

        protected void CheckCollisions(Ball ball)
        {

            //Put checking if ball is caught
            if (ball.isCaught)
            {
                return; // If ball is caught return
            }
          

            float radius = ball.Width / 2;
            //Paddle collisions
            //Check to see if the ball hits the paddle
            if (ballWithPaddle == 0 &&
                (ball.position.X > (paddle.position.X - radius - paddle.Width / 2)) &&
                (ball.position.X < (paddle.position.X + radius + paddle.Width / 2)) &&
                (ball.position.Y < paddle.position.Y) &&
                (ball.position.Y > (paddle.position.Y - radius - paddle.Height / 2)))
            {

                Vector2 normal = -1.0f * Vector2.UnitY; // sets the vector2 normal to "up"

                float dist = paddle.Width + radius * 2; // Distance from left to right of paddle

                float ballLocation = ball.position.X - (paddle.position.X - radius - paddle.Width / 2); // Where the ball is at in this distance

                float pcnt = ballLocation / dist; // Percent between the two parts of paddle


                if(pcnt < 0.33f) // if percent is less than 33% (left half) make the vector2 normal those values
                {
                    normal = new Vector2(-0.196f, -0.981f);
                    
                }
                else if(pcnt > 0.66f) // If percent is greater than 66% (right half) make vector2 normal those values
                {
                    normal = new Vector2(0.196f, 0.981f);
                    
                }


                ball.direction = Vector2.Reflect(ball.direction, normal); // Changes direction of ball
                ballHitSfx.Play(); // Plays the hit sound effect
                ballWithPaddle = 20; // Makes ball with paddle 20 so the ball isn't constantly changing direction


                if (ballCaught)
                {
                    ball.isCaught = true; // Set ball caught to true in Ball class
                    if(pcnt < 0.5f) // If the percent is less than 50 (left side of paddle) send ball to left
                    {
                        ball.direction = new Vector2(-0.707f, -0.707f);

                    }
                    else // If ball is not on left side, send ball to the right
                    {
                        ball.direction = new Vector2(0.707f, -0.707f);
                    }
                }


            }
            else if(ballWithPaddle > 0)
            {
                ballWithPaddle--; // If ball with paddle is not 0 (not touching) count down back to zero
            }



            //Detroying blocks
            Block collidedBlock = null;
            foreach(Block b in blocks)
            {
                if ((ball.position.X > (b.position.X - b.Width / 2 - radius)) && (ball.position.X < (b.position.X + b.Width / 2 + radius)) && (ball.position.Y > (b.position.Y - b.Height / 2 - radius)) &&
                    (ball.position.Y < (b.position.Y + b.Height / 2 + radius))) //Checks to see if you hit any of the blocks
                {
                    collidedBlock = b; // sets the block hit as collidedBlock




                    break; //breaks from for each loop
                }

            }

            if(collidedBlock != null)
            {
                
                if((ball.position.Y < (collidedBlock.position.Y - collidedBlock.Height / 2)) || (ball.position.Y > (collidedBlock.position.Y + collidedBlock.Height / 2)))
                {
                    ball.direction.Y = -1.0f * ball.direction.Y; // changes the y value of ball.direction if hitting the top or bottom of block
                }
                else // else not hitting the top or bottom of block
                {
                    ball.direction.X = -1.0f * ball.direction.X; // changes x value of ball.direction if hitting anywhere else on the block
                }

                bool notGrey = collidedBlock.OnHit();
                if (notGrey)
                {
                    blocks.Remove(collidedBlock); // removes the hit block from the array

                    if(random.NextDouble() < prob) // if random number is less than probability (20%) then spawn a power up
                    {
                        SpawnPowerUp(collidedBlock.position);
                    }

                }
                
                ballHitSfx.Play(); // Plays ball hit sound

            }





            // Wall collisions
            

            if (Math.Abs(ball.position.X - 32) < radius)
            {
                ball.direction.X = -1.0f * ball.direction.X;
                ballBounceSfx.Play();
            }

            else if (Math.Abs(ball.position.X - 992) < radius)
            {
                ball.direction.X = -1.0f * ball.direction.X;
                ballBounceSfx.Play();
            }

            else if (Math.Abs(ball.position.Y - 32) < radius)
            {
                ball.direction.Y = -1.0f * ball.direction.Y;
                ballBounceSfx.Play();
            }

            else if (ball.position.Y > (768 + radius))
            {
               
                RemoveBalls();
                
            }

             
            

        }

        protected void LoseLife()
        {
            
            paddle.position = new Vector2(512, 740);
            ball.position = paddle.position;
            ball.position.Y -= ball.Height + paddle.Height;
            ball.direction = new Vector2(0.707f, -0.707f);
            paddle.ChangeTexture("paddle");
            ballCaught = false;
            deathSfx.Play();
            SpawnBall();

        }




        protected void RemovePowerUp()
        {
            for(int i=powerups.Count - 1; i>=0; i--)
            {
                if (powerups[i].shouldRemove)
                {
                    powerups.RemoveAt(i);
                    Console.WriteLine("Power up removed");
                }
            }
        }




        protected void SpawnPowerUp(Vector2 position)
        {
            int powerType = random.Next(3);
            PowerUp p = new PowerUp((Power)powerType, this);
            p.LoadContent();
            p.position = position;
            powerups.Add(p);
        }





        protected void CheckForPowerUps()
        {
            Rectangle paddleR = paddle.BoundingRect;
            foreach (PowerUp p in powerups)
            {
                Rectangle powerUpR = p.BoundingRect;
                if (paddleR.Intersects(powerUpR)) // If paddle intersects with rec of powerup
                {
                    p.shouldRemove = true; // Removes power up when intersecting
                    powerupSFX.Play();
                    switch (p.power)
                    {
                        case (Power.BallCatch):
                            ballCaught = true;
                            break;
                        case (Power.PaddleSize):
                            paddle.ChangeTexture("paddle_long");
                            break;
                        case (Power.MultiBall):
                            SpawnBall();
                            break;
                    }
                }
            }

        }


        
        protected void SpawnBall()
        {
            Console.WriteLine("Ball Spawned");
            Ball ball = new Ball(this);
            ball.LoadContent();
            ball.position = paddle.position;
            ball.position.Y -= ball.Height + paddle.Height;
            balls.Add(ball);
        }


        protected void RemoveBalls()
        {
            for (int i = balls.Count - 1; i>= 0; i--)
            {
                if (balls[i].shouldRemove)
                {
                    balls.RemoveAt(i);
                }
            }

            if (balls.Count == 0)
            {
                LoseLife();
            }

        }


    }
}
