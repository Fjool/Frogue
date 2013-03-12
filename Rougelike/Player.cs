using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Rougelike
{
    class Player
    {
        Texture2D tilemap;
        SpriteBatch spriteBatch;
        GraphicsDevice graphics;
        Map map;

        const Int16 SPRITE_SIZE = 96;
              
        public int Loc_X{ get; set; }
        public int Loc_Y{ get; set; }

        enum Direction{ South = 0, West, North, East };
        Direction facing;
        Boolean walking = false;
        int Animation_Frame = 0;
        double Animation_Rate = 100;
        double lastTime;

        public Player(GraphicsDevice graphicsIn, Map mapIn)
        {
            graphics = graphicsIn;
            spriteBatch = new SpriteBatch(graphics);
            map = mapIn;
            Loc_X = 0;
            Loc_Y = 0;
            facing = Direction.North;
        }

        public void LoadContent(ContentManager Content)
        {            
            tilemap = Content.Load<Texture2D>("KatieWalk");            
        }

        public Int16 Speed()
        {
            return 5;
        }

        public void ProcessKeys(KeyboardState KeyState)
        {            
            walking = false;

            if (KeyState.IsKeyDown(Keys.W))
            {   Loc_Y -= Speed();

                if (Loc_Y < 0){ Loc_Y = 0; };

                facing = Direction.North;
                walking = true;
            }
            else if (KeyState.IsKeyDown(Keys.S))
            {   Loc_Y += Speed();

                if (Loc_Y >= map.PixelsHigh()){ Loc_Y = map.PixelsHigh(); };

                facing = Direction.South;
                walking = true;
            }
            
            if (KeyState.IsKeyDown(Keys.A)) 
            {   Loc_X -= Speed();
            
                if (Loc_X < 0){ Loc_X = 0; };

                facing = Direction.West;
                walking = true;
            }
            else if (KeyState.IsKeyDown(Keys.D))
            {   Loc_X += Speed();

                if (Loc_X >= map.PixelsWide()){ Loc_X = map.PixelsWide(); };

                facing = Direction.East;
                walking = true;
            }
        }

        public void Update(GameTime gameTime, KeyboardState KeyState)
        {
            ProcessKeys(KeyState);
            
            if (!walking)
            {
                Animation_Frame = 0;
            }
            else if (gameTime.TotalGameTime.TotalMilliseconds > lastTime + Animation_Rate)
            {               
                Animation_Frame++;
                if (Animation_Frame > 8) { Animation_Frame = 1; };
                lastTime = gameTime.TotalGameTime.TotalMilliseconds;
            }
        }

        public void Render()
        {
            spriteBatch.Begin();

            // render the portion of the buffer that we can see to the screen
            spriteBatch.Draw( tilemap
                            , new Rectangle( (graphics.Viewport.Width  / 2) - (SPRITE_SIZE/2)
                                           , (graphics.Viewport.Height / 2) - (SPRITE_SIZE/2) 
                                           , SPRITE_SIZE
                                           , SPRITE_SIZE
                                           )
                            , new Rectangle( Animation_Frame * SPRITE_SIZE, ((int)facing * 96)+1, SPRITE_SIZE, SPRITE_SIZE)
                            , Color.White
                            ); 

            spriteBatch.End();  
        }
    }
}
