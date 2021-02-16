using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Robot.Model;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using System.Runtime.Remoting.Channels;
using System.Windows.Navigation;
using Robot.Persistence;
using Robot2.Model;

namespace Robot2.ViewModel
{
    class RobotViewModel : ViewModelBase
    {
        #region Fields

        public GameModel _model; // modell

        #endregion

        #region Properties

        /// <summary>
        /// Új játék kezdése parancs lekérdezése.
        /// </summary>
        public DelegateCommand NewGameEasyCommand { get; private set; }
        public DelegateCommand NewGameMediumCommand { get; private set; }
        public DelegateCommand NewGameHardCommand { get; private set; }

        /// <summary>
        /// Játék betöltése parancs lekérdezése.
        /// </summary>
        public DelegateCommand LoadGameCommand { get; private set; }


        /// <summary>
        /// Játék mentése parancs lekérdezése.
        /// </summary>
        public DelegateCommand SaveGameCommand { get; private set; }


        public DelegateCommand PauseCommand { get; private set; }

        public DelegateCommand ContinueCommand { get; private set; }

        public ObservableCollection<Field> Fields { get; set; }


        public DelegateCommand MakeWallCommand { get; private set; }

        public Int32 GetViewSize { get { return _model.ModelSize; } }
        /// <summary>
        /// Fennmaradt játékidő lekérdezése.
        /// </summary>
        public String GameTime { get { return TimeSpan.FromSeconds( _model.GameTime ).ToString( "g" ); } }

        #endregion

        #region Events

        /// <summary>
        /// Új játék eseménye.
        /// </summary>
        public event EventHandler NewGameEasy;
        public event EventHandler NewGameMedium;
        public event EventHandler NewGameHard;

        /// <summary>
        /// Játék betöltésének eseménye.
        /// </summary>
        public event EventHandler LoadGame;

        /// <summary>
        /// Játék mentésének eseménye.
        /// </summary>
        public event EventHandler SaveGame;


        public event EventHandler PauseGame;

        public event EventHandler ContinueGame;

        public event EventHandler<RobotEventArgs> MakeNewWall;

        #endregion

        #region Constructors

        /// <summary>
        ///A nézetmodell példányosítása.
        /// </summary>
        /// <param name="model">A modell típusa.</param>
        public RobotViewModel( GameModel model )
        {
            // játék csatlakoztatása
            _model = model;
            _model.GameAdvanced += new EventHandler<EventArgs>( Model_GameAdvanced );
            _model.GameOver += new EventHandler<EventArgs>( Model_GameOver );


            // parancsok kezelése
            NewGameEasyCommand = new DelegateCommand( param => OnNewGameEasy( ) );
            NewGameMediumCommand = new DelegateCommand( param => OnNewGameMedium( ) );
            NewGameHardCommand = new DelegateCommand( param => OnNewGameHard( ) );

            MakeWallCommand = new DelegateCommand( param => OnNewWall( param ) );

            LoadGameCommand = new DelegateCommand( param => OnLoadGame( ) );
            SaveGameCommand = new DelegateCommand( param => OnSaveGame( ) );
            PauseCommand = new DelegateCommand( param => OnPause( ) );
            ContinueCommand = new DelegateCommand( param => OnContinue( ) );

            // játéktábla létrehozása
            Fields = new ObservableCollection<Field>( );
            for (Int32 i = 0; i < _model.ModelSize; i++) // inicializáljuk a mezőket
            {
                for (Int32 j = 0; j < _model.ModelSize; j++)
                {
                    Fields.Add( new Field
                    {
                        Model = _model,
                        Type = Robot.Persistence.FieldType.Empty,
                        X = i,
                        Y = j,
                        Number = i * 10 + j

                    } );

                }
            }
            OnPropertyChanged( "GetViewSize" );

            RefreshTable( );


        }

        #endregion

        private void RefreshTable()
        {
            foreach (Field field in Fields) // inicializálni kell a mezőket is
            {
                field.Type = _model.Table.FieldValues[ field.X, field.Y ];
                if (field.IsWall)
                {
                    System.Diagnostics.Debug.WriteLine( "Helyzet: " + field.X + " " + field.Y );
                }
            }

            OnPropertyChanged( "Fields" );

            OnPropertyChanged( "GetViewSize" );
        }

        private void RebuildTable()
        {
            Fields = new ObservableCollection<Field>( );
            for (Int32 i = 0; i < _model.ModelSize; i++)
            {
                for (Int32 j = 0; j < _model.ModelSize; j++)
                {
                    Fields.Add( new Field
                    {
                        Model = _model,
                        Type = Robot.Persistence.FieldType.Empty,
                        X = i,
                        Y = j,
                        Number = i * 10 + j
                    } );

                }
            }
            RefreshTable( );
            OnPropertyChanged( "GetViewSize" );
        }

        #region Game event handlers

        /// <summary>
        /// Játék végének eseménykezelője.
        /// </summary>
        private void Model_GameOver( object sender, EventArgs e )
        {

        }

        /// <summary>
        /// Játék előrehaladásának eseménykezelője.
        /// </summary>
        private void Model_GameAdvanced( object sender, EventArgs e )
        {
            RefreshTable( );
            OnPropertyChanged( "GameTime" );
        }

        #endregion

        #region Event methods


        private void OnNewWall( object param )
        {

            Field field = param as Field;

            if (MakeNewWall != null)
                MakeNewWall( this, new RobotEventArgs( field.Number ) );


            OnPropertyChanged( "Fields" );
            RefreshTable( );
        }

        /// <summary>
        /// Új játék indításának eseménykiváltása.
        /// </summary>
        private void OnNewGameEasy()
        {
            if (NewGameEasy != null)
                NewGameEasy( this, EventArgs.Empty );

            RebuildTable( );
            OnPropertyChanged( "GetViewSize" );
        }
        private void OnNewGameMedium()
        {
            if (NewGameMedium != null)
                NewGameMedium( this, EventArgs.Empty );

            RebuildTable( );
            OnPropertyChanged( "GetViewSize" );
        }
        private void OnNewGameHard()
        {
            if (NewGameHard != null)
                NewGameHard( this, EventArgs.Empty );

            RebuildTable( );
            OnPropertyChanged( "GetViewSize" );
        }



        /// <summary>
        /// Játék betöltése eseménykiváltása.
        /// </summary>
        private void OnLoadGame()
        {
            if (LoadGame != null)
                LoadGame( this, EventArgs.Empty );

            RebuildTable( );
            OnPropertyChanged( "GetViewSize" );
        }

        /// <summary>
        /// Játék mentése eseménykiváltása.
        /// </summary>
        private void OnSaveGame()
        {
            if (SaveGame != null)
                SaveGame( this, EventArgs.Empty );
        }

        /// <summary>
        /// Játék mentése eseménykiváltása.
        /// </summary>
        private void OnPause()
        {
            if (PauseGame != null)
                PauseGame( this, EventArgs.Empty );
        }

        private void OnContinue()
        {
            if (ContinueGame != null)
                ContinueGame( this, EventArgs.Empty );
        }

        #endregion
    }
}
