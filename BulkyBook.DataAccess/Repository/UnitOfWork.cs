using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            CoverType = new CoverTypeRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            WishList = new WishListRepository(_db);
            Address = new AddressRepository(_db);
            Coupon = new CouponRepository(_db);
            ProductReview = new ProductReviewRepository(_db);
            Wallet = new WalletRepository(_db);
            WalletTransaction = new WalletTransactionRepository(_db);
            Offer = new OfferRepository(_db);
           
        }
        public ICategoryRepository Category { get; private set; }
        public ICoverTypeRepository CoverType { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }

        public IShoppingCartRepository ShoppingCart { get; private set; }

        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IWishListRepository WishList { get; private set; }
        public IAddressRepository Address { get; private set; }
        public ICouponRepository Coupon { get; private set; }
        public IProductReviewRepository ProductReview { get; private set; }
        public IWalletRepository Wallet { get; private set; }
        public IWalletTransactionRepository WalletTransaction { get; private set; }
        public IOfferRepository Offer { get; private set; }

               public void Save() 
        {
            _db.SaveChanges();
        }
        public void Update() { }
    }
}
