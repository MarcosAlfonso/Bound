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

        //Player Vars
        public int jumpCount = 2;
        public float runSpeed = 120;
        public float accelSpeed;

        //Player States
        private Boolean isRunning;
        private Boolean isAccelerating;
        private Boolean isSliding;

        //Slide Vars
        private Boolean canSlide = true; //So you don't slide back in the opposite direction when reaching end of rail
        private PhysModel currentSlide;
        private Vector3 slideVelocity;

        //Score Keeping
        public PhysModel.ColorType lastColor = PhysModel.ColorType.Touched;
        public int platScore;
        public int combo;

        //Constructor
        public PlayerController(Vector3 pos)
        {
            //Creates player physModel
            physModel = new PhysModel(pos, 1f, 1, PhysModel.PhysType.Player, PhysModel.ColorType.Red);
            physModel.DrawModel = false;
            
            //Kills friction so player slides around freely
            physModel.phys.Material.KineticFriction = 0.0f;
            physModel.phys.Material.StaticFriction = 0.0f;

            Level.PhysList.Add(physModel);

            //Collision Event Callback
            physModel.phys.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;

            //Sets Camera
            camera = Render.camera;
        }

        //Handles all sorts of stuff pertaining to collisions
        private void HandleCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            //Iterates through all PhysModels to find the one were colliding with
            foreach (var physCollider in Level.PhysList)
            {
                //Found it!
                if (physCollider.phys.CollisionInformation == other)
                {

                    //Column Platform Physics
                    if (physCollider.physType == PhysModel.PhysType.ColumnPlatform)
                    {
                        //Checks if the collision vector was beneath the player sphere
                        if (pair.Contacts[0].Contact.Normal.Y > .8f)
                        {
                            //Resets jump count
                            jumpCount = 2;

                            //Figures out combo
                            if (lastColor == physCollider.curColor && lastColor != PhysModel.ColorType.Touched)
                                combo++;
                            else
                                combo = 1;

                            //Adds to overall score
                            platScore += combo;

                            //Tracks color for next combo
                            lastColor = physCollider.curColor;

                            //Sets platform to touched
                            physCollider.curColor = PhysModel.ColorType.Touched;
                        }
                    }
                    //Rail Platform Physics
                    else if (physCollider.physType == PhysModel.PhysType.RailPlatform && canSlide)
                    {
                        //Slide Details
                        canSlide = false;
                        jumpCount = 2;
                        isSliding = true;
                        currentSlide = physCollider;
                        physModel.phys.IsAffectedByGravity = false;
                        physCollider.curColor = PhysModel.ColorType.Touched;

                        //Handles slide direction based off current velocity
                        if (physModel.phys.LinearVelocity.Z > 0)
                            slideVelocity = new Vector3(0, 0, runSpeed + accelSpeed);
                        else
                            slideVelocity = new Vector3(0, 0, -(runSpeed + accelSpeed));
                    }
                }
            }

        }

        public void Update()
        {
            //Updates camera position to player phys with camera offset
            camera.Position = physModel.phys.Position + camOffset;
            camera.UpdateViewMatrix();

            //NO ROLLING!
            physModel.phys.AngularVelocity = new Vector3();

            //If speed is accelerating, add to it
            if (isAccelerating)
                accelSpeed += .03f;

            //Running Logic
            if (isRunning)
            {
                //Sliding Logic
                if (isSliding)
                {
                    //Sets up rail slide movement 
                    physModel.phys.LinearVelocity = slideVelocity;

                    //TODO
                    //Hard coded check for end of slide FIX THIS
                    if (physModel.phys.Position.Z > currentSlide.phys.Position.Z + 125 || physModel.phys.Position.Z < currentSlide.phys.Position.Z - 125)
                    {
                        isSliding = false;
                        physModel.phys.IsAffectedByGravity = true;
                    }
                }
                //Not sliding, just do normal movement in camera direction
                else
                {
                    physModel.phys.LinearVelocity = new Vector3((float) Math.Sin(camera.leftrightRot)*-(runSpeed + accelSpeed), physModel.phys.LinearVelocity.Y, (float) Math.Cos(camera.leftrightRot)*-(runSpeed + accelSpeed));
                }
            }

            //If you fall below 200, you DIE
            if (physModel.phys.Position.Y < 200)
                Kill();
        }

        public void KeyboardInput(KeyboardState ks)
        {
            //S - Start/Stop Running 
            if (ks.IsKeyDown(Keys.S) && !Input.lastKs.IsKeyDown(Keys.S))
                isRunning = !isRunning;

            //SPACE - Cheat Jump
            if (ks.IsKeyDown(Keys.Space))
                physModel.phys.LinearVelocity = new Vector3(physModel.phys.LinearVelocity.X, 50, physModel.phys.LinearVelocity.Z);

            //W - Start/Stop Accel
            if (ks.IsKeyDown(Keys.W) && !Input.lastKs.IsKeyDown(Keys.W))
                isAccelerating = !isAccelerating;

            //G - Generate new level
            if (ks.IsKeyDown(Keys.G) && !Input.lastKs.IsKeyDown(Keys.G))
            {
                Level.ClearLevel();
                Level.GenerateLevel();
            }

            //PgUp - Increases Speed
            if (ks.IsKeyDown(Keys.PageUp) && !Input.lastKs.IsKeyDown(Keys.PageUp))
                runSpeed += 75;

            //PgDn - Increases Speed
            if (ks.IsKeyDown(Keys.PageDown) && !Input.lastKs.IsKeyDown(Keys.PageDown))
                runSpeed -= 75;

            //R - Respawn
            if (ks.IsKeyDown(Keys.R) && !Input.lastKs.IsKeyDown(Keys.R))
            {
                Kill();
            }

        }

        public void MouseInput(MouseState curMs, float amt)
        {
            //Handles mouse control of camera
            float xDifference = curMs.X - Game1.ScreenW/2;
            float yDifference = curMs.Y - Game1.ScreenH/2;
            Render.camera.leftrightRot -= Render.camera.rotationSpeed*xDifference*amt;
            Render.camera.updownRot -= Render.camera.rotationSpeed*yDifference*amt;
            Mouse.SetPosition(Game1.device.Viewport.Width/2, Game1.device.Viewport.Height/2);
            camera.UpdateViewMatrix();

            //Click to jump, checks jump count
            if (curMs.LeftButton == ButtonState.Pressed && Input.lastMs.LeftButton == ButtonState.Released && jumpCount > 0)
            {
                //Slide Details
                isSliding = false;
                canSlide = true;

                //Jump Details
                physModel.phys.IsAffectedByGravity = true;
                physModel.phys.LinearVelocity = new Vector3(physModel.phys.LinearVelocity.X, 0, physModel.phys.LinearVelocity.Y);
                physModel.phys.ApplyImpulse(new Vector3(0), new Vector3(0, 80, 0));
                jumpCount--;
            }

            //Debug Stop 
            if (curMs.RightButton == ButtonState.Pressed)
            {
                physModel.phys.LinearVelocity = new Vector3();
                isRunning = false;
            }

        }

        //Handles Proper Death/Respawn
        public void Kill()
        {
            //Moves model, stops model, resets player movement vars
            physModel.phys.Position = Level.PhysList[2].phys.Position + new Vector3(0, 550, 0);
            physModel.phys.LinearVelocity = new Vector3();
            isRunning = false;
            runSpeed = 120;
            isAccelerating = false;
            accelSpeed = 0.0f;
            platScore = 0;

            //Resets all the platforms colors
            foreach (var plats in Level.PhysList)
            {
                plats.Reset();
            }
        }
    }
}
