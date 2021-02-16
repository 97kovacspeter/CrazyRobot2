using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.Persistence
{
    #region enum

    public enum FieldType { Empty, Ruined, Wall, PermaWall, Magnet }

    #endregion

    /// <summary>
    /// A játéktábla típusa.
    /// </summary>
    public class Table
    {
        #region Fields

        public FieldType[ , ] FieldValues; //mezőértékek
        private int _size;

        #endregion

        #region Property

        public int Size { get { return _size; } }

        #endregion

        #region Constructor

        public Table( int size )
        {
            _size = size;
            FieldValues = new FieldType[ size, size ];
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    if (i == 0 || j == 0 || i == size - 1 || j == size - 1)
                    {
                        FieldValues[ i, j ] = FieldType.PermaWall;
                    }
                    else
                    {
                        FieldValues[ i, j ] = FieldType.Empty;
                    }
                }
            }
            FieldValues[ (size) / 2, (size) / 2 ] = FieldType.Magnet;

        }

        #endregion

    }
}
