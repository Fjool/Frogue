using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TileEngine;

namespace Rougelike
{
    public enum Direction{ South = 0, West, North, East };
   
    public class Player
    {
        Layer playerLayer;

        public Vector2 loc;

        Boolean walking = false;
        int Animation_Frame = 0;
        
        double Animation_Rate = 250;
        double Move_Rate      = 50;
        
        double lastTime_Animate, lastTime_Move;

        public Map map{ get; set; }

        public Vector2 direction, distanceTravelled;
        public Vector2 dimensions;

        public int TileSize { get; set; }

        public Player(Vector2 dimensions_In)
        {
            TileSize = 32;

            dimensions = dimensions_In;
            playerLayer = new Layer(dimensions, -1);  
            playerLayer.TileSize = TileSize;
        }

        public void LoadContent(ContentManager Content)
        {            
            playerLayer.LoadTiles(Content, "wobbly_blob");
        }

        public Int16 Speed()
        {
            return 10;
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
 
        protected void UpdateFogMap()
        {
            map.fogLayer.FadeAllCellsBy(-16, 0, 256);

            int playerX = (int)loc.X;
            int playerY = (int)loc.Y;
            
            // set the fog map to full bright (0) for  our presence
            map.fogLayer.data[playerX, playerY] = 0;

            // set to half bright for adjacent cells (4 dirs)
            if (playerX     > 0               ) { map.fogLayer.data[playerX-1, playerY  ] -= 128; };
            if (playerY     > 0               ) { map.fogLayer.data[playerX  , playerY-1] -= 128; };
            if (playerX + 1 < map.dimensions.X) { map.fogLayer.data[playerX+1, playerY  ] -= 128; };
            if (playerY + 1 < map.dimensions.Y) { map.fogLayer.data[playerX  , playerY+1] -= 128; };

            // set to quarter bright for corner cells
            if (playerX > 0) 
            {   if (playerY > 0)
                {   map.fogLayer.data[playerX-1, playerY-1] -= 64; 
                }
                
                if (playerY + 1 < map.dimensions.X)
                {   map.fogLayer.data[playerX-1, playerY+1] -= 64; 
                }
            }
                
            if (playerX + 1 < map.dimensions.X)
            {   if (playerY > 0)
                {   map.fogLayer.data[playerX+1, playerY-1] -= 64; 
                }
                
                if (playerY + 1 < map.dimensions.X)
                {   map.fogLayer.data[playerX+1, playerY+1] -= 64; 
                };
            }    
        }

        protected void UpdateMovement(GameTime gameTime)
        {
            if (walking && gameTime.TotalGameTime.TotalMilliseconds > lastTime_Move + Move_Rate)
            {                
                distanceTravelled += direction;

                if (distanceTravelled.Length() >= map.TileSize)
                {
                    var moved = (distanceTravelled / map.TileSize);
                    moved.Normalize();
                    
                    loc += moved;
                    
                    distanceTravelled = Vector2.Zero;
                    
                    walking = false;                   
                }

                KeepPlayerOnMaze();

                UpdateFogMap();
 
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

        protected void GenerateLayer()
        {
            playerLayer.SetAllCellsTo(-1);

            // draw the player's sprite in the appropriate cell, at the appropriate animation phase
            playerLayer.data[(int)loc.X, (int)loc.Y] = Animation_Frame;                                       
        }

        public void Render(Vector2 map_loc, RenderTarget2D map_buffer, Rectangle Area, SpriteBatch spriteBatch)
        {
            GenerateLayer();
            playerLayer.Render(map_loc, map_buffer, Area, spriteBatch, distanceTravelled, false);            
        }
    }
}