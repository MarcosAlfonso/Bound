using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionTests;
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
        public FreeCamera camera;
        private Vector3 camOffset = new Vector3(0, 4, 0);

        //Player Vars
        public int jumpCount = 2;
        public float runSpeed = 120;
        public float accelSpeed;
        public float accelRate = .01f;

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
        public int highscore;

        //Constructor
        public PlayerController(Vector3 pos)
        {
            //Creates player physModel
            physModel = new PhysModel(pos, 1f, 1, PhysModel.PhysType.Player, PhysModel.ColorType.Red);
            physModel.DrawModel = false;
            
            //Kills friction so player slides around freely
            physModel.phys.Material.KineticFriction = 0.0f;
            physModel.phys.Material.StaticFriction = 0.0f;

            //Collision Event Callback
            physModel.phys.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;
            physModel.phys.CollisionInformation.Events.ContactRemoved += EndCollision;

            //Sets Camera
            camera = Render.camera;
        }

        private void EndCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair, ContactData contact)
        {
            camera.runShake = false;
        }

        //Handles all sorts of stuff pertaining to collisions
        private void HandleCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            //Iterates through all PhysModels to find the one were colliding with
            foreach (var row in Level.RowList.ToList())
            {
                for (int i = 0; i < 6; i++)
                {
                    PhysModel physCollider = row.platArray[i];

                    //Found it!
                    if (physCollider != null && physCollider.phys.CollisionInformation == other)
                    {
                        //Re Enable Shake, we're running
                        camera.runShake = true;

                        camera.landShakeStart = (long)Game1.gameRunTime.TotalGameTime.TotalMilliseconds;


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
                                if (physCollider.curColor != PhysModel.ColorType.Touched)
                                    platScore += combo;

                                //Tracks color for next combo
                                lastColor = physCollider.curColor;

                                //Sets platform to touched
                                physCollider.curColor = PhysModel.ColorType.Touched;
                            }
                        }
                            //Rail Platform Physics
                        else if (physCollider.physType == PhysModel.PhysType.RailPlatform && (canSlide || currentSlide != physCollider))
                        {
                            //Slide Details
                            canSlide = false;
                            jumpCount = 2;
                            isSliding = true;
                            currentSlide = physCollider;
                            physModel.phys.IsAffectedByGravity = false;

                            //Handles slide direction based off current velocity
                            if (physModel.phys.LinearVelocity.Z > 0)
                                slideVelocity = new Vector3(0, 0, runSpeed + accelSpeed);
                            else
                                slideVelocity = new Vector3(0, 0, -(runSpeed + accelSpeed));

                            //Figures out combo
                            if (lastColor == physCollider.curColor && lastColor != PhysModel.ColorType.Touched)
                                combo++;
                            else
                                combo = 1;

                            //Adds to overall score
                            if (physCollider.curColor != PhysModel.ColorType.Touched)
                                platScore += combo;

                            //Tracks color for next combo
                            lastColor = physCollider.curColor;

                            //Sets platform to touched
                            physCollider.curColor = PhysModel.ColorType.Touched;
                        }
                    }
                }

            }
        }

        public void Update(GameTime gameTime)
        {
            //Updates camera position to player phys with camera offset
            camera.Position = physModel.phys.Position + camOffset;
            camera.UpdateViewMatrix(gameTime);

            //NO ROLLING!
            physModel.phys.AngularVelocity = new Vector3();

            if (platScore > highscore)
                highscore = platScore;

            //If speed is accelerating, add to it
            if (isAccelerating)
                accelSpeed += accelRate;

            //Running Logic
            if (isRunning)
            {
                //Sliding Logic
                if (isSliding)
                {
                    //Sets up rail slide movement 
                    physModel.phys.LinearVelocity = slideVelocity;

                    if (physModel.phys.Position.Z > currentSlide.phys.Position.Z + currentSlide.dimensions.Z / 2 || physModel.phys.Position.Z < currentSlide.phys.Position.Z - currentSlide.dimensions.Z / 2)
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

            if (physModel.phys.Position.Y < 350)
            {
                camera.fallShake = true;
                camera.shakeScale = 8-((Math.Abs(-200 - physModel.phys.Position.Y))/550f*8);
            }
            //If you fall below -100, you DIE
            if (physModel.phys.Position.Y < -200)
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

            //Click to jump, checks jump count
            if (curMs.LeftButton == ButtonState.Pressed && Input.lastMs.LeftButton == ButtonState.Released && jumpCount > 0)
            {
                //Slide Details
                isSliding = false;
                canSlide = true;

                //Jump Details
                camera.jumpShakeStart = (long)Game1.gameRunTime.TotalGameTime.TotalMilliseconds;

                physModel.phys.IsAffectedByGravity = true;
                physModel.phys.LinearVelocity = new Vector3(physModel.phys.LinearVelocity.X, 0, physModel.phys.LinearVelocity.Y);
                physModel.phys.ApplyImpulse(new Vector3(0), new Vector3(0, 85, 0));
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
            physModel.phys.Position = new Vector3(0, 500, -120);

            physModel.phys.LinearVelocity = new Vector3();
            isRunning = false;
            jumpCount = 2;
            runSpeed = 120;
            isAccelerating = false;
            accelSpeed = 0.0f;
            platScore = 0;
            combo = 1;
            camera.shakeScale = 0;

            //Resets all the platforms colors
            foreach (var row in Level.RowList)
            {
                for (int i = 0; i < 6; i++)
                {
                    var plat = row.platArray[i];
                    if (plat != null)
                        plat.Reset();
                }
            }
        }
    }
}
