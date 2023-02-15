using DataBoard.Enum;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System;

namespace DataBoard.Database
{
    public class Atmdb : IDisposable
    {

        private readonly Atmservice _dbContext;
        private bool _disposed;

        public Atmdb(Atmservice dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateDB()
        {
            try
            {
                var sqlConn = await _dbContext.OpenConnection();
                SqlCommand cm = new SqlCommand("CREATE DATABASE MyAtmDB", sqlConn);
                
                await cm.ExecuteNonQueryAsync();

                Console.WriteLine("transaction table created Successfully");

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

                SqlCommand cm = new SqlCommand("create table Customer(CustomerID INT PRIMARY KEY IDENTITY(1,1),FirstName VARCHAR(50) NOT NULL, " +
                    "AccountNumber INT NOT NULL UNIQUE,Pin INT NOT NULL UNIQUE, Balance DECIMAL NOT NULL); ", sqlConn);



                SqlCommand cm1 = new SqlCommand("create table Withdrawals(WithdrawalID INT PRIMARY KEY IDENTITY(1, 1),CustomerID INT NOT NULL,Amount DECIMAL(18, 2) NOT NULL," +
                    "WithdrawalDate DATETIME NOT NULL,FOREIGN KEY(CustomerID) REFERENCES Customer(CustomerID)); ", sqlConn);



                SqlCommand cm2 = new SqlCommand("CREATE TABLE Transfers(TransferID INT PRIMARY KEY IDENTITY(1, 1),CustomerID INT NOT NULL," +
                    "Amount DECIMAL(18, 2) NOT NULL,TransferDate DATETIME NOT NULL,FOREIGN KEY(CustomerID) REFERENCES Customer(CustomerID)); ", sqlConn);



                SqlCommand cm3 = new SqlCommand("CREATE TABLE Transactions(ID INT PRIMARY KEY IDENTITY(1, 1),CustomerID INT NOT NULL,CName VARCHAR(50) NOT NULL, " +
                    "Amount DECIMAL(18, 2) NOT NULL,TransactionDate DATETIME NOT NULL,TransactionType VARCHAR(50) NOT NULL, FOREIGN KEY(CustomerID) REFERENCES Customer(CustomerID)); ", sqlConn);

                await cm.ExecuteNonQueryAsync();
                await cm1.ExecuteNonQueryAsync();
                await cm2.ExecuteNonQueryAsync();
                await cm3.ExecuteNonQueryAsync();

                Console.WriteLine("transaction table created Successfully");

                /*var secondcon = await _dbContext.OpenConnection();*/

            }
            catch (Exception ex)
            {
                Console.WriteLine("OOPs, something went wrong." + ex);
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
