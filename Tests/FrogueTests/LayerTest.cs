using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TileEngine;
using Microsoft.Xna.Framework;

namespace FrogueTests
{
    //--------------------------------------------------------------------------
    [TestFixture]
    public class TestLayer
    { 
        Layer test;

        [SetUp]
        public void Init()
        {   test = new Layer(new Vector2(2,2), 0);
        }

        protected void checkCell(int i, int j, int Value)
        {
            Assert.AreEqual(Value, test.data[i,j], String.Format("Cell[{0},{1}] contained invalid value: {2}",i,j,Value));            
        }

        protected void checkAllCellsAre(int Value)
        {
            for (int i=0; i<(int)test.dimensions.X; i++)
                for (int j=0; j<(int)test.dimensions.Y; j++)
                {   checkCell(i, j, Value);
                }
        }

        [Test]
        public void DoesLayerInitialiseWithDefault()
        {
            checkAllCellsAre(0);
        }

        [Test]
        public void CanSetAllCellsToValue()
        {
            test.SetAllCellsTo(-1);
            checkAllCellsAre(-1);
        }

        [Test]
        public void CanDrawSinglePixelRectangleOnLayer()
        {
            test = new Layer(new Vector2(3,3), 0);
            
            test.DrawRectangle(new Rectangle(1,1,1,1), 1);   
            
            checkCell(0,0,0);
            checkCell(1,0,0);
            checkCell(2,0,0);
            checkCell(0,1,0);
            checkCell(1,1,1);
            checkCell(2,1,0);
            checkCell(0,2,0);
            checkCell(1,2,0);
            checkCell(2,2,0);
        }

        [Test]
        public void CanDrawMultiPixelRectangleOnLayer_WholeLayer()
        {
            test = new Layer(new Vector2(3,3), 0);
            
            test.DrawRectangle(new Rectangle(0,0,3,3), 1);   
            
            checkCell(0,0,1);
            checkCell(1,0,1);
            checkCell(2,0,1);
            checkCell(0,1,1);
            checkCell(1,1,1);
            checkCell(2,1,1);
            checkCell(0,2,1);
            checkCell(1,2,1);
            checkCell(2,2,1);
        }
        
        [Test]
        public void CanDrawMultiPixelRectangleOnLayer_PartialLayer()
        {
            test = new Layer(new Vector2(3,3), 0);
            
            test.DrawRectangle(new Rectangle(0,0,2,2), 1);   
            
            checkCell(0,0,1);
            checkCell(1,0,1);
            checkCell(2,0,0);
            checkCell(0,1,1);
            checkCell(1,1,1);
            checkCell(2,1,0);
            checkCell(0,2,0);
            checkCell(1,2,0);
            checkCell(2,2,0);
        }

        [Test]
        public void CanDrawRectangleOffTheEdge_Negative()
        {
            test = new Layer(new Vector2(3,3), 0);
            
            test.DrawRectangle(new Rectangle(-1,-1,2,2), 1);   
            
            checkCell(0,0,1);
            checkCell(1,0,1);
            checkCell(2,0,0);
            checkCell(0,1,1);
            checkCell(1,1,1);
            checkCell(2,1,0);
            checkCell(0,2,0);
            checkCell(1,2,0);
            checkCell(2,2,0);
        }

        [Test]
        public void CanDrawRectangleOffTheEdge_Positive()
        {
            test = new Layer(new Vector2(3,3), 0);
            
            test.DrawRectangle(new Rectangle(1,1,4,4), 1);   
            
            checkCell(0,0,0);
            checkCell(1,0,0);
            checkCell(2,0,0);
            checkCell(0,1,0);
            checkCell(1,1,1);
            checkCell(2,1,1);
            checkCell(0,2,0);
            checkCell(1,2,1);
            checkCell(2,2,1);
        }

        [Test]
        public void IsRectangleEntirelyOffTheEdgeIgnored()
        {
            test = new Layer(new Vector2(3,3), 0);
            
            test.DrawRectangle(new Rectangle(3,3,4,4), 1);   
            
            checkCell(0,0,0);
            checkCell(1,0,0);
            checkCell(2,0,0);
            checkCell(0,1,0);
            checkCell(1,1,0);
            checkCell(2,1,0);
            checkCell(0,2,0);
            checkCell(1,2,0);
            checkCell(2,2,0);
        }
    
    }
}
