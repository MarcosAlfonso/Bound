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

        public enum ColorType
        {
            Touched = 0,
            Red = 1,
            Teal = 2,
            Green = 3,
            Orange = 4,
        }

        public enum PhysType
        {
            ColumnPlatform = 0,
            WallRunPlatform = 1,
            Player = 2,
            Collectable = 3,
        }

        public PhysType physType;

        public ColorType baseColor;
        public ColorType curColor;

        public PhysModel(Vector3 pos, Vector3 dims, int mass, PhysType pType, ColorType cType)
        {
            if (mass == 0)
                phys = new Box(pos, dims.X, dims.Y, dims.Z);
            else
                phys = new Box(pos, dims.X, dims.Y, dims.Z, mass);

            ModelDataExtractor

            physType = pType;
            baseColor = cType;
            curColor = cType;

            phys.CollisionInformation.Shape.ComputeVolume();

            scaleMatrix = Matrix.CreateScale(dims*(1f/30f));

            Game1.space.Add(phys);

            if ((int)physType <= 1)
            {
                model = Render.platformModel;
            }
            else
                model = Render.cubeModel;
        }

        public PhysModel(Vector3 pos, float radius, int mass, PhysType pType, ColorType cType)
        {
            if (mass == 0)
                phys = new Sphere(pos, radius);
            else
                phys = new Sphere(pos, radius, mass);

            physType = pType;
            baseColor = cType;
            curColor = cType; ;

            scaleMatrix = Matrix.CreateScale(radius*(1f/30f));

            Game1.space.Add(phys);
            model = Render.sphereModel;
        }

        public void Reset()
        {
            curColor = baseColor;
        }


        public void Draw()
        {
            if (DrawModel)
            {
                if (physType == PhysType.WallRunPlatform)
                {
                    var skinnyMatrix = Matrix.CreateScale(1, .333f, 1);
                    Render.DrawModel(model, skinnyMatrix * scaleMatrix*modelOffset*phys.WorldTransform, "Simplest", Level.platColors[(int) curColor%5]);
                }
                else
                    Render.DrawModel(model, scaleMatrix * modelOffset * phys.WorldTransform, "Simplest", Level.platColors[(int)curColor % 5]);

            }


        }
    }
}
