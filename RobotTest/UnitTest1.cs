using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Persistence;
using Robot.Model;

namespace RobotTest
{
    [TestClass]
    public class RobotTest
    {

        private GameModel _model;

        [TestInitialize]
        public void Initialize()
        {
            _model = new GameModel( null );
        }
        [TestMethod]
        public void ModelNewGameTest()
        {
            Table table = new Table( 9 );

            _model.NewGame( 9 );

            table = _model.Table;

            Assert.AreEqual( _model.GameTime, 0 );
            Assert.IsFalse( _model.IsGameOver );

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (i == 0 || j == 0 || i == 8 || j == 8)
                    {
                        Assert.AreEqual( table.FieldValues[ i, j ], FieldType.PermaWall );
                    }
                    else if (i == 4 && j == 4)
                    {
                        Assert.AreEqual( table.FieldValues[ i, j ], FieldType.Magnet );
                    }
                }
            }
        }

        [TestMethod]
        public void ModelGamOverTest()
        {
            Table table = new Table( 9 );

            _model.NewGame( 9 );

            table = _model.Table;

            _model.PlayerXValue = 3;
            _model.PlayerYValue = 4;

            _model.Step( Dir.Down );

            Assert.AreEqual( _model.IsGameOver, true );

        }

        [TestMethod]
        public void ModelTimeAdvanceTest()
        {
            Table table = new Table( 15 );

            _model.NewGame( 15 );

            table = _model.Table;

            _model.AdvanceTime( );//1
            Assert.AreEqual( _model.GameTime, 1 );


            _model.AdvanceTime( );//2
            Assert.AreEqual( _model.GameTime, 2 );


            _model.AdvanceTime( );//3
            Assert.AreEqual( _model.GameTime, 3 );

        }

        [TestMethod]
        public void RuiningWallTest()
        {
            Table table = new Table( 15 );

            _model.NewGame( 15 );

            table = _model.Table;

            table.FieldValues[ 10, 10 ] = FieldType.Wall;

            Assert.AreEqual( table.FieldValues[ 10, 10 ], FieldType.Wall );

            _model.PlayerXValue = 9;
            _model.PlayerYValue = 10;
            _model.CheckStep( Dir.Down );

            Assert.AreEqual( table.FieldValues[ 10, 10 ], FieldType.Ruined );

        }

    }
}
