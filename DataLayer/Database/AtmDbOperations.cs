using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace DataBoard.Database
{
    public class AtmDbOperations : IDisposable
    {
        private readonly Atmservice _dbContext;
        private bool _disposed;

        public AtmDbOperations(Atmservice dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateDB()
        {
            try
            {
                var sqlConn = await _dbContext.OpenConnection();

                // Check if database exists
                var cmCheckDb = new SqlCommand("SELECT COUNT(*) FROM sys.databases WHERE name='MyAtmDB'", sqlConn);
                var dbExists = Convert.ToInt32(await cmCheckDb.ExecuteScalarAsync()) > 0;

                if (!dbExists)
                {
                    // Create database if it doesn't exist
                    var cmCreateDb = new SqlCommand("CREATE DATABASE MyAtmDB", sqlConn);
                    await cmCreateDb.ExecuteNonQueryAsync();
                    Console.WriteLine("Database created successfully");
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine("OOPs, something went wrong." + ex);
            }
        }

        public async Task CreateTable()
        {
            try
            {
                var sqlConn = await _dbContext.OpenConnection();

                // Check if tables exist
                var cmCheckTables = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME IN ('Customer', 'Withdrawals', 'Transfers', 'Transactions')", sqlConn);
                var tablesExist = Convert.ToInt32(await cmCheckTables.ExecuteScalarAsync()) == 4;

                if (!tablesExist)
                {
                    // Create tables if they don't exist
                    var cmCreateTables = new SqlCommand("create table Customer(CustomerID INT PRIMARY KEY IDENTITY(1,1),FirstName VARCHAR(50) NOT NULL, " +
                        "AccountNumber INT NOT NULL UNIQUE,Pin INT NOT NULL UNIQUE, Balance DECIMAL NOT NULL);" +
                        "create table Withdrawals(WithdrawalID INT PRIMARY KEY IDENTITY(1, 1),CustomerID INT NOT NULL,Amount DECIMAL(18, 2) NOT NULL," +
                        "WithdrawalDate DATETIME NOT NULL,FOREIGN KEY(CustomerID) REFERENCES Customer(CustomerID));" +
                        "CREATE TABLE Transfers(TransferID INT PRIMARY KEY IDENTITY(1, 1),CustomerID INT NOT NULL," +
                        "Amount DECIMAL(18, 2) NOT NULL,TransferDate DATETIME NOT NULL,FOREIGN KEY(CustomerID) REFERENCES Customer(CustomerID));" +
                        "CREATE TABLE Transactions(ID INT PRIMARY KEY IDENTITY(1, 1),CustomerID INT NOT NULL,CName VARCHAR(50) NOT NULL, " +
                        "Amount DECIMAL(18, 2) NOT NULL,TransactionDate DATETIME NOT NULL,TransactionType VARCHAR(50) NOT NULL, FOREIGN KEY(CustomerID) REFERENCES Customer(CustomerID));",
                        sqlConn);
                    await cmCreateTables.ExecuteNonQueryAsync();
                    Console.WriteLine("Tables created successfully");
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine("OOPs, something went wrong." + ex);
            }
        }
        public async Task<int> GetCustomerCount()
        {
            var sqlConn = await _dbContext.OpenConnection();
            var cmd = new SqlCommand("SELECT COUNT(*) FROM Customer", sqlConn);
            var count = (int)await cmd.ExecuteScalarAsync();
            return count;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _dbContext.Dispose();
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
