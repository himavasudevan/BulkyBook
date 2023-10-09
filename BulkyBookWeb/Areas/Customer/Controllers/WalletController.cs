using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using System.Security.Claims;
using Stripe;
using Stripe.Checkout;
using BulkyBook.DataAccess.Repository;
using System.Collections.Generic;
using System.Security.Policy;
using MailKit.Search;
using System.Linq;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class WalletController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public WalletController(IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var wallet = _unitOfWork.Wallet.GetFirstOrDefault(W => W.ApplicationUserId == claim.Value);
            if (wallet == null)
            {



                var newWallet = new Wallet
                {


                    ApplicationUserId = claim.Value,
                    Balance = 0

                };
                _unitOfWork.Wallet.Add(newWallet);
                _unitOfWork.Save();





                var walletViewModel = new WalletVM
                {
                    Balance = newWallet.Balance
                };

                return View(walletViewModel);

            }
            else
            {

                var walletViewModel = new WalletVM
                {
                    Balance = wallet.Balance
                };

                return View(walletViewModel);
            }
        }
        public IActionResult ViewTransactions()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var wallettransactions = _unitOfWork.WalletTransaction.GetAll(p=>p.Wallet.ApplicationUserId==claim.Value);
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //var wallettransactions = _unitOfWork.WalletTransaction.GetAll(p => p.Wallet.ApplicationUserId == claim.Value, includeProperties: "Wallet");
            var wallethistoryviewmodel = wallettransactions.Select(p=>new WalletTRansactionVM()        
            {
                Id = p.Id,
                TransactionDate = p.TransactionDate,
                TransactionAmount = p.TransactionAmount,
                Description = p.description,
                OrderId = p.OrderId


            }).ToList();
            return View(wallethistoryviewmodel);
            

        }









    }


}
