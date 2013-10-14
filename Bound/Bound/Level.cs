using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace Bound
{
    internal static class Level
    {
        public static PlayerController player;
        public static List<PhysModel> PhysList = new List<PhysModel>();
        public static List<VisModel> VisList = new List<VisModel>();
        
        public static Vector3[] platColors =
            {
                //grey (touched)
                new Vector3(.4f,.4f,.4f),
                //red
                new Vector3(.85f, .05f, .05f), 
                //teal
                new Vector3(.1f,.6f,.9f), 
                //green
                new Vector3(.05f,.8f,.05f), 
                //orange
                new Vector3(1,.6f,0),
            };

    public static void Initialize()
        {
            GenerateLevel();
            player = new PlayerController(Level.PhysList[2].phys.Position + new Vector3(0, 550, 0));
        }


        public static void GenerateLevel()
        {
            float heightCount = 0;
            int colorCount = 0;

            //How many rows of plats
            for (int j = 0; j < 60; j++)
            {
                var heightVar = (float)Math.Sin(heightCount);
                heightCount += .08f;

                //Picks which platforms will spawn, 50/50
                bool[] platDecide = { Game1.r.Next(0, 6) < 3, Game1.r.Next(0, 5) < 3, Game1.r.Next(0, 4) < 3, Game1.r.Next(0, 4) < 3, Game1.r.Next(0, 5) < 31, Game1.r.Next(0, 6) < 3 };

                //If none are true, start over
                if (!platDecide[0] && !platDecide[1] && !platDecide[2] && !platDecide[3] && !platDecide[4] && !platDecide[5])
                {
                    platDecide[Game1.r.Next(0, 6)] = true;
                }

      

                float xOffset = (float)((Game1.r.NextDouble()-1)*-2.5);

                if (j % 5 == 0)
                {
                    PhysModel slidePlatform = new PhysModel(new Vector3(200, Game1.r.Next(350, 450) + heightVar * 110, 250 * j), new Vector3(120, 15, 250), 0, PhysModel.PhysType.WallRunPlatform, (PhysModel.ColorType)Game1.r.Next(0,5));
                    var rotMat = Matrix.CreateRotationZ(MathHelper.PiOver2);
                    slidePlatform.phys.OrientationMatrix = new Matrix3X3(rotMat.M11, rotMat.M12, rotMat.M13, rotMat.M21, rotMat.M22, rotMat.M23, rotMat.M31, rotMat.M32, rotMat.M33);
                    PhysList.Add(slidePlatform);
                }
                else
                {

                    for (int i = 0; i < 6; i++)
                    {
                        if (platDecide[i])
                        {
                            colorCount++;
                            int colorIndex = (colorCount%4) + 1;
                            PhysModel nextPlatform = new PhysModel(new Vector3(-250 + 120 * (i + xOffset), Game1.r.Next(-100, -40) + heightVar * 110, 250 * j), new Vector3(70, 1000, 70), 0, PhysModel.PhysType.ColumnPlatform, (PhysModel.ColorType)colorIndex);
                            nextPlatform.modelOffset = Matrix.CreateTranslation(0, -8, 0);
                            PhysList.Add(nextPlatform);
                        }
                    }
                }
            }

            for (int i = 0; i < 12; i++)
            {
                VisList.Add(new VisModel(new Vector3(1600, 200, 250+i*1200), new Vector3(Game1.r.Next(10, 30)), new Vector3((float) Game1.r.NextDouble()*MathHelper.TwoPi, (float) Game1.r.NextDouble()*MathHelper.TwoPi, (float) Game1.r.NextDouble()*MathHelper.TwoPi)));
                VisList.Add(new VisModel(new Vector3(-1200, 200, 250+i*1200), new Vector3(Game1.r.Next(10, 30)), new Vector3((float) Game1.r.NextDouble()*MathHelper.TwoPi, (float) Game1.r.NextDouble()*MathHelper.TwoPi, (float) Game1.r.NextDouble()*MathHelper.TwoPi)));
            }
        }

        public static void ClearLevel()
        {
            foreach (var physModel in PhysList.ToList())
            {
                if (physModel.physType == PhysModel.PhysType.ColumnPlatform || physModel.physType == PhysModel.PhysType.WallRunPlatform)
                {
                    Game1.space.Remove(physModel.phys);
                    PhysList.Remove(physModel);
                }
            }
            VisList.Clear();
        }
    }
}
