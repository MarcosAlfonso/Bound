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
        public static List<LevelRow> RowList = new List<LevelRow>();
        public static List<VisModel> VisList = new List<VisModel>();

        public static int colorCount;

        public static Vector3[] platColors =
            {
                //grey (touched)
                new Vector3(.4f, .4f, .4f),
                //red
                new Vector3(.85f, .05f, .05f),
                //teal
                new Vector3(.1f, .6f, .9f),
                //green
                new Vector3(.05f, .8f, .05f),
                //orange
                new Vector3(1, .6f, 0),
            };

        //Sets up a new level
        public static void Initialize()
        {
            GenerateLevel();

            //TODO
            //Moves player, this should move to a intro platform that is always the same
            player = new PlayerController(new Vector3(0, 500, -120));
        }

        public static void GenerateLevel()
        {
            //Incrememnters
            colorCount = 0;

            //Creates row
            RowList.Add(new LevelRow(PhysModel.PhysType.StartPlatform));

            //How many rows of plats
            for (int j = 1; j < 100; j++)
            {
                //Every 5 platforms do a column platform
                if (j % 10 < 5)
                {
                    RowList.Add(new LevelRow(PhysModel.PhysType.ColumnPlatform));
                }
                else
                {
                    RowList.Add(new LevelRow(PhysModel.PhysType.RailPlatform));
                }
            }

            //Adds large black cubes on left and right for visual communication
            for (int i = 0; i < 20; i++)
            {
                VisList.Add(new VisModel(new Vector3(1200, 200, 250 + i*1200), new Vector3(Game1.r.Next(10, 24)), new Vector3((float) Game1.r.NextDouble()*MathHelper.TwoPi, (float) Game1.r.NextDouble()*MathHelper.TwoPi, (float) Game1.r.NextDouble()*MathHelper.TwoPi)));
                VisList.Add(new VisModel(new Vector3(-1200, 200, 250 + i*1200), new Vector3(Game1.r.Next(10, 24)), new Vector3((float) Game1.r.NextDouble()*MathHelper.TwoPi, (float) Game1.r.NextDouble()*MathHelper.TwoPi, (float) Game1.r.NextDouble()*MathHelper.TwoPi)));
            }
        }

        //Clears a level of models
        public static void ClearLevel()
        {
            foreach (var row in RowList.ToList())
            {
                row.DeletePhys();
            }
            RowList.Clear();
            VisList.Clear();
        }
    }

    public class LevelRow
    {
        
        public PhysModel[] platArray = new PhysModel[6];
        public PhysModel.PhysType Type;
        public int Count;

        //Random offset laterally
        private float xOffset = (float) ((Game1.r.NextDouble() - .5f)*-2.5);

        public LevelRow(PhysModel.PhysType rowType)
        {
            //Row count in level
            Count = Level.RowList.Count;

            //Platform type this row hold
            Type = rowType;

            //Randomization for platform placements, weighted towards center
            bool[] platDecide = {Game1.r.Next(0, 6) < 3, Game1.r.Next(0, 5) < 3, Game1.r.Next(0, 4) < 3, Game1.r.Next(0, 4) < 3, Game1.r.Next(0, 5) < 31, Game1.r.Next(0, 6) < 3};
            //If none are true, make one true
            if (!platDecide[0] && !platDecide[1] && !platDecide[2] && !platDecide[3] && !platDecide[4] && !platDecide[5])
            {
                platDecide[Game1.r.Next(0, 6)] = true;
            }

            //if row Type is Column
            switch (Type)
            {
                case PhysModel.PhysType.ColumnPlatform:
                    for (int i = 0; i < 6; i++)
                    {
                        if (platDecide[i])
                        {
                            Level.colorCount++;
                            int colorIndex = (Level.colorCount%4) + 1;
                            PhysModel nextPlatform = new PhysModel(new Vector3(-250 + 120*(i + xOffset), Game1.r.Next(-100, -40), 250*Count), new Vector3(70, 1000, 70), 0, PhysModel.PhysType.ColumnPlatform, (PhysModel.ColorType) colorIndex);
                            nextPlatform.modelOffset = Matrix.CreateTranslation(0, -8, 0);

                            platArray[i] = nextPlatform;
                        }
                    }
                    break;
                case PhysModel.PhysType.RailPlatform:
                    for (int i = 0; i < 6; i++)
                    {
                        if (platDecide[i])
                        {
                            Level.colorCount++;
                            int colorIndex = (Level.colorCount%4) + 1;

                            PhysModel slidePlatform = new PhysModel(new Vector3(-250 + 120*(i + xOffset), Game1.r.Next(350, 450), 250*Count), new Vector3(20, 20, 250), 0, PhysModel.PhysType.RailPlatform, (PhysModel.ColorType) colorIndex);
                            slidePlatform.scaleMatrix *= Matrix.CreateScale(.5f, .5f, 1);
                            slidePlatform.modelOffset = Matrix.CreateTranslation(0, 2, 0);

                            platArray[i] = slidePlatform;
                        }
                    }
                    break;
                case PhysModel.PhysType.StartPlatform:
                    Level.colorCount = (Game1.r.Next(0, 5) + 1);
                    PhysModel starterPlaftorm = new PhysModel(new Vector3(0, Game1.r.Next(-100, -40), 250 * Count), new Vector3(70, 1000, 300), 0, PhysModel.PhysType.StartPlatform, PhysModel.ColorType.Touched);
                    starterPlaftorm.modelOffset = Matrix.CreateTranslation(0, -8, 0);
                    platArray[3] = starterPlaftorm;
                    break;
            }

            //Random Blocking Columns
            if (Game1.r.Next(0,3) == 2 && Count != 0)
            {
                int i = Game1.r.Next(0,6);
                Level.colorCount++;
                int colorIndex = (Level.colorCount % 4) + 1;
                PhysModel nextPlatform = new PhysModel(new Vector3(-250 + 120 * (i + xOffset), Game1.r.Next(100, 200), 250 * Count), new Vector3(90, 1000, 90), 0, PhysModel.PhysType.ColumnPlatform, (PhysModel.ColorType)colorIndex);
                nextPlatform.modelOffset = Matrix.CreateTranslation(0, -8, 0);

                if (platArray[i] != null)
                    platArray[i].Remove();

                platArray[i] = nextPlatform;
            }


        }

        //Removes all the physModels in an array from space
        public void DeletePhys()
        {
            for (int i = 0; i < 6; i++)
            {
                var physModel = platArray[i];
                if (physModel != null)
                    Game1.space.Remove(physModel.phys);
            }
        }

        //Drawing Function
        public void Draw()
        {
            for (int i = 0; i < 6; i++)
            {
                var physModel = platArray[i];
                if (physModel != null)
                    physModel.Draw();
            }
        }
    }
}



