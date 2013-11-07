using System;
using System.Collections.Generic;
using System.Linq;
using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using Bound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Bound
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;

        public const int ScreenW = 1280;
        public const int ScreenH = 720;

        public static GraphicsDevice device;

        public static int content = content;

        public static Space space;

        public static Random r;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            r = new Random();

            //Sets window size
            graphics.PreferredBackBufferWidth = ScreenW;
            graphics.PreferredBackBufferHeight = ScreenH;
            graphics.PreferMultiSampling = true;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            //graphics stuff
            device = graphics.GraphicsDevice;

            //Initialize rendering
            Render.Initialize();
            Render.effect = Content.Load<Effect>("HLSL"); 

            //Load Shit
            Render.cubeModel = LoadModel(@"Models\cubeModel");
            Render.sphereModel = LoadModel(@"Models\sphereModel");
            Render.platformModel = LoadModel(@"Models\platformModel");
            Render.skyDomeModel = LoadModel(@"Models\skyboxModel");
            Render.debugFont = Content.Load<SpriteFont>("debugFont");

            //Physics stuff
            space = new Space();
            space.ForceUpdater.Gravity = new Vector3(0, -120f, 0);

            Level.Initialize();

            //Sets mouse to center from start
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            Input.lastMs = Mouse.GetState();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

            //If active window, do input
            if (IsActive)
                Input.Update(timeDifference);

            Level.player.Update();
            space.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Render.Draw();

            base.Draw(gameTime);
        }

        private Model LoadModel(string assetName)
        {

            Model newModel = Content.Load<Model>(assetName);

            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = Render.effect.Clone();

            return newModel;
        }
        

       
    }
}
