using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Robot.Persistence;

namespace Robot
{
    public class FileDataAccess : IDataAccess
    {
        async Task<ReturnData> IDataAccess.LoadAsync( string path )
        {
            try
            {
                using (StreamReader reader = new StreamReader( path ))
                {
                    ReturnData returndata = new ReturnData
                    {
                        GameTime = int.Parse( reader.ReadLine( ) ),
                        playerX = int.Parse( reader.ReadLine( ) ),
                        playerY = int.Parse( reader.ReadLine( ) ),
                        size = int.Parse( reader.ReadLine( ) )
                    };

                    returndata.Table = new Table( returndata.size );

                    String line;
                    string[ ] numbers = new string[ returndata.size ];
                    int temp;

                    for (int i = 0; i < returndata.size; i++)
                    {
                        line = await reader.ReadLineAsync( );
                        numbers = line.Split( ' ' );

                        for (int j = 0; j < returndata.size; j++)
                        {
                            temp = int.Parse( numbers[ j ] );
                            if (temp == 0)
                            {
                                returndata.Table.FieldValues[ i, j ] = FieldType.Empty;
                            }
                            else if (temp == 1)
                            {
                                returndata.Table.FieldValues[ i, j ] = FieldType.Magnet;
                            }
                            else if (temp == 2)
                            {
                                returndata.Table.FieldValues[ i, j ] = FieldType.Ruined;
                            }
                            else if (temp == 3)
                            {
                                returndata.Table.FieldValues[ i, j ] = FieldType.Wall;
                            }
                            else if (temp == 4)
                            {
                                returndata.Table.FieldValues[ i, j ] = FieldType.PermaWall;
                            }

                        }
                    }

                    return returndata;
                }
            }
            catch
            {
                throw new Exception( "Failedload" );
            }
        }

        async Task IDataAccess.SaveAsync( string path, Table table, int gameTime, int playerX, int playerY, int size )
        {

            try
            {
                using (StreamWriter writer = new StreamWriter( path )) // fájl megnyitása
                {

                    await writer.WriteLineAsync( gameTime.ToString( ) );

                    await writer.WriteLineAsync( playerX.ToString( ) );

                    await writer.WriteLineAsync( playerY.ToString( ) );

                    await writer.WriteLineAsync( size.ToString( ) );

                    int temp = 0;

                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            switch (table.FieldValues[ i, j ])
                            {
                                case FieldType.Empty:
                                    temp = 0;
                                    break;
                                case FieldType.Magnet:
                                    temp = 1;
                                    break;
                                case FieldType.Ruined:
                                    temp = 2;
                                    break;
                                case FieldType.Wall:
                                    temp = 3;
                                    break;
                                case FieldType.PermaWall:
                                    temp = 4;
                                    break;
                            }
                            await writer.WriteAsync( temp + " " );
                        }
                        await writer.WriteLineAsync( );
                    }
                }
            }
            catch
            {
                throw new Exception( "Save failed" );
            }
        }
    }
}

