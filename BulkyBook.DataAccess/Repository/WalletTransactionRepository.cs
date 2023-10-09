using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class WalletTransactionRepository : Repository<WalletTransaction>, IWalletTransactionRepository
    {
        private ApplicationDbContext _db;
        public WalletTransactionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}