using Azure.Storage.Blobs;
using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Migrations;
using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using NuGet.Packaging.Signing;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBookWeb.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;


    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
    }

    public IActionResult Index()
    {
        return View();
    }

    //GET
    public IActionResult Upsert(int? id)
    {
        ProductVM productVM = new()
        {
            Product = new(),
            CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }),
            CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }),
            
        };

        if (id == null || id == 0)
        {
            //create product
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CoverTypeList"] = CoverTypeList;
            return View(productVM);
        }
        else
        {
            productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id,"ProductImages");
           // productVM.Images=_unitOfWork.Product.GetAll(includeProperties: "ProductImages");
            return View(productVM);

            //update product
        }


    }
    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(ProductVM obj, IFormFile? file)
    {

        if (ModelState.IsValid)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\products");
                var extension = Path.GetExtension(file.FileName);

                //if (obj.Product.ImageUrl != null)
                //{
                //    var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                //    if (System.IO.File.Exists(oldImagePath))
                //    {
                //        System.IO.File.Delete(oldImagePath);
                //    }
                //}
                await BlobUploadFileAsync(file, fileName+ extension);


				//using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
    //            {
    //                file.CopyTo(fileStreams);
    //            }
                obj.Product.ImageUrl =   fileName + extension;

            }

            if (obj.Images != null)
            {
                if (obj.Product.ProductImages == null) { 
                obj.Product.ProductImages = new List<ProductImage>();
                    }
                foreach (var image in obj.Images)
                {
                    if (image != null)
                    {
                        string fileName = Guid.NewGuid().ToString();
                        var uploads = Path.Combine(wwwRootPath, @"images\products");
                        var extension = Path.GetExtension(image.FileName);
                        //using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                        //{
                        //    image.CopyTo(fileStreams);
                        //}

                        //obj.Product.ProductImages.Add(new ProductImage() { ImageUrl = @"\images\products\" + fileName + extension });

                        //if (obj.Product.ProductImages != null)
                        //{
                        //    obj.Product.ProductImages.Add(new ProductImage() { ImageUrl = @"\images\products\" + fileName + extension });

                        //}
                        //obj.Product.ProductImages.Add(new ProductImage() { ImageUrl = @"\images\products\" + fileName + extension });
                        await BlobUploadFileAsync(image, fileName + extension);
                        obj.Product.ProductImages.Add(new ProductImage() { ImageUrl =   fileName + extension });

                    }

                }
                
            }
            if (obj.Product.Id == 0)
            {
                _unitOfWork.Product.Add(obj.Product);
                _unitOfWork.Save();
                TempData["success"] = constants.productcreated;
                return RedirectToAction("Index");
            }
            else
            {
                _unitOfWork.Product.Update(obj.Product);
                _unitOfWork.Save();
                TempData["success"] = constants.productupdated;
                return RedirectToAction("Index");
            }
            
        }
        return View(obj);
    }

	public async Task BlobUploadFileAsync(IFormFile file,string filename)
	{
		var blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=bulkyimages;AccountKey=6KF/4Pl3UeKz3qfPAco2lvXVpdLVRahzN39dodjV/RVnim+eQTZBgVMZH2RLhH6dCNt02azTfng6+AStVZIHag==;EndpointSuffix=core.windows.net";
		var blobContainerName = "productimages";

		BlobServiceClient blobService = new BlobServiceClient(blobConnectionString);


		//get the blob container
		var blobStorageContainerName = blobService.GetBlobContainerClient(blobContainerName);
		//get the blob client
		var blobStorageClient = blobStorageContainerName.GetBlobClient(filename);
		//read file stream
		var streamContent = file.OpenReadStream();
		//upload file
		await blobStorageClient.UploadAsync(streamContent);
	}
	


	#region API CALLS
	[HttpGet]
    public IActionResult GetAll()
    {
        var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
        return Json(new { data = productList });
    }
    //POST
    [HttpGet]
    public IActionResult ImageDelete(int? id,int? productId)
    {
        var product = _unitOfWork.Product.GetFirstOrDefault(u =>u.Id==productId ,"ProductImages");

        if (product != null)
        {
            var productImage = product.ProductImages.FirstOrDefault(u => u.Id == id);

            product.ProductImages.Remove(productImage);
            _unitOfWork.Product.Update(product);
            _unitOfWork.Save();

            TempData["success"] = constants.productimagesdeleted;
            return RedirectToAction("Upsert", new {id=productId});

        }
        return NotFound();
    }

    //POST
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = constants.errorwhiledeleting});
        }

        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }

        _unitOfWork.Product.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = constants.imagedeleted });

    }
    
   

    #endregion



}
