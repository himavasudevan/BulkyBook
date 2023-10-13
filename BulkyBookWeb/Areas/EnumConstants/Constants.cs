

using Microsoft.AspNetCore.Http.HttpResults;

namespace BulkyBookWeb.Controllers;
public class constants
{
	public const string categorynamemustbeunique = "The DisplayOrder cannot exactly match the Name.";

	public const string catgryexisting = "Category already existing";
	public const string catgrycreated = "Category created successfully";
	public const string  uniquedisplayorder="The DisplayOrder cannot exactly match the Name";
	public const string categryupdated = "Category updated successfully";
	public const string catgrydeleted = "Category deleted successfully";
	public const string compantcreated = "Company created successfully";

	public const string companyupdated = "Company updated successfully";
	public const string meassagejson = "Error while deleting";
	public const string companydeletedsucces = "Delete Successful";

	public const string couponcreated = "Coupon created successfully";
	public const string couponupdated = "Coupon updated successfully";
	public const string couponcantdelete = "Coupon used by orders.So cant be deleted";
	public const string coupondeleted = "Coupon deleted successfully";
	public const string covertypecreated = "CoverType created successfully";
	public const string covertypeupdated = "CoverType updated successfully";
	public const string covertypedeleted = "CoverType deleted successfully";

	public const string offercreated = "Offer created successfully";
	public const string offerupdated = "Offer updated successfully";
	public const string offerdeleted = "Offer deleted successfully";
	public const string orderupdated = "Order Details Updated Successfully.";
	public const string orderstatusupdated = "Order Status Updated Successfully.";
	public const string ordershipped = "Order Shipped Successfully.";
	public const string ordercancelled = "Order Cancelled Successfully.";
	public const string productcreated = "Product created successfully";
	public const string productupdated = "Product updated successfully";
	public const string productimagesdeleted = "Product images deleted successfully";
	public const string errorwhiledeleting = "Error while deleting";
	public const string imagedeleted = "Delete Successful";
	public const string couponcantbeappledwithoffer = "coupon cant be applied with the offer";
	public const string couponexpired = "Coupon expired";
	public const string couponisnotyetvalid = "Coupon is not yet valid";
	public const string couponisnotapplicable = "Coupon is not applicable.You should purchase above 1000";
	public const string walletnotfound = "Wallet not found for the user.";
	public const string orderpaidwithwallet = "Order Paid with Wallet Successfully.";
	public const string insufficientfund = "Insufficient funds in the wallet to complete the payment.";
	public const string productremovedfromwishlist = "Product removed from wish list";
	public const string productaddedtowishlist = "Product Added to wish list";
	public const string addressadded = "Address Added successfully";
	public const string addressdeleted = "Address deleted successfully";
	public const string addressupdated = "Address updated successfully";
	public const string reviewalreadyhave = "You have already submitted a review for this product.";
	public const string reviewsubmitted = "Review submitted successfully.";
	public const string validationfailed = "Validation failed.";
	
}

