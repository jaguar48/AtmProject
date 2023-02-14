using System;

namespace DataBoard.ViewModel
{
    public class CustomerViewModel
    {
        public int CustomerID { get; set; }
        public string? Name { get; set; }
        public decimal Balance { get; set; }
        public int Pin { get; set; }
        public int AccountNumber { get; set; }

        public CustomerViewModel(string name)
        {
            AccountNumber = GenerateAccountNumber();
            Name = name;
            Pin = GenerateATMPIN();
            Balance = GenerateBalance();

        }
        public CustomerViewModel()
        {


        }
        public static int GenerateAccountNumber()
        {
            var rand = new Random();
            int accountNumber = 0;

            accountNumber = 380000000 + rand.Next(1000000, 9999999);

            while (accountNumber < 380000000 || accountNumber >= 390000000)
            {
                accountNumber = 380000000 + rand.Next(1000000, 9999999);
            }

            return accountNumber;
        }

        public static decimal GenerateBalance()
        {
            var rand = new Random();
            decimal balance = rand.Next(100, 100000) / 100m;
            return balance;
        }

        public static int GenerateATMPIN()
        {
            var rand = new Random();
            int pin = rand.Next(1000, 9999);
            return pin;
        }







    }

}

