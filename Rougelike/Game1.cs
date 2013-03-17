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
        protected GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const Boolean IS_DEBUG = true;
        
        SpriteFont font;

        protected Vector2 camera = Vector2.Zero;
        protected Map     map;
        protected Player  player;
        
        int frameRate = 0;
        int frameCounter = 0;

        TimeSpan elapsedTime = TimeSpan.Zero;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected int ScreenWide{ get; set; }
        protected int ScreenHigh{ get; set; }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ScreenWide = 640;
            ScreenHigh = 480;

            graphics.PreferredBackBufferWidth = ScreenWide;
            graphics.PreferredBackBufferHeight = ScreenHigh;
            graphics.IsFullScreen = !IS_DEBUG;            
            graphics.ApplyChanges();

            Window.Title = "Fwarkk's Roguelike";
            
            CreateEntities(new Vector2(30,20));

            base.Initialize();
        }

        protected void CreateEntities(Vector2 MapSize)
        {            
            var dimensions = MapSize;

            player = new Player(dimensions);
            map    = new Map   (graphics.GraphicsDevice, player, dimensions);

            player.loc = new Vector2(1,1); // Vector2.One;            
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
            camera = (player.loc * map.TileSize + player.distanceTravelled);

            camera.X -= ScreenWide / 2;
            camera.Y -= ScreenHigh / 2;
            
            int PixelsWide = (int)(map.dimensions.X * map.TileSize) - ScreenWide;
            int PixelsHigh = (int)(map.dimensions.Y * map.TileSize) - ScreenHigh;

            if (camera.X <          0) { camera.X = 0;          }
            if (camera.Y <          0) { camera.Y = 0;          }
            if (camera.X > PixelsWide) { camera.X = PixelsWide; }
            if (camera.Y > PixelsHigh) { camera.Y = PixelsHigh; }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {   
            positionCameraAbovePlayer();
            map.Render(camera);  
            
            if (player.loc == map.exit.loc)
            {
                String WINN4R = "winnar is u!";

                spriteBatch.Begin();
                spriteBatch.DrawString(font, WINN4R, new Vector2((graphics.GraphicsDevice.Viewport.Width/2)  , (graphics.GraphicsDevice.Viewport.Height/2))  , Color.Black);
                spriteBatch.DrawString(font, WINN4R, new Vector2((graphics.GraphicsDevice.Viewport.Width/2)-1, (graphics.GraphicsDevice.Viewport.Height/2)-1), Color.Red  );
                spriteBatch.End();
            }

            if (IS_DEBUG)
            {
                frameCounter++;

                String FrameString = string.Format("FPS: {0}", frameRate);

                spriteBatch.Begin();
                spriteBatch.DrawString(font, FrameString, new Vector2(30, 30), Color.Black);
                spriteBatch.DrawString(font, FrameString, new Vector2(29, 29), Color.Red  );
                
                String PlayerString = string.Format("Player: {0}", player.loc);
                spriteBatch.DrawString(font, PlayerString, new Vector2(30, 60), Color.Black);
                spriteBatch.DrawString(font, PlayerString, new Vector2(29, 59), Color.Red  );
                
                PlayerString = string.Format("Camera: {0}", camera);
                spriteBatch.DrawString(font, PlayerString, new Vector2(30, 90), Color.Black);
                spriteBatch.DrawString(font, PlayerString, new Vector2(29, 89), Color.Red  );
                
                PlayerString = string.Format("Player loc in pixels: {0}", player.loc * map.TileSize + player.distanceTravelled);
                spriteBatch.DrawString(font, PlayerString, new Vector2(30, 120), Color.Black);
                spriteBatch.DrawString(font, PlayerString, new Vector2(29, 119), Color.Red  );
                
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}