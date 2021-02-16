using System;
using System.Threading.Tasks;
namespace Robot.Persistence
{
    public struct ReturnData
    {
        public int GameTime;
        public Table Table;
        public int playerX;
        public int playerY;
        public int size;
    }

    public interface IDataAccess
    {
        /// <summary>
        /// Fájl betöltése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <returns>A fájlból beolvasott játéktábla.</returns>
        Task<ReturnData> LoadAsync( String path );

        /// <summary>
        /// Fájl mentése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <param name="table">A fájlba kiírandó játéktábla.</param>
        Task SaveAsync( String path, Table Table, int GameTime, int playerX, int playerY, int size );
    }
}