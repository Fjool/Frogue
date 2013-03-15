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
    enum Direction{ South = 0, West, North, East };

    class Player
    {
        Texture2D tilemap;
        SpriteBatch spriteBatch;
        GraphicsDevice graphics;
        Map map;

        const Int16 SPRITE_SIZE = 96;
        const Int16 SPRITE_RENDER_SIZE = 32;
        
        public int Offset_X{ get; set; }
        public int Offset_Y{ get; set; }

        public int Map_X { get; set; }
        public int Map_Y { get; set; }

        Direction facing;

        Boolean walking = false;
        int Animation_Frame = 0;
        double Animation_Rate = 50;
        double lastTime;

        public Player(GraphicsDevice graphicsIn, Map mapIn)
        {
            graphics = graphicsIn;
            spriteBatch = new SpriteBatch(graphics);
            map = mapIn;
            Offset_X = 0;
            Offset_Y = 0;
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

        public void Move(Direction theDirection)
        {   
            if (!walking)
            {                
                facing = theDirection;
                walking = map.maze.DirectionOpen(Map_X, Map_Y, theDirection);               
            }               
        }

        public void MovePlayer()
        {   
            if (walking)
            {               
                switch(facing)
                {
                    case Direction.North: Offset_Y -= Speed(); break;
                    case Direction.South: Offset_Y += Speed(); break;
                    case Direction.East : Offset_X += Speed(); break;
                    case Direction.West : Offset_X -= Speed(); break;
                    default: break;
                }                             
            }

            // walks in full tile increments
            if (Offset_X < (0-map.TileSize())){ walking = false; Offset_X = 0; Map_X--;};
            if (Offset_Y < (0-map.TileSize())){ walking = false; Offset_Y = 0; Map_Y--;};
            if (Offset_X > (  map.TileSize())){ walking = false; Offset_X = 0; Map_X++;};
            if (Offset_Y > (  map.TileSize())){ walking = false; Offset_Y = 0; Map_Y++;};                            
        }
        
        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds > lastTime + Animation_Rate)
            {
                MovePlayer();
            
                if (!walking)
                {   Animation_Frame = 0;    // causes a stutter between tiles, needs fixing
                }
                else
                {               
                    Animation_Frame++;
                    if (Animation_Frame > 8) { Animation_Frame = 1; };                    
                }

                lastTime = gameTime.TotalGameTime.TotalMilliseconds;
            }
        }

        public void Render()
        {
            spriteBatch.Begin();

            // render the portion of the buffer that we can see to the screen
            spriteBatch.Draw( tilemap
                            , new Rectangle( Map_X * map.TileSize() + Offset_X
                                           , Map_Y * map.TileSize() + Offset_Y
                                           , SPRITE_RENDER_SIZE
                                           , SPRITE_RENDER_SIZE
                                           )
                            , new Rectangle( Animation_Frame * SPRITE_SIZE, ((int)facing * 96)+1, SPRITE_SIZE, SPRITE_SIZE)
                            , Color.White
                            ); 

            spriteBatch.End();  
        }
    }
}
