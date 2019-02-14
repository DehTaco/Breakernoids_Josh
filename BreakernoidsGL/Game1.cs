using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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


            for (int i=0; i<15; i++)
            {
                Block tempBlock = new Block(this);
                tempBlock.LoadContent();
                tempBlock.position = new Vector2(64 + i * 64, 200);
                blocks.Add(tempBlock);
            }


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
            paddle.Update(deltaTime);
            ball.Update(deltaTime);
            CheckCollisions();

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
            ball.Draw(spriteBatch);

            foreach (Block b in blocks)
            {
                b.Draw(spriteBatch);
            }

            spriteBatch.End();

            
        }

        protected void CheckCollisions()
        {
            float radius = ball.Width / 2;
            //Paddle collisions
            //Check to see if the ball hits the paddle
            if (ballWithPaddle == 0 &&
                (ball.position.X > (paddle.position.X - radius - paddle.Width / 2)) &&
                (ball.position.X < (paddle.position.X + radius + paddle.Width / 2)) &&
                (ball.position.Y < paddle.position.Y) &&
                (ball.position.Y > (paddle.position.Y - radius - paddle.Height / 2)))
            {

                Vector2 normal = -1.0f * Vector2.UnitY; // sets the normal to "up"

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

                ballWithPaddle = 20; // Makes ball with paddle 20 so the ball isn't constantly changing direction

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

                blocks.Remove(collidedBlock); // removes the hit block from the array

            }





            // Wall collisions
            

            if (Math.Abs(ball.position.X - 32) < radius)
            {
                ball.direction.X = -1.0f * ball.direction.X;
            }

            else if (Math.Abs(ball.position.X - 992) < radius)
            {
                ball.direction.X = -1.0f * ball.direction.X;
            }

            else if (Math.Abs(ball.position.Y - 32) < radius)
            {
                ball.direction.Y = -1.0f * ball.direction.Y;
            }

            else if (ball.position.Y > (768 + radius))
            {
                LoseLife();
            }


            

        }

        protected void LoseLife()
        {
            
            paddle.position = new Vector2(512, 740);
            ball.position = paddle.position;
            ball.position.Y -= ball.Height + paddle.Height;
            ball.direction = new Vector2(0.707f, -0.707f);
        }


        protected void Remove()
        {
            // Remove the block once the ball hits it
        }
        
    }
}
