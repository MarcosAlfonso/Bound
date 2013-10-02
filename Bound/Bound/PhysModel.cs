using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Materials;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Bound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bound
{
    public class PhysModel
    {
        public Entity phys;
        public Model model;

        public Matrix scaleMatrix;
        public Matrix modelOffset = Matrix.Identity;



        public Boolean DrawModel = true;

        public enum PhysType
        {
            PlatformTouched = 0,
            PlatformRed = 1,
            PlatformTeal = 2,
            PlatformGreen = 3,
            PlatformOrange = 4,
            Player = 5,
            Collectable = 6,
        }

        public PhysType baseType;
        public PhysType curType;

        public PhysModel(Vector3 pos, Vector3 dims, int mass, PhysType pType)
        {
            if (mass == 0)
                phys = new Box(pos, dims.X, dims.Y, dims.Z);
            else
                phys = new Box(pos, dims.X, dims.Y, dims.Z, mass);

            baseType = pType;
            curType = pType;

            scaleMatrix = Matrix.CreateScale(dims*(1f/30f));

            Game1.space.Add(phys);

            if ((int)curType <= 4)
            {
                model = Render.platformModel;
            }
            else
                model = Render.cubeModel;
        }

        public PhysModel(Vector3 pos, float radius, int mass, PhysType pType)
        {
            if (mass == 0)
                phys = new Sphere(pos, radius);
            else
                phys = new Sphere(pos, radius, mass);

            baseType = pType;
            curType = pType;

            scaleMatrix = Matrix.CreateScale(radius*(1f/30f));

            Game1.space.Add(phys);
            model = Render.sphereModel;
        }

        public void Reset()
        {
            curType = baseType;
        }


        public void Draw()
        {
            if (DrawModel)
            {
                Render.DrawModel(model, scaleMatrix * modelOffset * phys.WorldTransform, "Simplest", Level.platColors[(int)curType]);
            }


        }
    }
}
