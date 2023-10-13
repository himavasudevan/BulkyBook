using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace BulkyBookWeb.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class CouponController : Controller
{
	private readonly IUnitOfWork _unitOfWork;
	public CouponController(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public IActionResult Index()
	{
		IEnumerable<Coupon> objCouponList = _unitOfWork.Coupon.GetAll();
		return View(objCouponList);
	}

	//GET
	public IActionResult Create()
	{
		return View();
	}

	//POST
	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Create(Coupon obj)
	{

		if (ModelState.IsValid)
		{


			_unitOfWork.Coupon.Add(obj);
			_unitOfWork.Save();
			TempData["success"] = constants.couponcreated;
			return RedirectToAction("Index");

		}
		return View(obj);
	}

	//		//GET
	public IActionResult Edit(int? id)
	{
		if (id == null || id == 0)
		{
			return NotFound();
		}
		//var categoryFromDb = _db.Categories.Find(id);
		var CouponFromDbFirst = _unitOfWork.Coupon.GetFirstOrDefault(u => u.Id == id);
		//var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);

		if (CouponFromDbFirst == null)
		{
			return NotFound();
		}

		return View(CouponFromDbFirst);
	}

	//		//POST
	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Edit(Coupon obj)
	{

		if (ModelState.IsValid)
		{
			_unitOfWork.Coupon.Update(obj);
			_unitOfWork.Save();
			TempData["success"] =constants.couponcreated;
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
		//var categoryFromDb = _db.Categories.Find(id);
		var CouponFromDbFirst = _unitOfWork.Coupon.GetFirstOrDefault(u => u.Id == id);
		//var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);

		if (CouponFromDbFirst == null)
		{
			return NotFound();
		}

		return View(CouponFromDbFirst);
	}

	//POST
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public IActionResult DeletePOST(int? id)
	{  
		var obj = _unitOfWork.Coupon.GetFirstOrDefault(u => u.Id == id);


        var orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.CouponId == id);
		if (orderHeaders != null && orderHeaders.Any())
		{
			TempData["error"] = constants.couponcantdelete;
			return RedirectToAction("Index");
		}

		else if (obj == null)
		{
			return NotFound();
		}

		else
		{
			_unitOfWork.Coupon.Remove(obj);
			_unitOfWork.Save();
			TempData["success"] = constants.coupondeleted;
			return RedirectToAction("Index");
		}

	}
	

}