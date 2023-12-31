﻿using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Linq;

using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BulkyBookWeb.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class SalesReportController : Controller
{
  
    private readonly ApplicationDbContext _db;

    public SalesReportController(ApplicationDbContext db)
    {
        _db = db;
        
    }



    public IActionResult Index(DateTime? fromDate, DateTime? toDate)
    {
        // If fromDate or toDate is not provided, set default values
        fromDate ??= DateTime.MinValue;
        toDate ??= DateTime.MaxValue;

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

        // Pass the sales report entries and total sales to the view using ViewBag
        ViewBag.SalesReportEntries = salesReportEntries;
        ViewBag.TotalCount = totalSales.TotalCount;
        ViewBag.TotalAmount = totalSales.TotalAmount;

        return View(salesReportEntries);


    }


}


    


