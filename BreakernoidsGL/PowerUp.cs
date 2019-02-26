using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BreakernoidsGL
{

    public enum Power
    {
        BallCatch = 0,
        MultiBall,
        PaddleSize
    }

    public class PowerUp : GameObject
    {
        public Power power;
        public float speed = 300;
        public bool shouldRemove = false;

        public PowerUp(Power myPower, Game myGame):
            base(myGame)
        {
            power = myPower;
            switch (power)
            {
                case (Power.BallCatch):
                    textureName = "powerup_c";
                    break;
                case (Power.MultiBall):
                    textureName = "powerup_b";
                    break;
                case (Power.PaddleSize):
                    textureName = "powerup_p";
                    break;



            }

        }



        public override void Update(float deltaTime)
        {
            position.Y += speed * deltaTime; // moves the position down the screen
            if(position.Y > 1240)
            {
                shouldRemove = true; // removes powerup if it falls below screen
            }
            base.Update(deltaTime);
        }
    }
}
