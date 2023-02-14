using DataBoard;
using DataBoard.ViewModel;
using System.Threading.Tasks;
using System;

namespace BusinessLogic.Interfaces
{
    public interface IBankOperation : IDisposable
    {
        Task<string> CreateCustomer(CustomerViewModel customer);

        /* void Deposit(double amount);*/
        Task<CustomerViewModel> Withdraw(int accountNumber);
        Task<decimal> GetBalance(int Account);
        Task Transfer(int sourceAccountNumber, int destinationAccountNumber);

    }
}
