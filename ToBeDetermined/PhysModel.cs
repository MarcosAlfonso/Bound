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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ToBeDetermined
{
    public class PhysModel
    {
        public Entity phys;
        public Model model;

        public Matrix scaleMatrix;

        public Boolean DrawModel = true;
        public Vector3 colorTint = new Vector3(1);

        public PhysModel(Vector3 pos, Vector3 dims, int mass)
        {
            if (mass == 0)
                phys = new Box(pos, dims.X,dims.Y,dims.Z);
            else
                phys = new Box(pos, dims.X, dims.Y, dims.Z, mass);

            scaleMatrix = Matrix.CreateScale(dims*.033333333f);

            Game1.space.Add(phys);
            model = Render.gridCube;
        }

        public PhysModel(Vector3 pos, float radius, int mass)
        {
            if (mass == 0)
                phys = new Sphere(pos, radius);
            else
                phys = new Sphere(pos, radius, mass);


            scaleMatrix = Matrix.CreateScale(radius * .033333333f);

            Game1.space.Add(phys);
            model = Render.sphereModel;
        }

        public void setColorTint( Vector3 tint)
        {
            colorTint = tint;
        }

        public void Draw()
        {
            if (DrawModel)
                Render.DrawModel(model, Render.cubeTextures, scaleMatrix*phys.WorldTransform, "Simplest", colorTint);
        }



    }
}
