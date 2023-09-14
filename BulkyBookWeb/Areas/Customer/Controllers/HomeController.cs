using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using NuGet.Packaging.Signing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace BulkyBookWeb.Controllers;
[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index(string SearchString="",string SortOrder="ProductName")
    {
		IEnumerable<Product> productList;

        if (string.IsNullOrWhiteSpace(SearchString))
        {
            // If no search term is provided, show all products.
            productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
        }
        else
        {
            // If a search term is provided, filter products by name.
            productList = _unitOfWork.Product.GetAll(
                filter: p => p.Title.Contains(SearchString),
                includeProperties: "Category,CoverType"
            );
        }
        if (SortOrder == "ProductName")
        {

            productList = productList.OrderBy(p => p.Title);

        }
        else if(SortOrder == "PrizeFromHighToLow")
        {
            productList = productList.OrderByDescending(p => p.Price);

        }
        else
        {
            productList = productList.OrderBy(p => p.Price);

        }


        //IEnumerable<Product> productList = _unitOfWork.Product.GetAll(,includeProperties: "Category,CoverType");

        return View(productList);
    }

    public IActionResult Details(int productId)
    {
        ShoppingCart cartObj = new()
        {
            Count = 1,
            ProductId = productId,
            Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,CoverType,ProductImages"),
        };


        return View(cartObj);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        shoppingCart.ApplicationUserId = claim.Value;

        ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
            u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);


        if (cartFromDb == null)
        {

            _unitOfWork.ShoppingCart.Add(shoppingCart);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
        }
        else
        {
            _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
            _unitOfWork.Save();
        }


        return RedirectToAction(nameof(Index));
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public IActionResult AddWishList(int id,bool isWishListPage=false)
                    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        var existingWishListItem = _unitOfWork.WishList.GetFirstOrDefault(w => w.ProductId == id && w.ApplicationUserId == claim.Value);
        if (existingWishListItem != null)
        {
            
                _unitOfWork.WishList.Remove(existingWishListItem);
            _unitOfWork.Save();
           
            TempData["error"] = "Product removed from wish list";
            if (isWishListPage == false)
            {
                return RedirectToAction("Details", new { ProductId = id });
            }
            else
            {
                return RedirectToAction("WishList");
            }

        }
        else
        {
            

                _unitOfWork.WishList.Add(new WishList() { ProductId = id, ApplicationUserId = claim.Value });
                _unitOfWork.Save();
                TempData["success"] = "Product Added to wish list";

                return RedirectToAction("Details", new { ProductId = id });
            
        }
        
        
        

    }
    public IActionResult WishList()
    {
        return View();

    }
    public IActionResult GetWishList()
    {

        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        var WishListItem = _unitOfWork.WishList.GetAll( w=> w.ApplicationUserId == claim.Value, includeProperties: "Product");
        return Json(new { data = WishListItem });
    }
    public IActionResult Address()
    {
        return View();

    }
    public IActionResult GetAddress()
    {

        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        var AddressDetails = _unitOfWork.Address.GetAll(w => w.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
        return Json(new { data = AddressDetails });
    }
    //GET
   public IActionResult AddAddress()
    {
        
        return View();

    }
    

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddAddress(Address obj)
    {

            if (ModelState.IsValid)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            obj.ApplicationUserId = claim.Value;
            _unitOfWork.Address.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Address Added successfully";
                return RedirectToAction("Address");
            }
        
        return View(obj);
    }
    //public IActionResult Index1()
    //{
    //    IEnumerable<Address> objAddressList = _unitOfWork.Address.GetAll();
    //    return View(objAddressList);
    //}
    public IActionResult DeleteAddress(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        //var categoryFromDb = _db.Categories.Find(id);
        var addressFromDbFirst = _unitOfWork.Address.GetFirstOrDefault(u => u.id == id);
        //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);

        if (addressFromDbFirst == null)
        {
            return NotFound();
        }

        return View(addressFromDbFirst);
    }
    //POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteAddressPOST(int? id)
    {
        var obj = _unitOfWork.Address.GetFirstOrDefault(u => u.id == id);
        if (obj == null)
        {
            return NotFound();
        }

        _unitOfWork.Address.Remove(obj);
        _unitOfWork.Save();
        TempData["success"] = "Address deleted successfully";
        return RedirectToAction("Address");

    }
    //GET
    public IActionResult EditAddress(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        //var categoryFromDb = _db.Categories.Find(id);
        var addressFromDbFirst = _unitOfWork.Address.GetFirstOrDefault(u => u.id == id);
        //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);

        if (addressFromDbFirst == null)
        {
            return NotFound();
        }

        return View(addressFromDbFirst);
    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditAddress(Address obj)
    
    {
        
        if (ModelState.IsValid)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            obj.ApplicationUserId = claim.Value;
            _unitOfWork.Address.Update(obj);
            _unitOfWork.Save();
            TempData["success"] = "Address updated successfully";
            return RedirectToAction("Address");
        }
        return View(obj);
    }




    


}


