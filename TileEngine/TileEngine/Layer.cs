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
        public Vector2 dimensions;
       
        private int CountTilesHigh() { return (tilemap.Height / TileSize); }
        private int CountTilesWide() { return (tilemap.Width  / TileSize); }
        
        // set all elements of the layer to empty
        public void SetAllCellsTo(int Value)
        {   for (int i=0; i<(int)dimensions.X; i++)
                for (int j=0; j<(int)dimensions.Y; j++)
                {   data[i,j] = Value;
                }
        }

        public void DrawRectangle(Rectangle rect, int Value)
        {
            rect.X      = Math.Max(0, rect.X);
            rect.Y      = Math.Max(0, rect.Y);
            
            for (int i = rect.X; i < rect.X + rect.Width; i++)
            {                   
                for (int j = rect.Y; j < rect.Y + rect.Height; j++)
                {   
                    if (i < (int)dimensions.X
                    &&  j < (int)dimensions.Y)
                    {
                        data[i, j] = Value;              
                    }
                }
            }
        }

        public void FadeAllCellsBy(int Value, int min, int max)
        {   for (int i=0; i<(int)dimensions.X; i++)
                for (int j=0; j<(int)dimensions.Y; j++)
                {   data[i,j] -= Value;

                    if (data[i,j] < min) { data[i,j] = min; }
                    if (data[i,j] > max) { data[i,j] = max; }
                }
        }

        public Layer(Vector2 dimensions_In, int defaultTile)
        {
            dimensions = dimensions_In;
            data = new int[(int)dimensions.X, (int)dimensions.Y];

            SetAllCellsTo(defaultTile);            
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

        public void Render(Vector2 camera, RenderTarget2D map_buffer, Rectangle Area, SpriteBatch spriteBatch, Vector2 offset, Boolean AsFog)
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
                        if (!AsFog)
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
                        else
                        {   spriteBatch.Draw( tilemap
                                            , new Rectangle( i * TileSize+(int)offset.X
                                                           , j * TileSize+(int)offset.Y
                                                           , TileSize
                                                           , TileSize
                                                           )
                                            , GetMapTile(0, 0)  // fog always uses the first tile at the moment
                                            , new Color(new Vector4(1f,1f,1f,((float)data[(int)mapLoc.X + i,(int)mapLoc.Y + j] / 256.0f)))
                                            );    
                        }

                    }
                }
            }
        }
    }
}
