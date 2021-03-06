﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bound
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

            Level.player.KeyboardInput(curKs);

            lastKs = curKs;
        }

        //Mouse Input
        public static void Mouse(float amount)
        {
            MouseState curMs = Microsoft.Xna.Framework.Input.Mouse.GetState();

            Level.player.MouseInput(curMs, amount);

            lastMs = curMs;

        }
    }
}
