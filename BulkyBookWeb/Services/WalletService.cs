using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBookWeb.Helpers
{
    public  class WalletService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WalletService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void UpdateWalletBalance(string userId, double refundAmount)
        {
            // Your implementation here
        }
    }
}
