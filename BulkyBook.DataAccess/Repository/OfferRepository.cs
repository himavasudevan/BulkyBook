using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class OfferRepository : Repository<Offer>, IOfferRepository
    {
        private ApplicationDbContext _db;
        public OfferRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Offer obj)
        {
            _db.Offers.Update(obj);
        }

    }
}
