using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Materials;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Bound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bound
{
    public class PlayerController
    {
        public PhysModel physModel;

        //Camera
        private FreeCamera camera;
        private Vector3 camOffset = new Vector3(0, 4, 0);

        public int jumpCount = 2;

        public float runSpeed = 120;
        private Boolean isRunning;
        public float boostSpeed;
        private Boolean isBoosting;

        private Boolean isSliding;
        private PhysModel currentSlide;

        //Score Keeping
        public PhysModel.ColorType lastColor = PhysModel.ColorType.Touched;
        public int platScore;
        public int combo;

        public PlayerController(Vector3 pos)
        {
            physModel = new PhysModel(pos, 1f, 1, PhysModel.PhysType.Player, PhysModel.ColorType.Red);
            physModel.DrawModel = false;
            Level.PhysList.Add(physModel);

            physModel.phys.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;

            camera = Render.camera;
        }

        private void HandleCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            foreach (var physCollider in Level.PhysList)
            {
                if (physCollider.phys.CollisionInformation == other)
                {

                    //Column Platform Physics
                    if (physCollider.physType == PhysModel.PhysType.ColumnPlatform )
                    {
                        if (pair.Contacts[0].Contact.Normal.Y > .8f)
                        {
                            jumpCount = 2;
                            if (lastColor == physCollider.curColor && lastColor != PhysModel.ColorType.Touched)
                                combo++;
                            else
                                combo = 1;

                            platScore += combo;

                            lastColor = physCollider.curColor;
                            physCollider.curColor = PhysModel.ColorType.Touched;
                        }
                    }
                    else if (physCollider.physType == PhysModel.PhysType.WallRunPlatform)
                    {
                        jumpCount = 2;
                        isSliding = true;
                        currentSlide = physCollider;
                        physModel.phys.IsAffectedByGravity = false;
                        physCollider.curColor = PhysModel.ColorType.Touched;
                    }
                }
            }

        }

        public void Update()
        {
            camera.Position = physModel.phys.Position + camOffset;
            camera.UpdateViewMatrix();

            physModel.phys.AngularVelocity = new Vector3();

            if (isBoosting)
                boostSpeed += .03f;

            if (isRunning)
            {
                if (isSliding)
                {
                    if (physModel.phys.LinearVelocity.Z > 0)
                        physModel.phys.LinearVelocity = new Vector3(0, 0, runSpeed + boostSpeed);
                    else
                        physModel.phys.LinearVelocity = new Vector3(0, 0, -(runSpeed + boostSpeed));
                    
                        
                    

                    if (physModel.phys.Position.Z > currentSlide.phys.Position.Z + 125 || physModel.phys.Position.Z < currentSlide.phys.Position.Z - 125)
                    {
                        isSliding = false;
                        physModel.phys.IsAffectedByGravity = true;

                    }
                }
                else
                {
                    physModel.phys.LinearVelocity = new Vector3((float)Math.Sin(camera.leftrightRot) * -(runSpeed + boostSpeed), physModel.phys.LinearVelocity.Y, (float)Math.Cos(camera.leftrightRot) * -(runSpeed + boostSpeed));
                }
            }

            if (physModel.phys.Position.Y < 200)
                Kill();
        }

        public void KeyboardInput(KeyboardState ks)
        {
            if (ks.IsKeyDown(Keys.S) && !Input.lastKs.IsKeyDown(Keys.S))
                isRunning = !isRunning;

            if (ks.IsKeyDown(Keys.Space))
                physModel.phys.LinearVelocity = new Vector3(physModel.phys.LinearVelocity.X, 50, physModel.phys.LinearVelocity.Z);

            if (ks.IsKeyDown(Keys.W) && !Input.lastKs.IsKeyDown(Keys.W))
                isBoosting = !isBoosting;

            if (ks.IsKeyDown(Keys.G) && !Input.lastKs.IsKeyDown(Keys.G))
            {
                Level.ClearLevel();
                Level.GenerateLevel();
            }

            if (ks.IsKeyDown(Keys.PageUp) && !Input.lastKs.IsKeyDown(Keys.PageUp))
                runSpeed += 75;

            if (ks.IsKeyDown(Keys.PageDown) && !Input.lastKs.IsKeyDown(Keys.PageDown))
                runSpeed -= 75;

            if (ks.IsKeyDown(Keys.R) && !Input.lastKs.IsKeyDown(Keys.R))
            {
                Kill();
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
                isSliding = false;

                physModel.phys.IsAffectedByGravity = true;
                physModel.phys.LinearVelocity = new Vector3(physModel.phys.LinearVelocity.X, 0, physModel.phys.LinearVelocity.Y);
                physModel.phys.ApplyImpulse(new Vector3(0), new Vector3(0,80,0));
                jumpCount--;
            }

            if (curMs.RightButton == ButtonState.Pressed)
            {
                physModel.phys.LinearVelocity = new Vector3();
                isRunning = false;
            }

        }

        public void Kill()
        {
            physModel.phys.Position = Level.PhysList[2].phys.Position + new Vector3(0, 550, 0);
            physModel.phys.LinearVelocity = new Vector3();
            isRunning = false;
            runSpeed = 120;
            isBoosting = false;
            boostSpeed = 0.0f;
            platScore = 0;

            foreach (var plats in Level.PhysList)
            {
                plats.Reset();
            }
        }
    }
}
