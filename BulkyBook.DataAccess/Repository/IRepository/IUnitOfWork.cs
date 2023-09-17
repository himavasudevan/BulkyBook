using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category {  get; }
        ICoverTypeRepository CoverType {  get; }
        IProductRepository Product { get; }
        ICompanyRepository Company {  get; }
        IShoppingCartRepository ShoppingCart {  get; }
        IApplicationUserRepository ApplicationUser {  get; }
        IOrderDetailRepository OrderDetail {  get; }
        IOrderHeaderRepository OrderHeader {  get; }
        IWishListRepository WishList { get; }
        IAddressRepository Address { get; }
        ICouponRepository Coupon { get; }
        void Save();
        public void Update();
    }
}
