using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Materials;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ToBeDetermined
{
    public class PlayerController
    {
        public PhysModel physModel;

        //Camera
        private FreeCamera camera;
        private Vector3 camOffset = new Vector3(0, 2, 0);

        public int jumpCount = 2;

        private float runSpeed = 120;
        private Boolean isRunning;
        private float boostSpeed;
        private Boolean isBoosting;


        public PlayerController(Vector3 pos)
        {
            physModel = new PhysModel(pos, 3f, 1);
            physModel.DrawModel = false;
            Game1.PhysList.Add(physModel);

            physModel.phys.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;

            camera = Render.camera;
        }

        private void HandleCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            jumpCount = 2;
        }

        public void Update()
        {
            camera.position = physModel.phys.Position + camOffset;
            camera.UpdateViewMatrix();

            physModel.phys.AngularVelocity = new Vector3();

            if (isBoosting)
                boostSpeed += .2f;

            if (isRunning)
                physModel.phys.LinearVelocity = new Vector3((float)Math.Sin(camera.leftrightRot) * -(runSpeed + boostSpeed), physModel.phys.LinearVelocity.Y, (float)Math.Cos(camera.leftrightRot) * -(runSpeed + boostSpeed));

        }

        public void KeyboardInput(KeyboardState ks)
        {
            if (ks.IsKeyDown(Keys.S) && !Input.lastKs.IsKeyDown(Keys.S))
                isRunning = !isRunning;

            if (ks.IsKeyDown(Keys.W) && !Input.lastKs.IsKeyDown(Keys.W))
                isBoosting = !isBoosting;

            if (ks.IsKeyDown(Keys.R) && !Input.lastKs.IsKeyDown(Keys.R))
            {
                physModel.phys.Position = new Vector3(0, 10, 0);
                isRunning = false;
                isBoosting = false;
                boostSpeed = 0.0f;
            }

        }

        public void MouseInput(MouseState curMs, float amt)
        {

            float xDifference = curMs.X - Game1.ScreenW / 2;
            float yDifference = curMs.Y - Game1.ScreenH / 2;
            Render.camera.leftrightRot -= Render.camera.rotationSpeed*xDifference*amt;
            Render.camera.updownRot -= Render.camera.rotationSpeed*yDifference*amt;
            Mouse.SetPosition(Game1.device.Viewport.Width/2, Game1.device.Viewport.Height/2);
            camera.UpdateViewMatrix();

            if (curMs.LeftButton == ButtonState.Pressed && Input.lastMs.LeftButton == ButtonState.Released && jumpCount > 0)
            {
                physModel.phys.LinearVelocity = new Vector3(physModel.phys.LinearVelocity.X, 0, physModel.phys.LinearVelocity.Y);
                physModel.phys.ApplyImpulse(new Vector3(0), new Vector3(0,70,0));;
                jumpCount--;
            }

            if (curMs.RightButton == ButtonState.Pressed)
            {
                physModel.phys.LinearVelocity = new Vector3();
            }

        }
    }
}
