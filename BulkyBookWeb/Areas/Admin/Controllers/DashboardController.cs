using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Linq;
using System.Globalization;

namespace BulkyBookWeb.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;

    public DashboardController(ApplicationDbContext db)
    {
        _db = db;
    }

    public IActionResult CombinedDashboard(DateTime? fromDate = null, DateTime? toDate = null)
    {
        fromDate ??= DateTime.MinValue;
        toDate ??= DateTime.MaxValue;

        string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames;
        var chartVm = _db.OrderDetail
            .Where(od => od.OrderHeader.OrderDate >= fromDate && od.OrderHeader.OrderDate <= toDate)
            .GroupBy(p => new { p.OrderHeader.OrderDate.Month })
            .Select(p => new MonthlySalesData
            {
                Month = p.Key.Month,
                MonthName = monthNames[p.Key.Month - 1],
                TotalSaleAmount = p.Sum(c => c.Price),
                TotalSaleCount = p.Sum(c => c.Count),
            }).ToList();

        var salesReportEntries = _db.OrderDetail
            .Where(od => od.OrderHeader.OrderDate >= fromDate && od.OrderHeader.OrderDate <= toDate)
            .GroupBy(p => new { p.ProductId, p.Product.Title })
            .Select(p => new SalesReport()
            {
                ProductId = p.Key.ProductId,
                ProductTitle = p.Key.Title,
                TotalCount = p.Sum(c => c.Count),
                TotalAmount = p.Sum(c => c.Price),
            }).ToList();

        var totalSales = new
        {
            TotalCount = salesReportEntries.Sum(entry => entry.TotalCount),
            TotalAmount = salesReportEntries.Sum(entry => entry.TotalAmount)
        };

        

        ViewBag.FromDate = fromDate; // To pass filter values back to the view
        ViewBag.ToDate = toDate;

        var viewModel = new CombinedDashboardVM
        {
            MonthlySalesData = chartVm,
            SalesReportData = salesReportEntries
        };

        return View(viewModel);


    }
}






