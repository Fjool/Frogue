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
    static class List
    {
        // non-destructive shuffle
        public static List<T> Shuffle<T>(this IList<T> list, Random rand)  
        {  
            var answer = new List<T>();

            for (int n = list.Count - 1; n >= 0; n--)
            {   answer.Add(list[n]);
            }

            for (int n = list.Count - 1; n > 0; n--) // Note: Fisher-Yates shuffle
            {            
                int k = rand.Next(n + 1);
                T tmp = answer[k];
                answer[k] = answer[n];
                answer[n] = tmp;
            }

            return answer;
        }
    }

    class Cell
    {
        public List<Cell> Connections;

        public UInt16    i { get; set; }
        public UInt16    j { get; set; }        
        public UInt16 Type { get; set; }

        public static UInt16 CELL_EMPTY = 8;
        public static UInt16 CELL_WALL  = 4;

        // either East or South can be open
        Boolean[] Walls = {true, true}; //, true, true};

        public Cell()
        {     
            Connections = new List<Cell>();
            Type = CELL_WALL;
        }

        // a cell has all walls if its connections are all uncarved
        public Boolean HasAllWalls()
        {      
            return (  (Type == CELL_WALL)
                   && (Connections.Count(c => c.Type == CELL_WALL) >= 3)
                   );
        }
    }
        
    class Maze
    {   
        public Cell[,] grid;

        public UInt16 Wide() { return (UInt16)grid.GetLength(0); }
        public UInt16 High() { return (UInt16)grid.GetLength(1); }
        public UInt16 PassageLength{ get; set; }

        Random rand = new Random();              

        public Maze(UInt16 Wide, UInt16 High, UInt16 PassageLength_In)
        {
            grid = new Cell[Wide, High];        
            PassageLength = PassageLength_In;
        }
        
        public void CarveCell(Cell cell, UInt16 Steps, Direction theDirection, Cell source)
        {                  
            if (cell.HasAllWalls())
            {
                Steps++;
                cell.Type = Cell.CELL_EMPTY;  // empty

                if (Steps < PassageLength)
                {   
                    CarveCell(cell.Connections[(int)theDirection], Steps, theDirection, cell);                  
                }
                else
                {   // choose a new direction
                    var Neighbours = Enumerable.Range(0, 4).ToArray().Shuffle(rand);
                    
                    foreach (int  i in Neighbours)
                    {   if (cell.Connections[i] != source)
                        {   CarveCell(cell.Connections[i], 0, (Direction)i, cell);                        
                        }
                    }    
                }
            }
        }

        public void Generate()
        {   
            UInt16 i, j;

            // create a grid of empty cells
            for (i = 0; i < Wide(); i++)
            {   for (j = 0; j < High(); j++)
                {   grid[i,j] = new Cell();
                    grid[i,j].i = i;
                    grid[i,j].j = j;
                }
            }

            // connect adjacent cells
            for (i = 0; i < Wide(); i++)
            {   for (j = 0; j < High(); j++)
                {   
                    if (i >          0){ grid[i,j].Connections.Add(grid[i-1, j  ]); }
                    if (j >          0){ grid[i,j].Connections.Add(grid[  i, j-1]); }
                    if (i + 1 < Wide()){ grid[i,j].Connections.Add(grid[i+1, j  ]); }
                    if (j + 1 < High()){ grid[i,j].Connections.Add(grid[  i, j+1]); }
                }
            }

            CarveCell( grid[ 1
                           , 1
                           ]
                     , 0
                     , Direction.East
                     , null
                     ); 
        }
        
        public Boolean DirectionOpen(int x, int y, Direction theDirection)
        {
            var isValid = false;

            // find the map tile            
            switch (theDirection)
            {                    
                case Direction.North: y--; break;
                case Direction.South: y++; break;
                case Direction.East : x++; break;
                case Direction.West : x--; break;
            }

            isValid = (x >= 0) && (x < Wide())
                   && (y >= 0) && (y < High());

            return (isValid && grid[x,y].Type == Cell.CELL_EMPTY);            
        }       
    }

    class Map
    {        
        Texture2D tilemap;
        RenderTarget2D map_buffer;

        const int TILE_SIZE = 32;

        const int MAP_WIDE = 60;
        const int MAP_HIGH = 33;
        const int PASSAGE_LENGTH = 2;

        const int MAP_WIDE_PIXELS = MAP_WIDE * TILE_SIZE;
        const int MAP_HIGH_PIXELS = MAP_HIGH * TILE_SIZE;
        
        Random rand1 = new Random();
        
        public Maze maze;

        GraphicsDevice graphics;
        SpriteBatch spriteBatch;

        public Map(GraphicsDevice graphicsIn)
        {            
            graphics = graphicsIn;

            spriteBatch = new SpriteBatch(graphics);
                         
            // make a buffer for the map rendering which is a tile wider and higher than the screen's viewport
            map_buffer = new RenderTarget2D(graphics, graphics.Viewport.Width + TILE_SIZE, graphics.Viewport.Height + TILE_SIZE);
        }

        public int TileSize(){ return TILE_SIZE; }

        public int PixelsWide() { return MAP_WIDE_PIXELS - graphics.Viewport.Width;  }        
        public int PixelsHigh() { return MAP_HIGH_PIXELS - graphics.Viewport.Height; }

        public int ScreenTilesWide() { return (graphics.Viewport.Width  / TILE_SIZE); }
        public int ScreenTilesHigh() { return (graphics.Viewport.Height / TILE_SIZE); }

        public Rectangle renderRegion(int View_X, int View_Y)
        {
            return new Rectangle(View_X % TILE_SIZE, View_Y % TILE_SIZE, graphics.Viewport.Width, graphics.Viewport.Height);
        }

        public Texture2D texture()
        {   return map_buffer;
        }
       
        public void LoadContent(ContentManager Content)
        {            
            tilemap = Content.Load<Texture2D>("DungeonStyle1");            
        }

        private int CountTilesHigh() { return (tilemap.Height / TILE_SIZE);        }
        private int CountTilesWide() { return (tilemap.Width  / TILE_SIZE);        }
        private int NumTiles(      ) { return CountTilesHigh() * CountTilesWide(); }
      
        public void GenerateMap()
        {
            maze = new Maze(MAP_WIDE, MAP_HIGH, PASSAGE_LENGTH);
            maze.Generate();            
        }

        protected Rectangle GetMapTile(int i, int j)
        {                   
            return new Rectangle( ((maze.grid[i, j].Type % CountTilesWide()) * TILE_SIZE)
                                , ((maze.grid[i, j].Type / CountTilesWide()) * TILE_SIZE)
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
