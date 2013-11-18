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
        public static Vector3 lightDirDown = Vector3.Normalize(new Vector3(0, 1, 0));
        public static Vector3 lightDirUp = Vector3.Normalize(new Vector3(0, -1, 0));
        private static float lightPower = 0.7f;
        private static float ambientPower = 0.7f;
        public static Vector3 tint = new Vector3(255, 1.0f, 1.0f);

        //Cameras
        public static FreeCamera camera;

        //Content
        public static Model cubeModel;
        public static Model sphereModel;
        public static Model columnModel;
        public static Model skyDomeModel;
        public static SpriteFont debugFont;
        public static SpriteFont scoreFont;

        //Debug
        public static DebugDisplay debugDisplay = new DebugDisplay(20, 20);

        public static void Initialize()
        {
            camera = new FreeCamera();
        }

        public static void Draw()
        {
            //Clears screen
            Game1.device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, new Color(255,255,255), 1.0f, 0);

            DrawSkyDome();

            //Draws all the PhysModels
            foreach (var row in Level.RowList)
            {
                row.Draw();
            }

            //Draws all the VisModels
            foreach (VisModel vm in Level.VisList)
            {
                vm.Draw();
            }

            //Debug Strings
            /*
            debugDisplay.addDebug("Current Speed: " + (Level.player.runSpeed + Level.player.accelSpeed));
            debugDisplay.addDebug("Current Linear  Velocity: " + (Level.player.physModel.phys.LinearVelocity));
            debugDisplay.addDebug("Current Position: " + Level.player.physModel.phys.Position);
            debugDisplay.addDebug("Last Platform: " + Level.player.lastColor );
            debugDisplay.addDebug("Shake Scale: " + Level.player.camera.shakeScale);
             */

            //Gui stuff
            
            

            //Debug Drawing
            Game1.spriteBatch.Begin();
            DrawAlignedString(scoreFont, "Record: " +Level.player.highscore.ToString(), new Rectangle(Game1.ScreenW-156,0,128,64), Alignment.Right, Color.Black);
            DrawAlignedString(scoreFont, "Score: " +Level.player.platScore.ToString(), new Rectangle(Game1.ScreenW-156,46,128,64), Alignment.Right, Color.Black);
            DrawAlignedString(scoreFont, "Combo: " + Level.player.combo.ToString(), new Rectangle(Game1.ScreenW-156,92,128,64), Alignment.Right, Color.Black);
            debugDisplay.Draw();
            Game1.spriteBatch.End();

        }

        //Draws Models
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
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xLightDirUp"].SetValue(lightDirUp);
                    currentEffect.Parameters["xLightDirDown"].SetValue(lightDirDown);
                    currentEffect.Parameters["xLightPower"].SetValue(lightPower);
                    currentEffect.Parameters["xAmbient"].SetValue(ambientPower);
                    currentEffect.Parameters["xColor"].SetValue(tint);
                    currentEffect.Parameters["xFogStart"].SetValue(650);
                    currentEffect.Parameters["xFogEnd"].SetValue(2000);
                    currentEffect.Parameters["xFogColor"].SetValue(new Vector3(1));
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(camera.viewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(camera.projectionMatrix);
                    currentEffect.Parameters["cameraPosition"].SetValue(camera.Position);

                }
                mesh.Draw();
            }
        }

        //Draws Sky Dome
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


        //Aligned Text
        public enum Alignment { Center = 0, Left = 1, Right = 2, Top = 4, Bottom = 8 }

        public static void DrawAlignedString(SpriteFont font, string text, Rectangle bounds, Alignment align, Color color)
        {
            Vector2 size = font.MeasureString(text);
            Vector2 pos = new Vector2(bounds.Center.X,bounds.Center.Y);
            Vector2 origin = size * 0.5f;

            if (align.HasFlag(Alignment.Left))
                origin.X += bounds.Width / 2 - size.X / 2;

            if (align.HasFlag(Alignment.Right))
                origin.X -= bounds.Width / 2 - size.X / 2;

            if (align.HasFlag(Alignment.Top))
                origin.Y += bounds.Height / 2 - size.Y / 2;

            if (align.HasFlag(Alignment.Bottom))
                origin.Y -= bounds.Height / 2 - size.Y / 2;

            Game1.spriteBatch.DrawString(font, text, pos, color, 0, origin, 1, SpriteEffects.None, 0);
        }
    }
}
