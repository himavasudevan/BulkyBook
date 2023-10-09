using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class WalletRepository : Repository<Wallet>, IWalletRepository
    {
        private ApplicationDbContext _db;
        public WalletRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Wallet obj)
        {
            _db.Wallets.Update(obj);

        }


        public void AddTowallet(string email)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                var wallet = _db.Wallets.FirstOrDefault(W => W.ApplicationUserId == user.Id);
                if (wallet == null)
                {



                    var newWallet = new Wallet
                    {

                        Balance = 0,
                        ApplicationUserId = user.Id,

                    };
                    _db.Wallets.Add(newWallet);
                    _db.SaveChanges();


                }
                var newwallet = _db.Wallets.FirstOrDefault(w => w.ApplicationUserId == user.Id);
                newwallet.Balance = newwallet.Balance + 100;


                

                var walletTransaction = new WalletTransaction
                {
                    WalletId = newwallet.WalletId,
                    TransactionDate = DateTime.Now,
                    TransactionAmount =100 ,
                    OrderId = null,
                    description = "reference refund"




                };
                _db.WalletTransactions.Add(walletTransaction);
                



                _db.SaveChanges();

                    


                }




            }





        

        }
    }
