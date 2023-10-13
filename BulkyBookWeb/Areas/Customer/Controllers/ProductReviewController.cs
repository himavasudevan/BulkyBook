using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBookWeb.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Stripe;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BulkyBookWeb.Controllers;
[Area("Customer")]
[Authorize]
public class ProductReviewController : Controller
{


    private readonly IUnitOfWork _unitOfWork;

    public ProductReviewController(IUnitOfWork unitOfWork)
    {

        _unitOfWork = unitOfWork;
    }
    public IActionResult Index()
    {
        return View();
    }


    //GET
    public IActionResult Create()
    {
        return View();
    }

    //POST

    [HttpPost]




    public IActionResult Create(ProductReview obj)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        obj.ApplicationUserId = claim.Value;

        if (ModelState.IsValid)
        {

            
            // Check if the user has already submitted a review for this product
            var existingReview = _unitOfWork.ProductReview.GetFirstOrDefault(
                p => p.ProductId == obj.ProductId && p.ApplicationUserId == claim.Value);


            if (existingReview != null)
            {
                // Return a custom JSON response for the error case
                return Json(new { success = false, message = constants.reviewalreadyhave });
            }

            // If not, proceed to save the review
            obj.ReviewDate = DateTime.Now;
            _unitOfWork.ProductReview.Add(obj);
            _unitOfWork.Save();

            // Return a custom JSON response for the success case
            return Json(new { success = true, message = constants.reviewsubmitted, savedObject = obj });
        }

        // Return a custom JSON response for validation failure
        return Json(new { success = false, message = constants.validationfailed });
    }

}













