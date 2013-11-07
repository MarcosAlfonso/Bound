using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bound
{
    //Object for drawing a model
    class VisModel
    {
        public Model model;
        public Matrix worldMatrix;

        //Constructor sets model to cube, and creates matrix
        public VisModel(Vector3 pos, Vector3 scale, Vector3 rot)
        {
            model = Render.cubeModel;
            worldMatrix = Matrix.CreateScale(scale)*Matrix.CreateFromYawPitchRoll(rot.X, rot.Y, rot.Z)*Matrix.CreateTranslation(pos);
        }

        public void Draw()
        {
            Render.DrawModel(model, worldMatrix, "Simplest", Level.platColors[0]);            
        }




    }
}
