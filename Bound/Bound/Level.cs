using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ToBeDetermined;

namespace Bound
{
    static class Level
    {
        public static PlayerController player;
        public static List<PhysModel> PhysList = new List<PhysModel>();

        public static void Initialize()
        {
            GenerateLevel();
            player = new PlayerController(Level.PhysList[2].phys.Position + new Vector3(0, 550, 0));
        }


        public static void GenerateLevel()
        {
            //How many rows of plats
            for (int j = 0; j < 60; j++)
            {
                //Picks which platforms will spawn, 50/50
                bool[] platDecide = { Game1.r.Next(0, 2) == 1, Game1.r.Next(0, 2) == 1, Game1.r.Next(0, 2) == 1, Game1.r.Next(0, 2) == 1, Game1.r.Next(0, 2) == 1 };

                //If none are true, start over
                if (!platDecide[0] && !platDecide[1] && !platDecide[2] && !platDecide[3] && !platDecide[4])
                {
                    platDecide[Game1.r.Next(0, 5)] = true;
                }

                int xOffset = Game1.r.Next(-1, 1);

                for (int i = 0; i < 5; i++)
                {
                    if (platDecide[i])
                    {
                        PhysModel physModel = new PhysModel(new Vector3(-250 + 120 * (i+xOffset), Game1.r.Next(-80, -50), -250 + 250 * j), new Vector3(70, 1000, 70), 0, PhysModel.PhysType.Platform);
                        physModel.modelOffset = Matrix.CreateTranslation(0, -8, 0);
                        physModel.setColorTint(new Vector3(.85f, 0.05f, 0.05f));
                        PhysList.Add(physModel);
                    }
                }
            }
        }

        public static void ClearLevel()
        {
            foreach (var physModel in PhysList.ToList())
            {
                if (physModel.Type == PhysModel.PhysType.Platform)
                {
                    Game1.space.Remove(physModel.phys);
                    PhysList.Remove(physModel);
                }
            }

        }
    }
}
