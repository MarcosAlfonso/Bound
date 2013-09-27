using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ToBeDetermined
{
    public static class Input
    {
        public static MouseState lastMs;
        public static KeyboardState lastKs;

        //Update Everything
        public static void Update(float amount)
        {
            Keyboard(amount);
            Mouse(amount);
        }

        //Keyboard Input
        public static void Keyboard(float amount)
        {
            KeyboardState curKs = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            Game1.player.KeyboardInput(curKs);

            if (curKs.IsKeyDown(Keys.P))
                Game1.PhysList.Add(new PhysModel(new Vector3(0,20,0), new Vector3(1,2,1), 1 ));
            if (curKs.IsKeyDown(Keys.O))
                Game1.PhysList.Add(new PhysModel(new Vector3(0, 20, 0), 1, 1));

            lastKs = curKs;
        }

        //Mouse Input
        public static void Mouse(float amount)
        {
            MouseState curMs = Microsoft.Xna.Framework.Input.Mouse.GetState();

            Game1.player.MouseInput(curMs, amount);

            lastMs = curMs;

        }
    }
}
