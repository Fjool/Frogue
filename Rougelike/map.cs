using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Rougelike
{
    class Map
    {        
        Texture2D tilemap;
        RenderTarget2D map_buffer;

        const int TILE_SIZE = 64;
        const int MAP_WIDE = 100;
        const int MAP_HIGH = 100;
        
        const int MAP_WIDE_PIXELS = MAP_WIDE * TILE_SIZE;
        const int MAP_HIGH_PIXELS = MAP_HIGH * TILE_SIZE;

        Random rand1 = new Random();
        
        // Design a map         
        int[,] map ;

        GraphicsDevice graphics;
        SpriteBatch spriteBatch;

        public Map(GraphicsDevice graphicsIn)
        {            
            graphics = graphicsIn;

            spriteBatch = new SpriteBatch(graphics);
                         
            // make a buffer for the map rendering which is a tile wider and higher than the screen's viewport
            map_buffer = new RenderTarget2D(graphics, graphics.Viewport.Width + TILE_SIZE, graphics.Viewport.Height + TILE_SIZE);
        }

        public int PixelsWide()
        {
            return MAP_WIDE_PIXELS - graphics.Viewport.Width;
        }
        
        public int PixelsHigh()
        {
            return MAP_HIGH_PIXELS - graphics.Viewport.Height;
        }

        public Rectangle renderRegion(int View_X, int View_Y)
        {
            return new Rectangle(View_X % TILE_SIZE, View_Y % TILE_SIZE, graphics.Viewport.Width, graphics.Viewport.Height);
        }

        public Texture2D texture()
        {   return map_buffer;
        }
       
        public void LoadContent(ContentManager Content)
        {            
            tilemap = Content.Load<Texture2D>("grundvari tileset");            
        }

        private int CountTilesHigh() { return (tilemap.Height / TILE_SIZE);        }
        private int CountTilesWide() { return (tilemap.Width  / TILE_SIZE);        }
        private int NumTiles(      ) { return CountTilesHigh() * CountTilesWide(); }
                
        public void GenerateMap()
        {
            map = new int[MAP_WIDE, MAP_HIGH];

            for (int i = 0; i < MAP_WIDE; i++)
            {   for (int j = 0; j < MAP_HIGH; j++)
                { 
                    // allow the engine to choose from these particular tiles
                    int[] valid_tiles = {3, 13, 23, 63, 73, 83, 93, 103, 113, 131};
                    //int[] valid_tiles = {155};
                    List<int> valid_list = new List<int>(valid_tiles);

                    int chosen_tile = -1;

                    while (!valid_tiles.Contains(chosen_tile))
                    {                 
                        chosen_tile = rand1.Next(NumTiles());
                        
                    }

                    map[i,j] = chosen_tile;
                }
            }              
        }
        
        protected Rectangle GetMapTile(int i, int j)
        {
            return new Rectangle( ((map[i, j] % CountTilesWide()) * TILE_SIZE)
                                , ((map[i, j] / CountTilesWide()) * TILE_SIZE)
                                , TILE_SIZE
                                , TILE_SIZE
                                );            
        }

        public void Render(int View_X, int View_Y)
        {   
            int BufferTop_X = (View_X / TILE_SIZE);
            int BufferTop_Y = (View_Y / TILE_SIZE);
            
            graphics.SetRenderTarget(map_buffer);
            
            spriteBatch.Begin();
            
            // draw to map buffer
            for (int j = 0; j < (graphics.Viewport.Height / TILE_SIZE) + 1; j++)
            {
                for (int i = 0; i < (graphics.Viewport.Width / TILE_SIZE) + 1; i++)
                {   
                    if (  (BufferTop_X + i < MAP_WIDE)
                       && (BufferTop_Y + j < MAP_HIGH)
                       )
                    {
                        spriteBatch.Draw( tilemap
                                        , new Rectangle(i*TILE_SIZE, j*TILE_SIZE, TILE_SIZE, TILE_SIZE)
                                        , GetMapTile(BufferTop_X + i, BufferTop_Y + j)
                                        , Color.White
                                        );    
                    }
                }
            }

            spriteBatch.End();

            // reset output to back buffer
            graphics.SetRenderTarget(null);  
          
            spriteBatch.Begin();
             
            // render the portion of the buffer that we can see to the screen
            spriteBatch.Draw( map_buffer
                            , new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height)
                            , renderRegion(View_X, View_Y)
                            , Color.White
                            ); 

            spriteBatch.End();      
        }
    }
}
