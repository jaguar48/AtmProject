using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BusinessLogic.Interfaces;
using DataBoard.Database;
using DataBoard.Enum;
using DataBoard.Model;
using DataBoard.ViewModel;
using Microsoft.Data.SqlClient;

namespace BusinessLogic.Implementation
{
    public class BankOperations : IBankOperation
    {
        private int _accountNumber;
        private decimal _balance;

        private readonly Atmservice _dbContext;
        private bool _disposed;

        public BankOperations(Atmservice dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<string> CreateCustomer(CustomerViewModel customer)
        {
            try
            {
                var sqlConn = await _dbContext.OpenConnection();

                if (!string.IsNullOrWhiteSpace(customer.Name) && customer.AccountNumber > 0 && customer.Pin > 0 && customer.Balance >= 0)
                {
                    if (customer.Pin.ToString().Length == 4)
                    {

                        SqlCommand cm = new SqlCommand($"INSERT INTO Customer(FirstName, AccountNumber, Pin, Balance) VALUES('{customer.Name}'," +
                                        $"{customer.AccountNumber},{customer.Pin},{customer.Balance} )", sqlConn);
                        await cm.ExecuteNonQueryAsync();


                        return "Customer created successfully";
                    }
                    else
                    {
                        Console.WriteLine("Pin must have exactly 4 digits");
                        return "error";
                    }
                }
                else
                {

                    return "One or more of the customer properties are null or have an invalid value.";
                }
            }
            catch (Exception ex)
            {

                return $"something went wrong {ex}";
            }
        }



        public async Task<CustomerViewModel> Withdraw(int accountNumber)
        {
            TransactionViewModel transaction = new TransactionViewModel();
            CustomerViewModel customer = new CustomerViewModel();

            Console.WriteLine("Enter amount you want to withdraw: ");
            string input = Console.ReadLine();
            decimal amount;
            if (!decimal.TryParse(input, out amount))
            {
                Console.WriteLine("Invalid amount entered");
                return customer;
            }

            var sqlConn = await _dbContext.OpenConnection();
            await using SqlCommand cmd = new SqlCommand($"SELECT * FROM Customer WHERE AccountNumber = @accountNumber", sqlConn);
            cmd.Parameters.AddWithValue("@accountNumber", accountNumber);

            await using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                customer.CustomerID = (int)reader["CustomerID"];
                customer.Name = reader["FirstName"].ToString();
                customer.AccountNumber = (int)reader["AccountNumber"];
                customer.Balance = (decimal)(reader["Balance"]);
                reader.Close();

                if (customer.Balance - amount >= 0)
                {
                    customer.Balance -= amount;
                    await using SqlCommand cm = new SqlCommand($"UPDATE Customer SET Balance = @balance WHERE AccountNumber = @accountNumber", sqlConn);

                    cm.Parameters.AddWithValue("@balance", customer.Balance);
                    cm.Parameters.AddWithValue("@accountNumber", accountNumber);

                    transaction.CustomerID = customer.CustomerID;
                    transaction.Name = customer.Name;
                    transaction.Amount = amount;
                    transaction.TransactionDate = DateTime.Now;
                    transaction.TransactionType = "Withdrawal";

                    SqlTransaction dbTransaction = sqlConn.BeginTransaction();
                    cm.Transaction = dbTransaction;

                    try
                    {
                        await cm.ExecuteNonQueryAsync();

                        await using SqlCommand command = new SqlCommand("INSERT INTO Transactions (CustomerID, CName, Amount, TransactionDate, TransactionType) VALUES (@customerID, @name, @amount, @transactionDate, @transactionType)", sqlConn);

                        command.Parameters.AddWithValue("@customerID", customer.CustomerID);
                        command.Parameters.AddWithValue("@name", transaction.Name);
                        command.Parameters.AddWithValue("@amount", transaction.Amount);
                        command.Parameters.AddWithValue("@transactionDate", transaction.TransactionDate);
                        command.Parameters.AddWithValue("@transactionType", transaction.TransactionType.ToString());
                        command.Transaction = dbTransaction;

                        await command.ExecuteNonQueryAsync();

                        dbTransaction.Commit();
                        Console.WriteLine("Withdrawal successful");
                        return customer;
                    }
                    catch (Exception ex)
                    {
                        dbTransaction.Rollback();
                        Console.WriteLine("An error occurred while trying to execute the SQL command", ex);
                        return customer;
                    }
                }
                else
                {
                    Console.WriteLine("Insufficient funds");
                    return customer;
                }
            }
            else
            {
                Console.WriteLine("Account not found");
                return customer;
            }
        }

        public async Task<decimal> GetBalance(int accountNumber)
        {

            var sqlConn = await _dbContext.OpenConnection();

            await using SqlCommand cm = new SqlCommand($"SELECT Balance FROM Customer WHERE AccountNumber = @accountNumber", sqlConn);
            cm.Parameters.AddWithValue("@accountNumber", accountNumber);

            try
            {
                var balance = (decimal)await cm.ExecuteScalarAsync();

                return balance;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while trying to retrieve the balance: " + ex.Message);
                return -1;
            }
        }



