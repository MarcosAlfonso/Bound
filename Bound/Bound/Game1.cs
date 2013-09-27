using System;
using System.Collections.Generic;
using System.Linq;
using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ToBeDetermined
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public const int ScreenW = 1280;
        public const int ScreenH = 720;

        public static GraphicsDevice device;

        public static int content = content;

        public static Space space;

        public static Random r;
        
        public static List<PhysModel> PhysList = new List<PhysModel>();
        private PhysModel tester;

        public static PlayerController player;

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
            graphics.ApplyChanges();

            //graphics stuff
            device = graphics.GraphicsDevice;

            //Initialize rendering
            Render.Initialize();
            Render.effect = Content.Load<Effect>("HLSL"); 

            //Load Textures
            Render.testColumn = LoadModel(@"Models\testColumn", out Render.columnTextures);
            Render.cubeModel = LoadModel(@"Models\cubeModel", out Render.cubeTextures);
            Render.gridCube = LoadModel(@"Models\gridCube", out Render.gridTextures);
            Render.sphereModel = LoadModel(@"Models\sphereModel", out Render.cubeTextures);

            //Physics stuff
            space = new Space();

            var ground = new PhysModel(Vector3.Zero, new Vector3(5000, 5, 5000), 0, PhysModel.PhysType.Ground);
            ground.setColorTint(new Vector3(.24f,.5f, .25f));

            PhysList.Add(ground);

            space.ForceUpdater.Gravity = new Vector3(0, -120f, 0);

            player = new PlayerController(new Vector3(0, 20, 0));

            //How many rows of plats
            for (int j = 0; j < 60; j++)
            {
                //Picks which platforms will spawn, 50/50
                bool[] platDecide = {r.Next(0, 2) == 1, r.Next(0, 2) == 1, r.Next(0, 2) == 1, r.Next(0, 2) == 1};

                //If none are true, start over
                if (!platDecide[0] && !platDecide[1] && !platDecide[2] && !platDecide[3])
                {
                    platDecide[r.Next(0, 4)] = true;
                }

                //if at least one is true, do eet
                for (int i = 0; i < 4; i++)
                {
                    if (platDecide[i])
                    {
                        PhysModel phys = new PhysModel(new Vector3(-250 + 120*i, r.Next(20, 50), -250 + 250*j), new Vector3(50, 2, 70), 0, PhysModel.PhysType.Platform);
                        phys.setColorTint(new Vector3(1, 0.1f, 0.1f));
                        PhysList.Add(phys);
                    }
                }
            }


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

            player.Update();
            space.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Render.Draw();

            base.Draw(gameTime);
        }

        private Model LoadModel(string assetName, out Texture2D[] textures)
        {

            Model newModel = Content.Load<Model>(assetName);
            textures = new Texture2D[7];
            int i = 0;
            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                    textures[i++] = currentEffect.Texture;

            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = Render.effect.Clone();

            return newModel;
        }
        

       
    }
}
