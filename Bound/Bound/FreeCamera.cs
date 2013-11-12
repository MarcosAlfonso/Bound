﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bound
{
    public class FreeCamera
    {
        //Camera looks at and position
        public Matrix viewMatrix;

        //Camera "lens"
        public Matrix projectionMatrix;

        //Camera vars
        public Vector3 Position = new Vector3(130, 30, -50);
        public float leftrightRot = MathHelper.Pi;
        public float updownRot = -MathHelper.Pi / 10.0f;
        
        //Control
        public float rotationSpeed = 0.1f;
        public float moveBoost;

        //Shake
        public bool runShake;
        public long landShakeStart; 
        public long jumpShakeStart; 

        public FreeCamera()
        {
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4*1.3f, Game1.device.Viewport.AspectRatio, 1.0f, 30000.0f);

        }

        public void UpdateViewMatrix(GameTime gameTime)
        {

            float shakeLeftRight = leftrightRot;
            float shakeUpDown = updownRot;

            if (runShake)
            {
                shakeLeftRight += (float) Math.Sin(gameTime.TotalGameTime.TotalMilliseconds*.006f)/50f;
                shakeUpDown += (float) Math.Sin(gameTime.TotalGameTime.TotalMilliseconds*.012f)/60f; 
            }

            if (gameTime.TotalGameTime.TotalMilliseconds - landShakeStart < 270)
            {
                shakeUpDown -= (float) Math.Sin(Math.PI*(gameTime.TotalGameTime.TotalMilliseconds - landShakeStart)/270f)/10f;
            }

            Matrix cameraRotation = Matrix.CreateRotationX(shakeUpDown) * Matrix.CreateRotationY(shakeLeftRight);


            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = Position + cameraRotatedTarget;

            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);



            viewMatrix = Matrix.CreateLookAt(Position, cameraFinalTarget, cameraRotatedUpVector);
        }

        
    }
}
