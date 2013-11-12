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
    //Physics + Model
    public class PhysModel
    {
        public Entity phys; //Generic physics shape container
        public Model model;
        public Vector3 dimensions;

        public Matrix scaleMatrix;

        //Model offset for weird scaling issues
        public Matrix modelOffset = Matrix.Identity;
        
        public Boolean DrawModel = true;

        //physType tracks the kind of physics object it is
        public PhysType physType;
        
        //Color Details
        public ColorType baseColor;
        public ColorType curColor;

        //Enum for the colors types
        public enum ColorType
        {
            Touched = 0,
            Red = 1,
            Teal = 2,
            Green = 3,
            Orange = 4,
        }

        //Enum for physics types
        public enum PhysType
        {
            StartPlatform = 0,
            ColumnPlatform = 1,
            RailPlatform = 2,
            Player = 3,
            Collectable = 4,
        }

        //PhysModel for Rectangular Prism
        public PhysModel(Vector3 pos, Vector3 dims, int mass, PhysType pType, ColorType cType)
        {
            dimensions = dims;

            //Mass determines of static or dynamic
            if (mass == 0)
                phys = new Box(pos, dims.X, dims.Y, dims.Z);
            else
                phys = new Box(pos, dims.X, dims.Y, dims.Z, mass);

            physType = pType;
            baseColor = cType;
            curColor = cType;

            //Scales by dim, scale down by .03.... why?
            scaleMatrix = Matrix.CreateScale(dims*(1f/30f));

            Game1.space.Add(phys);
            model = Render.cubeModel;
        }

        //PhysModel for Spheres
        public PhysModel(Vector3 pos, float radius, int mass, PhysType pType, ColorType cType)
        {
            dimensions = new Vector3(radius);

            //Mass determines whether static or dynamic
            if (mass == 0)
                phys = new Sphere(pos, radius);
            else
                phys = new Sphere(pos, radius, mass);

            physType = pType;
            baseColor = cType;
            curColor = cType; ;

            //Scales by radius, scale down by .03.... why?
            scaleMatrix = Matrix.CreateScale(radius*(1f/30f));

            Game1.space.Add(phys);
            model = Render.sphereModel;
        }

        //For resetting a platform to a pretouched state
        public void Reset()
        {
            curColor = baseColor;
        }


        public void Draw()
        {
            //If set to draw
            if (DrawModel)
            {
                Render.DrawModel(model, scaleMatrix * modelOffset * phys.WorldTransform, "Simplest", Level.platColors[(int)curColor % 5]);
            }
        }

        //Removes both physics object
        public void Remove()
        {
            Game1.space.Remove(phys);
        }
    }
}
