using DataBoard.ViewModel;
using System;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IBankOperation : IDisposable
    {
        Task<string> CreateCustomer(CustomerViewModel customer);


        Task<CustomerViewModel> Withdraw(int accountNumber);
        Task<decimal> GetBalance(int Account);
        Task Transfer(int sourceAccountNumber, int destinationAccountNumber);

    }
}
