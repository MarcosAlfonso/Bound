using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bound
{
    public static class Render
    {
        public static Effect effect;

        //Lighting
        public static Vector3 lightDir = Vector3.Normalize(new Vector3(0, 1, 0));
        private static float lightPower = 0.7f;
        private static float ambientPower = 0.7f;
        public static Vector3 tint = new Vector3(255, 1.0f, 1.0f);

        //Cameras
        public static FreeCamera camera;

        //Content
        public static Model cubeModel;
        public static Model sphereModel;
        public static Model platformModel;
        public static Model skyDomeModel;

        public static SpriteFont debugFont;

        public static DebugDisplay debugDisplay = new DebugDisplay(20, 20);

        public static void Initialize()
        {
            camera = new FreeCamera();
        }


        public static void Draw()
        {
            Game1.device.BlendState = BlendState.Opaque;
            Game1.device.DepthStencilState = DepthStencilState.Default;
            Game1.device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, new Color(255,255,255), 1.0f, 0);
            var rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            //rs.FillMode = FillMode.WireFrame;
            Game1.device.RasterizerState = rs;

            DrawSkyDome();

            foreach (PhysModel pm in Level.PhysList)
            {
                pm.Draw();
            }

            foreach (VisModel vm in Level.VisList)
            {
                vm.Draw();
            }

            //Debugs
            debugDisplay.addDebug("Current Speed: " + (Level.player.runSpeed + Level.player.boostSpeed));
            debugDisplay.addDebug("Current Position: " + Level.player.physModel.phys.Position);
            debugDisplay.addDebug("Last Platform: " + Level.player.lastPlatform );
            debugDisplay.addDebug("Combo: " + Level.player.combo);
            debugDisplay.addDebug("Score " + Level.player.platScore);

            Game1.spriteBatch.Begin();
            debugDisplay.Draw();
            Game1.spriteBatch.End();

        }

        public static void DrawModel(Model model, Matrix wMatrix, string technique, Vector3 tint)
        {
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index]*wMatrix;
                    currentEffect.CurrentTechnique = currentEffect.Techniques[technique];
                    currentEffect.Parameters["xWorldViewProjection"].SetValue(worldMatrix*camera.viewMatrix*camera.projectionMatrix);
                    //currentEffect.Parameters["xTexture"].SetValue(textures[i++]);
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xLightDir"].SetValue(lightDir);
                    currentEffect.Parameters["xLightPower"].SetValue(lightPower);
                    currentEffect.Parameters["xAmbient"].SetValue(ambientPower);
                    currentEffect.Parameters["xColor"].SetValue(tint);
                    //float xFogStart;
                    currentEffect.Parameters["xFogStart"].SetValue(650);
                    //float xFogEnd;
                    currentEffect.Parameters["xFogEnd"].SetValue(2000);
                    //float3 xFogColor;
                    currentEffect.Parameters["xFogColor"].SetValue(new Vector3(1));

                    //float4x4 World;
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);

                    //float4x4 View;
                    currentEffect.Parameters["xView"].SetValue(camera.viewMatrix);

                    currentEffect.Parameters["xProjection"].SetValue(camera.projectionMatrix);

                    //float3 cameraPosition;
                    currentEffect.Parameters["cameraPosition"].SetValue(camera.Position);

                }
                mesh.Draw();
            }
        }

        private static void DrawSkyDome()
        {
            Game1.device.DepthStencilState = DepthStencilState.DepthRead;

            Matrix[] modelTransforms = new Matrix[skyDomeModel.Bones.Count];
            skyDomeModel.CopyAbsoluteBoneTransformsTo(modelTransforms);

            Matrix wMatrix = Matrix.CreateTranslation(0, -0.3f, 0)*Matrix.CreateScale(100)*Matrix.CreateTranslation(camera.Position);
            foreach (ModelMesh mesh in skyDomeModel.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index]*wMatrix;
                    currentEffect.CurrentTechnique = currentEffect.Techniques["SkyDome"];
                    currentEffect.Parameters["xWorldViewProjection"].SetValue(worldMatrix*camera.viewMatrix*camera.projectionMatrix);
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                }
                mesh.Draw();
            }
            Game1.device.DepthStencilState = DepthStencilState.Default;

        }
    }
}
