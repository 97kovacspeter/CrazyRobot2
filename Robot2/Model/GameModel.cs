using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Robot.Persistence;

namespace Robot.Model
{
    #region enums

    /// <summary>
    /// Irányok felsorolása.
    /// </summary>
    public enum Dir { Left, Right, Up, Down }

    #endregion

    /// <summary>
    /// Robot játék típusa.
    /// </summary>
    public class GameModel
    {
        #region Variables

        private Dir _direction;
        private Table _table;
        private IDataAccess _dataAccess;
        private int _gameTime;
        private bool _win = false;
        private bool _pause = false;
        private bool _crashed = false;
        private readonly Random _rnd = new Random( );
        private int _playerX;
        private int _playerY;
        private int _size;

        #endregion

        #region Constructor

        /// <summary>
        /// Játék példányosítása.
        /// </summary>
        public GameModel( IDataAccess dataAccess )
        {
            _dataAccess = dataAccess;
            _table = new Table( 9 );
            _win = false;
            NewGame( 9 );
        }


        #endregion

        #region Events

        /// <summary>
        /// Játék előrehaladásának eseménye.
        /// </summary>
        public event EventHandler<EventArgs> GameAdvanced;

        /// <summary>
        /// Játék végének eseménye.
        /// </summary>
        public event EventHandler<EventArgs> GameOver;

        #endregion

        #region Properties

        /// <summary>
        /// Eltelt játékidő lekérdezése.
        /// </summary>
        public int GameTime { get { return _gameTime; } }


        /// <summary>
        /// Játéktábla lekérdezése.
        /// </summary>
        public Table Table { get { return _table; } }

        /// <summary>
        /// Játék végének lekérdezése.
        /// </summary>
        public bool IsGameOver { get { return _win; } }

        /// <summary>
        /// Játék szüneteltetésének lekérdezése.
        /// </summary>
        public bool IsGamePaused { get { return _pause; } }


        /// <summary>
        /// Játékos X értékének lekérdezése.
        /// </summary>
        public int PlayerXValue { get { return _playerX; } set { _playerX = value; } }


        /// <summary>
        /// Játékos Y értékének lekérdezése.
        /// </summary>
        public int PlayerYValue { get { return _playerY; } set { _playerY = value; } }


        /// <summary>
        /// Model méretének lekérdezése.
        /// </summary>
        public int ModelSize { get { return _size; } }

        #endregion

        #region Public game methods

        #region AdvanceRobot

        public void AdvanceRobot( Dir direction )
        {
            if (!_crashed)
            {
                bool craziness = Generator( );
                if (craziness)
                {
                    direction = ChangeDirection( );
                }

                CheckStep( direction );
            }
            _crashed = false;
        }

        #endregion

        #region CreateWall

        public void CreateWall( int i, int j )
        {
            if (_table.FieldValues[ i, j ] == FieldType.Empty)
            {
                _table.FieldValues[ i, j ] = FieldType.Wall;
            }
        }

        #endregion

        #region NewGame

        /// <summary>
        /// Új játék kezdése.
        /// </summary>
        public void NewGame( int size )
        {
            _win = false;
            _table = new Table( size );
            _gameTime = 0;
            _direction = ChangeDirection( );
            _size = size;
            GenerateStart( size );
        }

        #endregion

        #region AdvanceTime

        /// <summary>
        /// Játékidő léptetése.
        /// </summary>
        public void AdvanceTime()
        {
            if (IsGameOver)
                return;
            if (IsGamePaused)
                return;
            if (_gameTime % 1 == 0) //changing difficulty
            {
                AdvanceRobot( _direction );
            }

            _gameTime++;
            //System.Diagnostics.Debug.WriteLine( "Helyzet: " + PlayerXValue + " " + PlayerYValue );

            if (GameAdvanced != null)
            {
                GameAdvanced( this, null );
            }
        }

        #endregion

        #region Checkdirection

        public FieldType CheckDirection( Dir direction )
        {
            switch (direction)
            {
                case Dir.Left:
                    return _table.FieldValues[ _playerX, _playerY - 1 ];
                case Dir.Right:
                    return _table.FieldValues[ _playerX, _playerY + 1 ];
                case Dir.Down:
                    return _table.FieldValues[ _playerX + 1, _playerY ];
                case Dir.Up:
                    return _table.FieldValues[ _playerX - 1, _playerY ];
                default:
                    return FieldType.PermaWall;
            }
        }
        #endregion

        #region Step

