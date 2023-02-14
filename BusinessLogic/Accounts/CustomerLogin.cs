using BusinessLogic.Interfaces;
using DataBoard.Database;
using DataBoard.ViewModel;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System;

namespace BusinessLogic.Accounts
{
    public class CustomerLogin : ILoginAccountInterface, IDisposable
    {



        private readonly Atmservice _dbContext;
        private bool _disposed;

        public CustomerLogin(Atmservice dbContext)
        {
            _dbContext = dbContext;
        }
        public int Pin { get; set; }

        public static decimal _balance;
        public static string? _name;

        public static int _account;

        public bool IsAuthenticated { get; set; }


        public async Task<CustomerViewModel> Authenticate()
        {
            var sqlConn = await _dbContext.OpenConnection();
            CustomerViewModel customer = new CustomerViewModel();

            Console.WriteLine("Enter your Pin number: ");
            string inputPin = Console.ReadLine();
            if (!int.TryParse(inputPin, out int pin) || inputPin.Length != 4)
            {
                Console.WriteLine("Pin must be a 4-digit integer.");
                return customer;
            }

            await using SqlCommand command = new SqlCommand("SELECT * FROM Customer WHERE Pin = @Pin", sqlConn);
            command.Parameters.AddWithValue("@Pin", pin);

            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    reader.Read();

                    customer.Balance = (decimal)reader["Balance"];
                    _balance = customer.Balance;

                    customer.Name = (string)reader["FirstName"];
                    _name = customer.Name;

                    customer.AccountNumber = (int)reader["AccountNumber"];
                    _account = customer.AccountNumber;

                    Console.WriteLine("Login successful");
                    Console.Clear();
                    IsAuthenticated = true;

                    return customer;
                }
            }


            return customer;
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
