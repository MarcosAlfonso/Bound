using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ToBeDetermined
{
    static public class Render
    {
        struct MyOwnVertexFormat
        {
            private Vector3 position;
            private Vector2 texCoord;
            private Vector3 normal;

            public MyOwnVertexFormat(Vector3 position, Vector2 texCoord, Vector3 normal)
            {
                this.position = position;
                this.texCoord = texCoord;
                this.normal = normal;
            }

            public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(sizeof(float) * (3 + 2), VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
            );
        }

        public static Effect effect;

        //Lighting
        public static Vector3 lightPos = new Vector3(-250, 4, 30);
        static float lightPower = 1.0f;
        static float ambientPower = 0.7f;
        public static Vector3 tint = new Vector3(255,1.0f,1.0f);

        //Cameras
        static public FreeCamera camera;

        //Content
        public static Model testColumn;
        public static Model cubeModel;
        public static Model gridCube;
        public static Model sphereModel;
        public static Texture2D[] columnTextures;
        public static Texture2D[] cubeTextures;
        public static Texture2D[] gridTextures;
        public static Texture2D[] sphereTextures;

        public static void Initialize()
        {
            camera = new FreeCamera();
        }


        public static void Draw()
        {
            Game1.device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, new Color(184,214,224), 1.0f, 0);
            var rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            //rs.FillMode = FillMode.WireFrame;
            Game1.device.RasterizerState = rs;

            effect.CurrentTechnique = effect.Techniques["Simplest"];

            effect.Parameters["xWorldViewProjection"].SetValue(Matrix.Identity*camera.viewMatrix*camera.projectionMatrix);
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xLightPos"].SetValue(lightPos);
            effect.Parameters["xLightPower"].SetValue(lightPower);
            effect.Parameters["xAmbient"].SetValue(ambientPower);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }

            for (int i = 0; i < 10; i++)
            {
                //var columnMatrix = Matrix.CreateScale(.1f)*Matrix.CreateRotationY(i)*Matrix.CreateTranslation(60*i, 0, (50*i)%150);
                //DrawModel(testColumn, columnTextures, columnMatrix, "Simplest");
            }

            foreach (PhysModel pm in Game1.PhysList)
            {
                pm.Draw();
            }

        }

        public static void DrawModel(Model model, Texture2D[] textures, Matrix wMatrix, string technique, Vector3 tint)
        {
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            int i = 0;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;
                    currentEffect.CurrentTechnique = currentEffect.Techniques[technique];
                    currentEffect.Parameters["xWorldViewProjection"].SetValue(worldMatrix * camera.viewMatrix * camera.projectionMatrix);
                    currentEffect.Parameters["xTexture"].SetValue(textures[i++]);
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xLightPos"].SetValue(lightPos);
                    currentEffect.Parameters["xLightPower"].SetValue(lightPower);
                    currentEffect.Parameters["xAmbient"].SetValue(ambientPower);
                    currentEffect.Parameters["xColor"].SetValue(tint);

                }
                mesh.Draw();
            }
        }



    }
}
