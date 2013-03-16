using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Rougelike
{
    public enum Direction{ South = 0, West, North, East };
   
    public class Player
    {
        Texture2D tilemap;

        protected const Int16 SPRITE_SIZE = 32;
        
        public Vector2 loc;

        Boolean walking = false;
        int Animation_Frame = 0;
        
        double Animation_Rate = 250;
        double Move_Rate      = 50;
        
        double lastTime_Animate, lastTime_Move;

        public Map map{ get; set; }

        public Vector2 direction, distanceTravelled;

        public void LoadContent(ContentManager Content)
        {            
            tilemap = Content.Load<Texture2D>("wobbly_blob");            
        }

        public Int16 Speed()
        {
            return 5;
        }

        public void Move(Direction theDirection)
        {   
            if (!walking && map.maze.DirectionOpen(loc, theDirection))
            {
                walking = true;
                direction = Vector2.Zero;
               
                switch(theDirection)
                {
                    case Direction.North: direction.Y--; break;
                    case Direction.South: direction.Y++; break;
                    case Direction.East : direction.X++; break;
                    case Direction.West : direction.X--; break;
                    default: break;
                }                      
    
                if (direction != Vector2.Zero)
                {   direction.Normalize();
                }

                direction *= Speed();
            }
        }
 
        protected void UpdateMovement(GameTime gameTime)
        {
            if (walking && gameTime.TotalGameTime.TotalMilliseconds > lastTime_Move + Move_Rate)
            {                
                distanceTravelled += direction;

                if (distanceTravelled.Length() >= map.TileSize())
                {
                    var moved = (distanceTravelled / map.TileSize());
                    moved.Normalize();
                    
                    loc += moved;
                    
                    distanceTravelled = Vector2.Zero;

                    walking = false;                   
                }

                KeepPlayerOnMaze();
    
                lastTime_Move = gameTime.TotalGameTime.TotalMilliseconds;               
            }
        }

        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds > lastTime_Animate + Animation_Rate)
            {                
                Animation_Frame++;
                if (Animation_Frame > 7) { Animation_Frame = 0; };                    
                lastTime_Animate = gameTime.TotalGameTime.TotalMilliseconds;               
            }

            UpdateMovement(gameTime);    
        }

        public void KeepPlayerOnMaze()
        {            
            // prevent from leaving the maze
            if (loc.X < 0){ loc.X = 0; };
            if (loc.Y < 0){ loc.Y = 0; };
            if (loc.X > map.dimensions.X-1){ loc.X = map.dimensions.X-1; }
            if (loc.Y > map.dimensions.Y-1){ loc.Y = map.dimensions.Y-1; }                   
        }
        
        protected Rectangle RenderRectangle()
        {
            var renderLoc = loc * map.TileSize() + distanceTravelled;            
            
            return new Rectangle( (int)renderLoc.X
                                , (int)renderLoc.Y
                                , SPRITE_SIZE
                                , SPRITE_SIZE
                                );
        }
        
        public void Render(SpriteBatch spriteBatch, Vector2 renderLoc)
        {
            renderLoc = renderLoc + distanceTravelled;            
            
            spriteBatch.Draw( tilemap
                            , new Rectangle( (int)renderLoc.X
                                           , (int)renderLoc.Y
                                           , SPRITE_SIZE
                                           , SPRITE_SIZE
                                           )
                            , new Rectangle( Animation_Frame * SPRITE_SIZE, 0, SPRITE_SIZE, SPRITE_SIZE)
                            , Color.White
                            ); 
        }
    }
}