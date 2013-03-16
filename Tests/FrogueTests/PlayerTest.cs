using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;
using Rougelike;

namespace FrogueTests
{

    //--------------------------------------------------------------------------
    [TestFixture]
    public class TestPlayer: Player
    { 
        [SetUp] 
        public void Init()
        {           
            map = new Map(null, this, new Vector2(2,2));  
            loc = Vector2.Zero;
        }

        [Test]
        public void PlayerStartsAtZero()
        {   Assert.AreEqual(Vector2.Zero, loc);   
        }

        [Test]
        public void Does_KeepPlayerOnMaze_Work()
        {   
            // position player before the start of the map
            loc.X = -1;
            loc.Y = -1;
            KeepPlayerOnMaze();

            Assert.AreEqual(Vector2.Zero, loc);

            // position player after end of map
            loc = map.dimensions;
            KeepPlayerOnMaze();

            Assert.AreEqual(Vector2.One, loc);
        }

        [Test]
        public void Does_RenderRectangle_AnswerForZero()
        {
            loc = Vector2.Zero;
            var test = RenderRectangle();            
            Assert.AreEqual(test, new Rectangle(0,0,SPRITE_SIZE,SPRITE_SIZE));
        }

        [Test]
        public void Does_RenderRectangle_AnswerForOne()
        {
            loc = Vector2.One;
            
            Assert.AreEqual( new Rectangle( map.TileSize()
                                          , map.TileSize()
                                          , SPRITE_SIZE
                                          , SPRITE_SIZE
                                          )
                           , RenderRectangle()
                           );
        }

        [Test]
        public void DoesPlayerMoveCorrectlyBetweenColumns10_and_11()
        {
            loc = new Vector2(9, 1);

            distanceTravelled = new Vector2(1.01f, 0.0f);

            Assert.AreEqual( new Rectangle( (int)(map.TileSize() * loc.X) + 1
                                          , (int)(map.TileSize() * loc.Y)
                                          , SPRITE_SIZE
                                          , SPRITE_SIZE
                                          )
                           , RenderRectangle()
                           );

            distanceTravelled = new Vector2(SPRITE_SIZE + 1, 0.0f);

            Assert.AreEqual( new Rectangle( (int)(map.TileSize() * loc.X) + 1
                                          , (int)(map.TileSize() * loc.Y)
                                          , SPRITE_SIZE
                                          , SPRITE_SIZE
                                          )
                           , RenderRectangle()
                           );

        }
    }
}
