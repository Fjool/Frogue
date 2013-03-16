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

    //--------------------------------------------------------------------------
    public class Cell
    {
        public List<Cell> Connections;

        public UInt16    i { get; set; }
        public UInt16    j { get; set; }        
        public UInt16 Type { get; set; }

        public static UInt16 CELL_EMPTY = 5;
        public static UInt16 CELL_WALL  = 4;
        public static UInt16 CELL_DOOR  = 0;

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
        
    //--------------------------------------------------------------------------    
    public class Maze
    {   
        public Cell[,] grid;

        public UInt16 Wide() { return (UInt16)grid.GetLength(0); }
        public UInt16 High() { return (UInt16)grid.GetLength(1); }
        public UInt16 PassageLength{ get; set; }

        public UInt16 currentDepth, deepest;
        public Cell deepestCell;

        public int dist;

        Random rand = new Random();              

        Vector2 dimensions;

        public Maze(Vector2 dimensions_In, UInt16 PassageLength_In)
        {
            dimensions = dimensions_In;

            grid = new Cell[(int)dimensions.X, (int)dimensions.Y];        
            PassageLength = PassageLength_In;
        }
        
        public void CarveCell(Cell cell, UInt16 Steps, Direction theDirection, Cell source)
        {                  
            if (cell.HasAllWalls())
            {
                currentDepth++;   

                if (currentDepth > deepest)
                {   deepestCell = cell;
                }

                Steps++;
                cell.Type = Cell.CELL_EMPTY; 

                if (Steps < PassageLength)
                {
                    CarveCell(cell.Connections[(int)theDirection], Steps, theDirection, cell);                        
                }
                else
                {
                    // choose a new direction
                    var Neighbours = Enumerable.Range(0, 4).ToArray().Shuffle(rand);
                    
                    foreach (int  i in Neighbours)
                    {   if (cell.Connections[i] != source)
                        {   CarveCell(cell.Connections[i], 0, (Direction)i, cell);                        
                        }
                    }            
                }

                currentDepth--;
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

            currentDepth = 0;
            deepest      = 0;
            deepestCell  = null;

            CarveCell( grid[ 1
                           , 1
                           ]
                     , 0
                     , Direction.East
                     , null
                     );

            grid[1,1].Type = Cell.CELL_DOOR;
            deepestCell.Type = Cell.CELL_DOOR;
        }
        
        public Boolean DirectionOpen(Vector2 loc, Direction theDirection)
        {
            var isValid = false;

            // find the map tile            
            switch (theDirection)
            {                    
                case Direction.North: loc.Y--; break;
                case Direction.South: loc.Y++; break;
                case Direction.East : loc.X++; break;
                case Direction.West : loc.X--; break;
            }

            isValid = (loc.Y >= 0) && (loc.X < Wide())
                   && (loc.Y >= 0) && (loc.Y < High());

            return (isValid && ( grid[(int)loc.X, (int)loc.Y].Type == Cell.CELL_EMPTY
                              || grid[(int)loc.Y, (int)loc.Y].Type == Cell.CELL_DOOR
                               )
                   );            
        }       
    }
   
    //--------------------------------------------------------------------------
    public class Map
    {        
        Texture2D tilemap;
        RenderTarget2D map_buffer;

        const int TILE_SIZE = 32;

        //const int MAP_WIDE = 100;
        //const int MAP_HIGH = 100;
        const int PASSAGE_LENGTH = 2;

        //const int MAP_WIDE_PIXELS = MAP_WIDE * TILE_SIZE;
        //const int MAP_HIGH_PIXELS = MAP_HIGH * TILE_SIZE;
        
        Random rand1 = new Random();
        
        public Maze maze;

        GraphicsDevice graphics;
        SpriteBatch spriteBatch;

        Player player;

        public Vector2 dimensions;

        public Map(GraphicsDevice graphics_In, Player player_In, Vector2 dimensions_In)
        {   
            if (graphics_In != null)
            {
                graphics = graphics_In;
                spriteBatch = new SpriteBatch(graphics);
            
                // make a buffer for the map rendering which is a tile wider and higher than the screen's viewport
                map_buffer = new RenderTarget2D(graphics, graphics.Viewport.Width + TILE_SIZE, graphics.Viewport.Height + TILE_SIZE);
            }
         
            player = player_In;
            player.map = this;

            dimensions = dimensions_In;
        }

        public int TileSize(){ return TILE_SIZE; }

        public int ScreenTilesWide() { return (graphics.Viewport.Width  / TILE_SIZE); }
        public int ScreenTilesHigh() { return (graphics.Viewport.Height / TILE_SIZE); }
        
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
            maze = new Maze(dimensions, PASSAGE_LENGTH);
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
       
        public void Render(Vector2 camera)
        {     
            var mapLoc = camera / TILE_SIZE;
            var renderLoc = Vector2.Zero;

            graphics.SetRenderTarget(map_buffer);
            
            spriteBatch.Begin();
            
            // draw to map buffer
            for (int j = 0; j < (graphics.Viewport.Height / TILE_SIZE) + 1; j++)
            {
                for (int i = 0; i < (graphics.Viewport.Width / TILE_SIZE) + 1; i++)
                {   
                    if (  (mapLoc.X + i < dimensions.X)
                       && (mapLoc.Y + j < dimensions.Y)
                       )
                    {
                        spriteBatch.Draw( tilemap
                                        , new Rectangle(i*TILE_SIZE, j*TILE_SIZE, TILE_SIZE, TILE_SIZE)
                                        , GetMapTile((int)mapLoc.X + i, (int)mapLoc.Y + j)
                                        , Color.White
                                        );    

                        if (player.loc == new Vector2((int)mapLoc.X + i, (int)mapLoc.Y + j))
                        {   renderLoc = new Vector2(i*TILE_SIZE, j*TILE_SIZE);
                        }
                    }
                }
            }

            player.Render(spriteBatch, renderLoc);

            spriteBatch.End();

            // reset output to back buffer
            graphics.SetRenderTarget(null);  
          
            spriteBatch.Begin();
             
            // render the portion of the buffer that we can see to the screen
            spriteBatch.Draw( map_buffer
                            , new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height)
                            , new Rectangle( (int)(camera.X % (float)TILE_SIZE)
                                           , (int)(camera.Y % (float)TILE_SIZE)
                                           , graphics.Viewport.Width
                                           , graphics.Viewport.Height
                                           )
                            , Color.White
                            );
 
            spriteBatch.End();      
        }
    }
}
