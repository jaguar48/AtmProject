using DataBoard.ViewModel;
using System.Threading.Tasks;
using System;

namespace BusinessLogic.Interfaces
{
    public interface ILoginAccountInterface : IDisposable
    {
        int Pin { get; set; }
        bool IsAuthenticated { get; set; }
        Task<CustomerViewModel> Authenticate();

    }

    public interface IRegisterAccountInterface
    {
        int CustomerId { get; set; }

    }
}
