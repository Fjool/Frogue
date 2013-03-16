using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Rougelike
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const Boolean IS_DEBUG = true;
        
        Map        map;
        Player     player;
        SpriteFont font;
        Vector2    camera = Vector2.Zero;

        int frameRate = 0;
        int frameCounter = 0;

        TimeSpan elapsedTime = TimeSpan.Zero;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 480;
            graphics.IsFullScreen = !IS_DEBUG;            
            graphics.ApplyChanges();

            Window.Title = "Fwarkk's Roguelike";

            player = new Player();
            map    = new Map   (graphics.GraphicsDevice, player);

            player.X = 1;
            player.Y = 1;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("SpriteFont1");

            map.LoadContent(Content);
            map.GenerateMap();

            player.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState KeyState = Keyboard.GetState(PlayerIndex.One);

            if (KeyState.IsKeyDown(Keys.Escape)) { this.Exit();       }   // Allows the game to exit            
            if (KeyState.IsKeyDown(Keys.Space )) { map.GenerateMap(); } 
            
            if (KeyState.IsKeyDown(Keys.W)) { player.Move(Direction.North); }
            if (KeyState.IsKeyDown(Keys.S)) { player.Move(Direction.South); }
            if (KeyState.IsKeyDown(Keys.A)) { player.Move(Direction.West ); }
            if (KeyState.IsKeyDown(Keys.D)) { player.Move(Direction.East ); }
            
            player.Update(gameTime);

            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
            
            base.Update(gameTime);
        }

        protected void positionCameraAbovePlayer()
        {            
            camera.X = (player.X * map.TileSize()) - (graphics.GraphicsDevice.Viewport.Width / 2);
            camera.Y = (player.Y * map.TileSize()) - (graphics.GraphicsDevice.Viewport.Height / 2);

            if (camera.X <                0) { camera.X = 0;                }
            if (camera.Y <                0) { camera.Y = 0;                }
            if (camera.X > map.PixelsWide()) { camera.X = map.PixelsWide(); }
            if (camera.Y > map.PixelsHigh()) { camera.Y = map.PixelsHigh(); }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {   
            positionCameraAbovePlayer();

            map.Render(camera);  
            
            if (IS_DEBUG)
            {
                frameCounter++;

                String FrameString = string.Format("FPS: {0}", frameRate);

                spriteBatch.Begin();
                spriteBatch.DrawString(font, FrameString, new Vector2(30, 30), Color.Black);
                spriteBatch.DrawString(font, FrameString, new Vector2(29, 29), Color.Red  );
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}