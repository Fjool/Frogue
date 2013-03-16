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

        const Int16 SPRITE_SIZE = 32;
        const Int16 SPRITE_RENDER_SIZE = 32;
        
        public int Offset_X{ get; set; }
        public int Offset_Y{ get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        Boolean walking = false;
        int Animation_Frame = 0;
        double Animation_Rate = 250;
        double lastTime;

       public Map map{ get; set; }

        public Player()
        {
            Offset_X = 0;
            Offset_Y = 0;
        }

        public void LoadContent(ContentManager Content)
        {            
            tilemap = Content.Load<Texture2D>("wobbly_blob");            
        }

        public Int16 Speed()
        {
            return 1;
        }

        public void Move(Direction theDirection)
        {   
            if (!walking && map.maze.DirectionOpen(X, Y, theDirection))
            {
                walking = true;

                switch(theDirection)
                {
                    case Direction.North: Y--; break;
                    case Direction.South: Y++; break;
                    case Direction.East : X++; break;
                    case Direction.West : X--; break;
                    default: break;
                }                      
            
                // walks in full tile increments
                if (X < 0){ X = 0; };
                if (Y < 0){ Y = 0; };
                if (X > map.TilesWide()-1){ X = map.TilesWide()-1; }
                if (Y > map.TilesWide()-1){ Y = map.TilesHigh()-1; }            
            }
        }
 
        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds > lastTime + Animation_Rate)
            {                
                Animation_Frame++;
                if (Animation_Frame > 7) { Animation_Frame = 0; };                    

                lastTime = gameTime.TotalGameTime.TotalMilliseconds;

                walking = false;
            }
        }

        public void Render(SpriteBatch spriteBatch, int topX, int topY)
        {
            // render the portion of the buffer that we can see to the screen
            spriteBatch.Draw( tilemap
                            , new Rectangle( topX // X * map.TileSize() + Offset_X
                                           , topY // Y * map.TileSize() + Offset_Y
                                           , SPRITE_RENDER_SIZE
                                           , SPRITE_RENDER_SIZE
                                           )
                            , new Rectangle( Animation_Frame * SPRITE_SIZE, 0, SPRITE_SIZE, SPRITE_SIZE)   // ((int)facing * 96)+1
                            , Color.White
                            ); 
        }
    }
}
