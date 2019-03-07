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



    public class Ball : GameObject
    {
        public float speed = 500;
        public Vector2 direction = new Vector2(0.707f, -0.707f);
        public bool isCaught;
        public int onPaddle = 0;
        public bool shouldRemove = false;


        public Ball(Game myGame) :
            base(myGame)
        {
            textureName = "ball";
        }

        public override void Update(float deltaTime)
        {
            

            KeyboardState keyState = Keyboard.GetState();
            if (!isCaught) 
            {
                position += direction * speed * deltaTime;
                if (onPaddle > 0)
                {
                    onPaddle--;
                }
            }
            else 
            {
                if (keyState.IsKeyDown(Keys.Space))
                {
                    isCaught = false;
                }
                

            }

            if(position.Y > 768)
            {
                shouldRemove = true;
            }


            base.Update(deltaTime);
        }




    }
}