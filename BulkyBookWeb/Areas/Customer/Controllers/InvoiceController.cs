using BulkyBook.DataAccess;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _db;

        public InvoiceController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public IActionResult Index(int orderId)
        {
            // Retrieve data from the OrderHeader table
            var orderHeader = _db.OrderHeaders
                .Include(o => o.ApplicationUser) // Include related user data if needed
                .FirstOrDefault(o => o.Id == orderId);

            if (orderHeader == null)
            {
                return NotFound(); // Handle the case when the order is not found
            }

            // Retrieve data from the OrderDetail table for the given orderId
            var orderDetails = _db.OrderDetail
                .Include(od => od.Product)
                .Where(od => od.OrderId == orderId)
                .ToList();

            // Pass the retrieved data to the view
            var invoiceViewModel = new InvoiceVM()
            {
                OrderHeader = orderHeader,
                OrderDetails = orderDetails
            };

            return View(invoiceViewModel);
        }
    }
}
