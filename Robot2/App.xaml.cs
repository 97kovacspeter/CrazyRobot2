using Robot;
using Robot.Model;
using Robot.Persistence;
using Robot2.ViewModel;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

using System.Windows.Input;
using Robot2.Model;

namespace Robot2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        private GameModel _model;
        private RobotViewModel _viewModel;
        private MainWindow _view;
        private DispatcherTimer _timer;

        #endregion

        #region Constructors

        /// <summary>
        /// Alkalmazás példányosítása.
        /// </summary>
        public App()
        {
            Startup += new StartupEventHandler( App_Startup );
        }

        #endregion

        #region Application event handlers

        private void App_Startup( object sender, StartupEventArgs e )
        {
            // modell létrehozása
            _model = new GameModel( new FileDataAccess( ) );
            _model.GameOver += new EventHandler<EventArgs>( Model_GameOver );
            _model.NewGame( 9 );

            // nézemodell létrehozása
            _viewModel = new RobotViewModel( _model );
            _viewModel.NewGameEasy += new EventHandler( ViewModel_NewGameEasy );
            _viewModel.NewGameMedium += new EventHandler( ViewModel_NewGameMedium );
            _viewModel.NewGameHard += new EventHandler( ViewModel_NewGameHard );

            _viewModel.MakeNewWall += new EventHandler<RobotEventArgs>( ViewModel_MakeNewWall );

            _viewModel.LoadGame += new EventHandler( ViewModel_LoadGame );
            _viewModel.SaveGame += new EventHandler( ViewModel_SaveGame );
            _viewModel.PauseGame += new EventHandler( ViewModel_PauseGame );
            _viewModel.ContinueGame += new EventHandler( ViewModel_ContinueGame );
            // nézet létrehozása
            _view = new MainWindow( );
            _view.DataContext = _viewModel;
            _view.Show( );

            // időzítő létrehozása
            _timer = new DispatcherTimer( );
            _timer.Interval = TimeSpan.FromSeconds( 1 );
            _timer.Tick += new EventHandler( Timer_Tick );
            _timer.Start( );
        }

        private void Timer_Tick( object sender, EventArgs e )
        {
            _model.AdvanceTime( );
        }

        #endregion

        #region ViewModel event handlers

        private void ViewModel_MakeNewWall( object sender, RobotEventArgs e )
        {
            int i = e.GetPosition / 10;
            int j = e.GetPosition % 10;

            _model.CreateWall( i, j );
        }

        /// <summary>
        /// Új játék indításának eseménykezelője.
        /// </summary>
        private void ViewModel_NewGameEasy( object sender, EventArgs e )
        {
            _model.NewGame( 9 );
            _timer.Start( );
        }
        private void ViewModel_NewGameMedium( object sender, EventArgs e )
        {
            _model.NewGame( 13 );
            _timer.Start( );
        }
        private void ViewModel_NewGameHard( object sender, EventArgs e )
        {
            _model.NewGame( 17 );
            _timer.Start( );
        }

        /// <summary>
        /// Játék betöltésének eseménykezelője.
        /// </summary>
        private async void ViewModel_LoadGame( object sender, System.EventArgs e )
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop( );

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog( ); // dialógusablak
                openFileDialog.Title = "Loading robot table";
                openFileDialog.Filter = "Robot table|*.rtl";
                if (openFileDialog.ShowDialog( ) == true)
                {
                    // játék betöltése
                    await _model.LoadGameAsync( openFileDialog.FileName );

                    _timer.Start( );
                }
            }
            catch (Exception)
            {
                MessageBox.Show( "Loading was not successful!", "Robot", MessageBoxButton.OK, MessageBoxImage.Error );
            }

            if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                _timer.Start( );
        }

        /// <summary>
        /// Játék mentésének eseménykezelője.
        /// </summary>
        private async void ViewModel_SaveGame( object sender, EventArgs e )
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop( );

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog( ); // dialógablak
                saveFileDialog.Title = "Loading robot table";
                saveFileDialog.Filter = "Robot table|*.rtl";
                if (saveFileDialog.ShowDialog( ) == true)
                {
                    try
                    {
                        // játéktábla mentése
                        await _model.SaveGameAsync( saveFileDialog.FileName );
                    }
                    catch (Exception)
                    {
                        MessageBox.Show( "Saving was not successful!" + Environment.NewLine + "Wrong path or permission error.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error );
                    }
                }
            }
            catch
            {
                MessageBox.Show( "Saving was not successful!", "Robot", MessageBoxButton.OK, MessageBoxImage.Error );
            }

            if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                _timer.Start( );
        }


        private void ViewModel_PauseGame( object sender, EventArgs e )
        {
            _timer.Stop( );
            _model.Pause( );
            MessageBox.Show( "Paused!", "Robot game" );

        }
        private void ViewModel_ContinueGame( object sender, EventArgs e )
        {
            _timer.Start( );
            _model.Continue( );

        }

        #endregion

        #region Model event handlers

        /// <summary>
        /// Játék végének eseménykezelője.
        /// </summary>
        private void Model_GameOver( object sender, EventArgs e )
        {
            _timer.Stop( );

            MessageBoxResult result = MessageBox.Show( "Game Over!\nYour time was: " + _viewModel.GameTime + "\nWant to start a new game?",
                            "Robot game",
                            MessageBoxButton.YesNo );
            if (result == MessageBoxResult.Yes)
            {
                _model.NewGame( _model.ModelSize );
                _model.Continue( );
            }
        }



        #endregion

    }
}