        public void Step( Dir direction )
        {
            switch (direction)
            {
                case Dir.Down:
                    ++_playerX;
                    break;
                case Dir.Left:
                    --_playerY;
                    break;
                case Dir.Right:
                    ++_playerY;
                    break;
                case Dir.Up:
                    --_playerX;
                    break;
            }

            if (_table.FieldValues[ _playerX, _playerY ] == FieldType.Magnet)
            {
                _win = true;
                if (GameOver != null)
                {
                    GameOver( this, null );
                }
            }
        }

        #endregion

        #region CheckStep

        /// <summary>
        /// Játékos léptetése
        /// </summary>
        /// <param name="direction"></param>
        public void CheckStep( Dir direction )
        {

            if (IsGameOver)
                return;

            if (IsGamePaused)
                return;

            FieldType way = CheckDirection( direction );

            bool isFree = false;

            switch (way)
            {
                case FieldType.Empty:
                    isFree = true;
                    break;
                case FieldType.Ruined:
                    isFree = true;
                    break;
                case FieldType.Wall:
                    isFree = false;
                    break;
                case FieldType.PermaWall:
                    isFree = false;
                    break;
                case FieldType.Magnet:
                    isFree = true;
                    break;
            }

            if (isFree)
            {
                Step( direction );
            }
            else if (way == FieldType.Wall)
            {
                RuinWall( direction );
                direction = ChangeDirection( );
                _direction = direction;
                //CheckStep(direction);
            }
            else
            {
                direction = ChangeDirection( );
                _direction = direction;
                //CheckStep(direction);
            }

            if (GameAdvanced != null)
            {
                GameAdvanced( this, null );
            }
        }

        #endregion

        #region RuinWall

        public void RuinWall( Dir direction )
        {
            switch (direction)
            {
                case Dir.Left:
                    _table.FieldValues[ _playerX, _playerY - 1 ] = FieldType.Ruined;
                    break;
                case Dir.Right:
                    _table.FieldValues[ _playerX, _playerY + 1 ] = FieldType.Ruined;
                    break;
                case Dir.Down:
                    _table.FieldValues[ _playerX + 1, _playerY ] = FieldType.Ruined;
                    break;
                case Dir.Up:
                    _table.FieldValues[ _playerX - 1, _playerY ] = FieldType.Ruined;
                    break;
            }

            _crashed = true;
        }

        #endregion

        #region Pause

        public void Pause()
        {
            _pause = true;
        }

        #endregion

        #region Continue

        public void Continue()
        {
            _pause = false;
        }

        #endregion

        #endregion

        #region Async

        #region Load

        /// <summary>
        /// Játék betöltése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        public async Task LoadGameAsync( String path )
        {
            if (_dataAccess == null)
                throw new InvalidOperationException( "No data access is provided." );

            ReturnData returndata = await _dataAccess.LoadAsync( path );

            _gameTime = returndata.GameTime;
            _playerX = returndata.playerX;
            _playerY = returndata.playerY;
            _size = returndata.size;
            _table = returndata.Table;
        }

        #endregion

        #region Save

        /// <summary>
        /// Játék mentése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        public async Task SaveGameAsync( String path )
        {
            if (_dataAccess == null)
                throw new InvalidOperationException( "No data access is provided." );

            await _dataAccess.SaveAsync( path, _table, _gameTime, _playerX, _playerY, _table.Size );
        }

        #endregion

        #endregion

        #region Private game methods

        #region Generating start

        private void GenerateStart( int size )
        {
            int rand0 = _rnd.Next( 1, size / 2 - 1 );
            int rand1 = _rnd.Next( 1, size / 2 - 1 );

            _playerX = rand0;
            _playerY = rand1;

        }

        #endregion

        #region Generating craziness

        private bool Generator()
        {
            bool crazy = false;
            int type = _rnd.Next( 1, 10 );

            switch (type)
            {
                case 1:
                    crazy = true;
                    break;
                default:
                    crazy = false;
                    break;
            }
            return crazy;
        }

        #endregion

        #region Changing direction

        private Dir ChangeDirection()
        {
            Dir direction = Dir.Right;
            int type = _rnd.Next( 0, 256 );

            switch (type % 4)
            {
                case 0:
                    direction = Dir.Down;
                    break;
                case 1:
                    direction = Dir.Left;
                    break;
                case 2:
                    direction = Dir.Right;
                    break;
                case 3:
                    direction = Dir.Up;
                    break;

            }
            //System.Diagnostics.Debug.WriteLine( "Direction: "+direction+" "+type +" " );
            return direction;
        }

        #endregion

        #endregion
    }

}
