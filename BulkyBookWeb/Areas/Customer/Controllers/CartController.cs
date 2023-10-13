using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using BulkyBookWeb.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {


            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                var offers = _unitOfWork.Offer.GetAll(p => p.ExpiryDate >= DateTime.Now && p.StartDate <= DateTime.Now);

                var offer = offers.FirstOrDefault(p => p.ProductId == cart.ProductId || p.CategoryId == cart.Product.CategoryId);
                if (offer != null)
                {
                    cart.Product.OfferAmount = ((cart.Product.Price) * (offer.Percentage / 100)) * cart.Count;
                }
            }

            var totalOfferAmount = ShoppingCartVM.ListCart.Sum(cart => cart.Product.OfferAmount);
            ShoppingCartVM.TotalOfferAmount = totalOfferAmount;

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count) - cart.Product.OfferAmount;
            }

            return View(ShoppingCartVM);
        }



        public IActionResult Summary(int? addressId = -1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                includeProperties: "Product"),
                OrderHeader = new()
            };




            foreach (var cart in ShoppingCartVM.ListCart)
            {
                var offers = _unitOfWork.Offer.GetAll(p => p.ExpiryDate >= DateTime.Now && p.StartDate <= DateTime.Now);

                var offer = offers.FirstOrDefault(p => p.ProductId == cart.ProductId || p.CategoryId == cart.Product.CategoryId);
                if (offer != null)
                {
                    cart.Product.OfferAmount = ((cart.Product.Price) * (offer.Percentage / 100)) * cart.Count;
                }
            }


            // Calculate and set the total offer amount
            var totalOfferAmount = ShoppingCartVM.ListCart.Sum(cart => cart.Product.OfferAmount);
            ShoppingCartVM.TotalOfferAmount = totalOfferAmount;




            var wallet = _unitOfWork.Wallet.GetFirstOrDefault(w => w.ApplicationUserId == claim.Value);
            if (wallet != null)
            {
                ShoppingCartVM.WalletBalance = wallet.Balance;
            }
            else
            {
                // Handle the case where the wallet doesn't exist for the user
                ShoppingCartVM.WalletBalance = 0; // or any default value you prefer
            }
            if (addressId <= 0)
            {

                ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(
                    u => u.Id == claim.Value);


                ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
                ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
                ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
                ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
                ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
                ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            }



            else

            if (addressId > 0)
            {

                var address = _unitOfWork.Address.GetFirstOrDefault(p => p.id == addressId);
                ShoppingCartVM.OrderHeader.Name = address.Name;
                ShoppingCartVM.OrderHeader.PhoneNumber = address.PhoneNumber;
                ShoppingCartVM.OrderHeader.PostalCode = address.PostalCode;
                ShoppingCartVM.OrderHeader.State = address.State;
                ShoppingCartVM.OrderHeader.StreetAddress = address.StreetAddress;
                ShoppingCartVM.OrderHeader.City = address.City;





            }
            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price,
                    cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            ShoppingCartVM.OrderHeader.OrderTotal -= totalOfferAmount;
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPOST(bool cashOnDelivery = false)
        {
            ShoppingCartVM.OrderHeader.CashOnDelivery = cashOnDelivery;
            if (ShoppingCartVM.OrderHeader.CashOnDelivery = cashOnDelivery)
            {

                ShoppingCartVM.OrderHeader.PaymentMode = "CashonDelivery";
            }
            else
            {
                ShoppingCartVM.OrderHeader.PaymentMode = "Card";
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                includeProperties: "Product,Coupon");


            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;


            var offers = _unitOfWork.Offer.GetAll(p => p.ExpiryDate >= DateTime.Now && p.StartDate <= DateTime.Now);

            foreach (var cart in ShoppingCartVM.ListCart)
            {

                var offer = offers.FirstOrDefault(p => p.ProductId == cart.ProductId || p.CategoryId == cart.Product.CategoryId);
                if (offer != null)
                {
                    cart.Product.OfferAmount = cart.Product.Price-((cart.Product.Price) * (offer.Percentage / 100)) ;
                    //cart.Product.OfferAmount = ((cart.Product.Price) * (offer.Percentage / 100)) * cart.Count;
                }
            }


            // Calculate and set the total offer amount
            var totalOfferAmount = ShoppingCartVM.ListCart.Sum(cart => cart.Product.OfferAmount);
            ShoppingCartVM.TotalOfferAmount = totalOfferAmount;



            BulkyBook.Models.Coupon coupon = null;
            foreach (var cart in ShoppingCartVM.ListCart)
            {
                if (cart.Coupon != null)
                {
                    coupon = cart.Coupon;

                }
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price,
                    cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            //Check coupon is valid

            ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.OrderTotal - totalOfferAmount;

            double couponAmount = 0.0;
            var OrderTotal = ShoppingCartVM.OrderHeader.OrderTotal;
            if (coupon != null)
            {
                if (coupon.OfferType == "Percentage")
                {
                    couponAmount = ((double)coupon.OfferValue / 100) * OrderTotal;
                }
                else if (coupon.OfferType == "FixedAmount")
                {

                    couponAmount = coupon.OfferValue;

                }
                ShoppingCartVM.OrderHeader.CouponId = coupon.Id;
                ShoppingCartVM.OrderHeader.CouponAmount = couponAmount;
                ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.OrderTotal - couponAmount;


            }
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);
            if (applicationUser.CompanyId.GetValueOrDefault() == 0 && ShoppingCartVM.OrderHeader.CashOnDelivery == true)
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();
            foreach (var cart in ShoppingCartVM.ListCart)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }


           

            if (applicationUser.CompanyId.GetValueOrDefault() == 0 && ShoppingCartVM.OrderHeader.CashOnDelivery == false)
            {
                //stripe settings 
                var domain = "https://" + Request.Host.Value + "/";
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                {
                  "card",
                },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + $"customer/cart/index",
                };
                var cartItemCount = ShoppingCartVM.ListCart.Count();
                var eachItemCouponAmount = couponAmount / cartItemCount;
                foreach (var item in ShoppingCartVM.ListCart)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(((item.Product.OfferAmount==0? item.Price: item.Product.OfferAmount) - (eachItemCouponAmount / item.Count)) * 100),//20.00 -> 2000
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            },


                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);

                }

                var service = new SessionService();
                Session session = service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            
            else {
				return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
			}

        }
        [HttpGet]
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser");

            var orderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == id, includeProperties: "Product");



            foreach (var item in orderDetail)
            {
                var count = item.Count;
                item.Product.Stock = item.Product.Stock - count;
            }
            _unitOfWork.Save();
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment && orderHeader.PaymentMode == "Card")

            {

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                //check the stripe status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(id, orderHeader.SessionId, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }

            }
            _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Bulky Book", "<p>New Order Created</p>");
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId ==
            orderHeader.ApplicationUserId).ToList();
            HttpContext.Session.Clear();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);
        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            var product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == cart.ProductId);
            if (cart.Count >= product.Stock)
            {
                TempData["error"] = "You cant order more than  " + product.Stock;
                return RedirectToAction(nameof(Index));

            }
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            if (cart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
                var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1;
                HttpContext.Session.SetInt32(SD.SessionCart, count);
            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.SessionCart, count);
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult CouponValidation(string CouponCode)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var shoppingcart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value);


            if ( ShoppingCartVM!=null && ShoppingCartVM.TotalOfferAmount > 0 )
            {

               return BadRequest(constants.couponcantbeappledwithoffer);

            }
            var coupon = _unitOfWork.Coupon.GetFirstOrDefault(p => p.CouponCode == CouponCode);
            if (coupon == null)
            {
                return BadRequest("Invalid Coupon ");

            }
            if (coupon.ExpiryDate < DateTime.Now)
            {
                return BadRequest(constants.couponexpired);
            }
            if (coupon.StartDate > DateTime.Now)
            {
                return BadRequest(constants.couponisnotyetvalid);

            }
            
           



            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                includeProperties: "Product,Coupon"),
                OrderHeader = new()
            };
            double couponAmount = 0.0;


            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price,
                    cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
			if (ShoppingCartVM.OrderHeader.OrderTotal < coupon.minimumCartAmount)
			{

				return BadRequest(constants.couponisnotapplicable);
			}

			var OrderTotal = ShoppingCartVM.OrderHeader.OrderTotal;
            if (coupon != null)
            {
                if (coupon.OfferType == "Percentage")
                {     
                    couponAmount = ((double)coupon.OfferValue / 100) * OrderTotal;
                    if(couponAmount>coupon.maxRedeemableAmt)
                    {

                        couponAmount = coupon.maxRedeemableAmt?? couponAmount;
                    }
                }
                else if (coupon.OfferType == "FixedAmount"   && coupon.minimumCartAmount<=OrderTotal)
                {

                    couponAmount = coupon.OfferValue;

                }
            }
            foreach (var item in shoppingcart)
            {
                item.CouponId = coupon.Id;
                _unitOfWork.ShoppingCart.Update(item);
            }
            _unitOfWork.Save();
            OrderTotal = OrderTotal - couponAmount;
            ShoppingCartVM.OrderHeader.OrderTotal = OrderTotal;
            ShoppingCartVM.OrderHeader.CouponAmount = couponAmount;


            return new JsonResult(ShoppingCartVM);

        }
        public IActionResult Invoice()
        {
            return View();

        }

        private double GetPriceBasedOnQuantity(double quantity, double price, double price50, double price100)
        {
            if (quantity <= 50)
            {
                return price;
            }
            else
            {
                if (quantity <= 100)
                {
                    return price50;
                }
                return price100;
            }
        }
        public IActionResult PayWithWallet()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                includeProperties: "Product,Coupon");

            ShoppingCartVM.OrderHeader.PaymentMode = "Wallet";
            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;

            BulkyBook.Models.Coupon coupon = null;
            foreach (var cart in ShoppingCartVM.ListCart)
            {
                if (cart.Coupon != null)
                {
                    coupon = cart.Coupon;

                }
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price,
                    cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            //Check coupon is valid



            double couponAmount = 0.0;
            var OrderTotal = ShoppingCartVM.OrderHeader.OrderTotal;
            if (coupon != null)
            {
                if (coupon.OfferType == "Percentage")
                {
                    couponAmount = ((double)coupon.OfferValue / 100) * OrderTotal;
                }
                else if (coupon.OfferType == "FixedAmount")
                {

                    couponAmount = coupon.OfferValue;

                }
                ShoppingCartVM.OrderHeader.CouponId = coupon.Id;
                ShoppingCartVM.OrderHeader.CouponAmount = couponAmount;
                ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.OrderTotal - couponAmount;
            }


            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);





            if (applicationUser.CompanyId.GetValueOrDefault() == 0 && ShoppingCartVM.OrderHeader.CashOnDelivery == false)
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();
            foreach (var cart in ShoppingCartVM.ListCart)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();

            }












            var wallet = _unitOfWork.Wallet.GetFirstOrDefault(w => w.ApplicationUserId == claim.Value);

            if (wallet == null)
            {

                TempData["Error"] = constants.walletnotfound;
                return RedirectToAction("Summary");
            }

            if (ShoppingCartVM.OrderHeader.OrderTotal <= wallet.Balance)
            {

                var walletTransaction = new WalletTransaction
                {
                    WalletId = wallet.WalletId,
                    TransactionDate = DateTime.Now,
                    TransactionAmount = ShoppingCartVM.OrderHeader.OrderTotal * -1, // Deduct the order total
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    description = "Payment for order #" + ShoppingCartVM.OrderHeader.Id
                };


                _unitOfWork.WalletTransaction.Add(walletTransaction);
                _unitOfWork.Save();


                wallet.Balance -= ShoppingCartVM.OrderHeader.OrderTotal;
                _unitOfWork.Wallet.Update(wallet);
                _unitOfWork.Save();

                //		 Update order status
                _unitOfWork.OrderHeader.UpdateStatus(ShoppingCartVM.OrderHeader.Id, SD.PaymentStatusApproved, SD.StatusApproved);
                _unitOfWork.Save();

                TempData["Success"] = constants.orderpaidwithwallet;
                return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
            }
            else
            {
                TempData["Error"] = constants.insufficientfund;
                return RedirectToAction("Summary");
            }
        }



    }
}