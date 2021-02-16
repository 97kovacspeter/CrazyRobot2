using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Robot.Model;

namespace Robot2.ViewModel
{
    class Field : ViewModelBase
    {
        private Robot.Persistence.FieldType type;


        public bool IsMagnet { get { return type == Robot.Persistence.FieldType.Magnet; } }

        public bool IsPerma { get { return type == Robot.Persistence.FieldType.PermaWall; } }

        public bool IsWall { get { return type == Robot.Persistence.FieldType.Wall; } }

        public bool IsRuined { get { return type == Robot.Persistence.FieldType.Ruined; } }

        public bool IsEmpty { get { return type == Robot.Persistence.FieldType.Empty; } }

        public bool IsPlayer { get { return (X == Model.PlayerXValue && Y == Model.PlayerYValue); } }

        public Robot.Persistence.FieldType Type
        {
            get { return type; }
            set
            {
                type = value;

                OnPropertyChanged( "IsPlayer" );
                OnPropertyChanged( "IsMagnet" );
                OnPropertyChanged( "IsPerma" );
                OnPropertyChanged( "IsWall" );
                OnPropertyChanged( "IsRuined" );
                OnPropertyChanged( "IsEmpty" );
                OnPropertyChanged( "Type" );
            }
        }


        public GameModel Model { get; set; }

        /// <summary>
        /// Vízszintes koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 X { get; set; }

        /// <summary>
        /// Függőleges koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 Y { get; set; }

        /// <summary>
        /// Sorszám lekérdezése.
        /// </summary>
        public Int32 Number { get; set; }

        /// <summary>
        /// Lépés parancs lekérdezése, vagy beállítása.
        /// </summary>
        public DelegateCommand StepCommand { get; set; }
    }
}