        public async Task Transfer(int sourceAccountNumber, int destinationAccountNumber)
        {
            CustomerViewModel customer = new CustomerViewModel();
            TransactionViewModel savetransaction = new TransactionViewModel();

            string receiverName = "";

            while (true)
            {
                Console.WriteLine("Enter amount you want to transfer: ");

                string input = Console.ReadLine();
                decimal amount;
                if (!decimal.TryParse(input, out amount))
                {
                    Console.WriteLine("Invalid amount entered");
                    break;
                }

                var sqlConn = await _dbContext.OpenConnection();
                await using SqlCommand cmd = new SqlCommand($"SELECT * FROM Customer WHERE AccountNumber = @accountNumber", sqlConn);
                cmd.Parameters.AddWithValue("@accountNumber", sourceAccountNumber);



                await using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                    {
                        Console.WriteLine("Source account not found");
                        break;
                    }
                    customer.Balance = (decimal)(reader["Balance"]);
                    customer.Name = (string)(reader["FirstName"]);

                    if (sourceAccountNumber == destinationAccountNumber)
                    {
                        Console.WriteLine("Cannot make transfer to self");
                        break;
                    }
                    else if (customer.Balance - amount < 0)
                    {
                        Console.WriteLine("Insufficient funds");
                        break;
                    }
                }

                await using SqlCommand cmd2 = new SqlCommand($"SELECT FirstName FROM Customer WHERE AccountNumber = @destinationAccountNumber", sqlConn);
                cmd2.Parameters.AddWithValue("@destinationAccountNumber", destinationAccountNumber);
                await using (SqlDataReader reader2 = await cmd2.ExecuteReaderAsync())
                {
                    if (!await reader2.ReadAsync())
                    {
                        Console.WriteLine("Destination account not found");
                        break;
                    }
                    receiverName = (string)(reader2["FirstName"]);
                }

                await using SqlCommand cm1 = new SqlCommand($"UPDATE Customer SET Balance = @balance WHERE AccountNumber = @accountNumber", sqlConn);
                cm1.Parameters.AddWithValue("@balance", customer.Balance - amount);
                cm1.Parameters.AddWithValue("@accountNumber", sourceAccountNumber);

                await using SqlCommand cm2 = new SqlCommand($"UPDATE Customer SET Balance = Balance + @amount WHERE AccountNumber = @destinationAccountNumber", sqlConn);
                cm2.Parameters.AddWithValue("@amount", amount);
                cm2.Parameters.AddWithValue("@destinationAccountNumber", destinationAccountNumber);


                savetransaction.Name = customer.Name;
                savetransaction.Amount = amount;
                savetransaction.TransactionDate = DateTime.Now;
                savetransaction.TransactionType = "Transfer";





                SqlTransaction transaction = sqlConn.BeginTransaction();

                try
                {
                    cm1.Transaction = transaction;
                    await cm1.ExecuteNonQueryAsync();




                    await using SqlCommand command = new SqlCommand("INSERT INTO Transactions (CName, Amount, TransactionDate, TransactionType)" +
                       " VALUES (@name, @amount, @transactionDate, @transactionType)", sqlConn);

                    /*command.Parameters.AddWithValue("@customerId", transaction.CustomerId);*/
                    command.Parameters.AddWithValue("@name", savetransaction.Name);
                    command.Parameters.AddWithValue("@amount", savetransaction.Amount);
                    command.Parameters.AddWithValue("@transactionDate", savetransaction.TransactionDate);
                    command.Parameters.AddWithValue("@transactionType", (savetransaction.TransactionType));
                    command.Transaction = transaction;
                    await command.ExecuteNonQueryAsync();

                    cm2.Transaction = transaction;
                    await cm2.ExecuteNonQueryAsync();

                    transaction.Commit();


                    Console.WriteLine("Transfer successful");
                    Console.WriteLine($"{customer.Name} has sent ${amount} to {receiverName}");

                    break;
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("An error occurred while trying to execute the SQL command", ex);
                    Console.WriteLine("Do you want to retry the transfer (yes/no)?");
                    string retry = Console.ReadLine();
                    if (retry != "yes")
                    {
                        break;
                    }
                }
            }

        }


        /*  private static void ViewTransactionHistory()
          {
              using (SqlCommand command = new SqlCommand("SELECT * FROM Transactions WHERE AccountNumber = @accountNumber", _connection))
              {
                  command.Parameters.AddWithValue("@accountNumber", _accountNumber);

                  using (SqlDataReader reader = command.ExecuteReader())
                  {
                      if (!reader.HasRows)
                      {
                          Console.WriteLine("No transactions found.");
                          return;
                      }

                      while (reader.Read())
                      {
                          Console.WriteLine("Transaction ID: " + reader["TransactionId"]);
                          Console.WriteLine("Type: " + reader["TransactionType"]);
                          Console.WriteLine("Amount: " + reader["Amount"]);
                          Console.WriteLine("Date: " + reader["Date"]);
                          Console.WriteLine();
                      }
                  }
              }
          }*/




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




