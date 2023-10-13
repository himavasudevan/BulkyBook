using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace BulkyBookWeb.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class OfferController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public OfferController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<Offer> objOfferList = _unitOfWork.Offer.GetAll(includeProperties:"Category,Product");
        return View(objOfferList);
    }

    //GET
    public IActionResult Create()
    {
        return View();
    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Offer obj)
    {

        if (ModelState.IsValid)
        {


            _unitOfWork.Offer.Add(obj);
            _unitOfWork.Save();
            TempData["success"] = constants.offercreated;
            return RedirectToAction("Index");

        }

       
        return View(obj);
    }
    [HttpGet]
    public IActionResult GetOfferOptions(string offerType)
    {
        if (offerType == "Product")
        {
           
            var productOptions = _unitOfWork.Product.GetAll().Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Title
            }).ToList();

            return Json(productOptions);
        }
        else if (offerType == "Category")
        {
            
            var categoryOptions = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return Json(categoryOptions);
        }

        
        return Json(new List<SelectListItem>());
    }

    //		//GET
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        
        var OfferFromDbFirst = _unitOfWork.Offer.GetFirstOrDefault(u => u.Id == id);
        

        if (OfferFromDbFirst == null)
        {
            return NotFound();
        }

        return View(OfferFromDbFirst);
    }

    //		//POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Offer obj)
    {

        if (ModelState.IsValid)
        {
            _unitOfWork.Offer.Update(obj);
            _unitOfWork.Save();
            TempData["success"] = constants.offerupdated;
            return RedirectToAction("Index");
        }
        return View(obj);
    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        
        var OfferFromDbFirst = _unitOfWork.Offer.GetFirstOrDefault(u => u.Id == id);
        //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);

        if (OfferFromDbFirst == null)
        {
            return NotFound();
        }

        return View(OfferFromDbFirst); 
    }

    //POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePOST(int? id)
    {
        var obj = _unitOfWork.Offer.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return NotFound();
        }

        _unitOfWork.Offer.Remove(obj);
        _unitOfWork.Save();
        TempData["success"] =constants.offerdeleted;
        return RedirectToAction("Index");

    }


}