using ATM.BLL.Implementation;
using BusinessLogic.Implementation;
using DataBoard.Database;
using DataBoard.ViewModel;

namespace MutliAtm
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            // TABLE CREATION
            using (var atmDbOperations = new AtmDbOperations(new Atmservice()))
            {
                await atmDbOperations.CreateTable();
            }

            // AUTOMATIC DB CREATION
            using (var atmDbOperations = new AtmDbOperations(new Atmservice()))
            {
                await atmDbOperations.CreateDB();
            }

            // MANUAL CUSTOMER CREATION
            using (var atmDbOperations = new AtmDbOperations(new Atmservice()))
            {
                int count = await atmDbOperations.GetCustomerCount();
                while (count < 4)
                {
                    using (var bankOperations = new BankOperations(new Atmservice()))
                    {
                        var newCustomerViewModel = new CustomerViewModel("bishop");

                        var createdCustomer = await bankOperations.CreateCustomer(newCustomerViewModel);

                        Console.WriteLine(createdCustomer);
                        Console.WriteLine($"balance: {newCustomerViewModel.Balance}, account: {newCustomerViewModel.AccountNumber}, Pin: {newCustomerViewModel.Pin}");
                    }

                    count = await atmDbOperations.GetCustomerCount();
                    if (count >= 4)
                    {
                        Console.WriteLine("Maximum number of customers reached.");
                        break;
                    }
                }
            }

            // CUSTOMER LOGIN
            using (var loginAccountInterface = new CustomerLogin(new Atmservice()))
            {
                var customer = await loginAccountInterface.Authenticate();

                if (!loginAccountInterface.IsAuthenticated)
                {
                    Console.WriteLine("Incorrect pin");
                    return;
                }

                Console.WriteLine($"Welcome {customer.Name}\nAccount number is {customer.AccountNumber}\nBalance is ${customer.Balance}");

                // BANK OPERATIONS
                using (var bankOperations = new BankOperations(new Atmservice()))
                {
                    while (true)
                    {
                        Console.WriteLine("1. Withdraw");
                        Console.WriteLine("2. Transfer");
                        
                        Console.WriteLine("3. Exit");

                        Console.Write("Enter your choice: ");
                        if (!int.TryParse(Console.ReadLine(), out int choice))
                        {
                            Console.WriteLine("Invalid input");
                            continue;
                        }

                        switch (choice)
                        {
                            case 1:
                                var withdrawalResult = await bankOperations.Withdraw(customer.AccountNumber);
                                if (withdrawalResult != null && withdrawalResult.Balance >= 0)
                                {
                                    var updatedBalance = await bankOperations.GetBalance(customer.AccountNumber);
                                    Console.WriteLine($"Hello {customer.Name}, your updated balance is ${updatedBalance}");
                                    Console.WriteLine("Press any key to return to the menu...");
                                    Console.ReadKey();
                                }
                                break;

                            case 2:
                                Console.WriteLine("Enter the destination account number: ");
                                var destinationAccountNumber = int.Parse(Console.ReadLine());

                                try
                                {
                                    await bankOperations.Transfer(customer.AccountNumber, destinationAccountNumber);

                                    var updatedBalance = await bankOperations.GetBalance(customer.AccountNumber);
                                    Console.WriteLine($"Hello {customer.Name}, your updated balance is ${updatedBalance}");
                                    Console.WriteLine("Press any key to return to the menu...");
                                    Console.ReadKey();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Transfer failed: " + ex.Message);
                                }
                                break;

                            

                            case 3:
                                return;

                            default:
                                Console.WriteLine("Invalid input");
                                break;
                        }
                    }

                }
            }
        }
    }
}