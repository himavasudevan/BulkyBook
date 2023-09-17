using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
	public class CouponRepository : Repository<Coupon>, ICouponRepository
	{
		private ApplicationDbContext _db;
		public CouponRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}
        public void Update(Coupon obj)
        {
            _db.Coupons.Update(obj);
        }

    }
}
