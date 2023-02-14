using BusinessLogic.Accounts;
using BusinessLogic.Implementation;
using BusinessLogic.Interfaces;
using DataBoard.Database;
using DataBoard.ViewModel;

namespace MutliAtm
{
    public class Program
    {
        static async Task Main(string[] args)
        {

            /* using (Atmdb myatm = new Atmdb(new Atmservice()))
             {
                 await myatm.CreateTable();



             }*/

            /*   using (IBankOperation bankOperation = new BankOperations(new Atmservice()))
               {


                   var customercreate = new CustomerViewModel("okonkwo");


                   var createdcustomer = await bankOperation.CreateCustomer(customercreate);


                   Console.WriteLine(createdcustomer);


                   Console.WriteLine("balance: " + customercreate.Balance + "account:" + customercreate.AccountNumber + "Pin:" + customercreate.Pin );



               };*/




            using ILoginAccountInterface loginAccountInterface = new CustomerLogin(new Atmservice());

            var customer = await loginAccountInterface.Authenticate();
            if (!loginAccountInterface.IsAuthenticated)
            {
                Console.WriteLine("Incorrect pin");
                return;
            }

            Console.WriteLine($"Welcome {customer.Name} \nAccount number is {customer.AccountNumber} \nBalance is  ${customer.Balance}");

            using (IBankOperation bankOperation = new BankOperations(new Atmservice()))
            {
                while (true)
                {
                    Console.WriteLine("1. Withdraw");
                    Console.WriteLine("2. Transfer");
                    Console.WriteLine("3. Exit");

                    Console.WriteLine("Enter your choice: ");
                    var choice = int.Parse(Console.ReadLine());

                    switch (choice)
                    {
                        case 1:


                            var withdrawalResult = await bankOperation.Withdraw(customer.AccountNumber);
                            if (withdrawalResult != null && withdrawalResult.Balance >= 0)
                            {
                                var updatedBalance = await bankOperation.GetBalance(customer.AccountNumber);
                                Console.WriteLine($"Hello {customer.Name}, your updated balance is ${updatedBalance}");
                            }
                            else
                            {
                                Console.WriteLine("Withdrawal failed");
                            }
                            break;

                        case 2:
                            Console.WriteLine("Enter the destination account number: ");
                            var destinationAccountNumber = int.Parse(Console.ReadLine());



                            try
                            {
                                await bankOperation.Transfer(customer.AccountNumber, destinationAccountNumber);


                                var updatedBalance = await bankOperation.GetBalance(customer.AccountNumber);
                                Console.WriteLine($"Hello {customer.Name}, your updated balance is ${updatedBalance}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Transfer failed: " + ex.Message);
                            }
                            break;

                        case 3:
                            return;
                    }
                }
            };
        }

    }
}
