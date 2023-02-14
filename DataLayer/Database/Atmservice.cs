using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;


namespace DataBoard.Database
{

        public class Atmservice: IDisposable
        {
            private readonly string _connString;

            private bool _disposed;

            private SqlConnection _dbConnection = null;
            public Atmservice() : this(@"Data Source=.;Initial Catalog=AtmDBApp;Integrated Security=True;Encrypt=False;TrustServerCertificate=False;")
            {

            }

            public Atmservice(string connString)
            {
                _connString = connString;
            }

            public async Task<SqlConnection> OpenConnection()
            {
                _dbConnection = new SqlConnection(_connString);
                await _dbConnection.OpenAsync();
                return _dbConnection;
            }

            public async Task CloseConnection()
            {
                if (_dbConnection?.State != ConnectionState.Closed)
                {
                    await _dbConnection?.CloseAsync();
                }
            }


            protected virtual void Dispose(bool disposing)
            {
                if (_disposed)
                {
                    return;
                }

                if (disposing)
                {
                    _dbConnection.Dispose();
                }

                _disposed = true;
            }
            public void Dispose()
            {

                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        
    }
