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
    public class TestGame: Game1
    { 
        [SetUp]
        public void Init()
        {
            CreateEntities(new Vector2(40,40));

            ScreenWide = 640;
            ScreenHigh = 480;

            player.loc = Vector2.Zero;
        }
        
        [Test]
        public void DoesCameraDefaultToTopLeft()
        {
            positionCameraAbovePlayer();
            Assert.AreEqual(Vector2.Zero, camera);
        }

        [Test]
        public void DoesCameraScrollWithPlayer()
        {
            player.loc = new Vector2(12,12);

            positionCameraAbovePlayer();
            Assert.AreEqual( new Vector2(64,144)
                           , camera
                           );        
        }

        [Test]
        public void IsPlayerDistanceTravelledAddedToDrawingPosition()
        {   player.distanceTravelled = new Vector2(33, 0);
            player.loc = new Vector2(12,12);

            positionCameraAbovePlayer();
            Assert.AreEqual( new Vector2(97,144)
                           , camera
                           ); 
        }
        
        [Test]
        public void IsScreenScrolling()
        {   player.distanceTravelled = new Vector2(0, 0);
            player.loc = new Vector2(1,9);

            positionCameraAbovePlayer();
            Assert.AreEqual( new Vector2(0,48)
                           , camera
                           ); 
        }
    }
}
