using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace TileEngine
{
    public class Layer
    {
        public int[,] data;
        Texture2D tilemap;

        public int TileSize{ get; set; }
        Vector2 dimensions;
       
        private int CountTilesHigh() { return (tilemap.Height / TileSize); }
        private int CountTilesWide() { return (tilemap.Width  / TileSize); }
        
        public Layer(Vector2 dimensions_In)
        {
            dimensions = dimensions_In;
            data = new int[(int)dimensions.X, (int)dimensions.Y];
        }

        public void LoadTiles(ContentManager Content, String ResourceName)
        {            
            tilemap = Content.Load<Texture2D>(ResourceName);            
        }
        
        protected Rectangle GetMapTile(int i, int j)
        {                   
            return new Rectangle( ((data[i, j] % CountTilesWide()) * TileSize)
                                , ((data[i, j] / CountTilesWide()) * TileSize)
                                , TileSize
                                , TileSize
                                );            
        } 

        public void Render(Vector2 camera, RenderTarget2D map_buffer, Rectangle Area, SpriteBatch spriteBatch, Vector2 offset)
        {                
            var mapLoc = camera / TileSize;

            // draw to map buffer
            for (int j = 0; j < (Area.Height / TileSize) + 1; j++)
            {
                for (int i = 0; i < (Area.Width / TileSize) + 1; i++)
                {   
                    if (  (mapLoc.X + i < dimensions.X)
                       && (mapLoc.Y + j < dimensions.Y)
                       )
                    {
                        spriteBatch.Draw( tilemap
                                        , new Rectangle( i * TileSize+(int)offset.X
                                                       , j * TileSize+(int)offset.Y
                                                       , TileSize
                                                       , TileSize
                                                       )
                                        , GetMapTile((int)mapLoc.X + i, (int)mapLoc.Y + j)
                                        , Color.White
                                        );    
                    }
                }
            }
        }
    }
}
