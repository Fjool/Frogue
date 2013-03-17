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
    public class TestPlayer
    { 
        Player test;

        [SetUp] 
        public void Init()
        {           
            test = new Player(new Vector2(2,2));
            test.map = new Map(null, test, new Vector2(2,2));  
            test.loc = Vector2.Zero;
        }

        [Test]
        public void PlayerStartsAtZero()
        {   Assert.AreEqual(Vector2.Zero, test.loc);   
        }

        [Test]
        public void Does_KeepPlayerOnMaze_Work()
        {   
            // position player before the start of the map
            test.loc.X = -1;
            test.loc.Y = -1;
            test.KeepPlayerOnMaze();

            Assert.AreEqual(Vector2.Zero, test.loc);

            // position player after end of map
            test.loc = test.map.dimensions;
            test.KeepPlayerOnMaze();

            Assert.AreEqual(Vector2.One, test.loc);
        }
        /*
        [Test]
        public void Does_RenderRectangle_AnswerForZero()
        {
            test.loc = Vector2.Zero;            
            Assert.AreEqual( new Rectangle(0,0,test.TileSize(),test.TileSize())
                           , test.RenderRectangle()
                           );
        }

        [Test]
        public void Does_RenderRectangle_AnswerForOne()
        {
            test.loc = Vector2.One;
            
            Assert.AreEqual( new Rectangle( test.map.TileSize()
                                          , test.map.TileSize()
                                          , test.TileSize()
                                          , test.TileSize()
                                          )
                           , test.RenderRectangle()
                           );
        }

        [Test]
        public void DoesPlayerMoveCorrectlyBetweenColumns10_and_11()
        {
            test.loc = new Vector2(9, 1);

            test.distanceTravelled = new Vector2(1.01f, 0.0f);

            Assert.AreEqual( new Rectangle( (int)(test.map.TileSize() * test.loc.X) + 1
                                          , (int)(test.map.TileSize() * test.loc.Y)
                                          , test.TileSize()
                                          , test.TileSize()
                                          )
                           , test.RenderRectangle()
                           , "Distance moved < TileSize"
                           );

            test.distanceTravelled = new Vector2(test.TileSize() + 1, 0.0f);

            Assert.AreEqual( new Rectangle( (int)(test.map.TileSize() * test.loc.X) + 1
                                          , (int)(test.map.TileSize() * test.loc.Y)
                                          , test.TileSize()
                                          , test.TileSize()
                                          )
                           , test.RenderRectangle()
                           , "Distance moved > TileSize"
                           );

        }
 */
    }
}
