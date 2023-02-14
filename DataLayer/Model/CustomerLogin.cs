/*using BusinessLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic;
using DataBoard.Database;
using System.Data.Common;
using Microsoft.Data.SqlClient;



namespace DataBoard.Model
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

        public decimal _balance;

        public bool IsAuthenticated { get; set; }
     

        public async Task<bool> Authenticate(int pin)
        {
            var sqlConn = await _dbContext.OpenConnection();

            Console.WriteLine("Enter your Pin number: ");
            Pin = int.Parse(Console.ReadLine());

            using (SqlCommand command = new SqlCommand("SELECT * FROM Customer WHERE Pin = @Pin", sqlConn))
            {
                command.Parameters.AddWithValue("@Pin", Pin);
               

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();

                        _balance = (decimal)reader["Balance"];

                        return true;
                        Console.WriteLine("Login successful");
                    }
                   
                }
            }
            return false;

           
   

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
*/