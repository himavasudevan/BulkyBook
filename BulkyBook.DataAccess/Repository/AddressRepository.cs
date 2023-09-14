using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{

    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        private ApplicationDbContext _db;
        public AddressRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Address obj)
        {
            _db.Addresses.Update(obj);
        }

    }
}
