using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ToBeDetermined
{
    public class FreeCamera
    {
        //Camera looks at and position
        public Matrix viewMatrix;
        //Camera "lens"
        public Matrix projectionMatrix;

        //Camera vars
        public Vector3 position = new Vector3(130, 30, -50);
        public float leftrightRot = MathHelper.PiOver2;
        public float updownRot = -MathHelper.Pi / 10.0f;
        
        //Control
        public float rotationSpeed = 0.1f;
        public float moveBoost;

        public FreeCamera()
        {
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4*1.25f, Game1.device.Viewport.AspectRatio, 1.0f, 1000.0f);

            UpdateViewMatrix();
        }

        public void UpdateViewMatrix()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);

            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = position + cameraRotatedTarget;

            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

            viewMatrix = Matrix.CreateLookAt(position, cameraFinalTarget, cameraRotatedUpVector);
        }

        
    }
}
