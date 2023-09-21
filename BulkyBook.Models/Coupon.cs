using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BulkyBook.Models
{
	public class Coupon : IValidatableObject
    {

		[Key]
		public int Id { get; set; }
        [Display(Name = "Start Date"), DataType(DataType.Date), Required]
        public DateTime StartDate { get; set; }
      
        [Display(Name = "Expiry Date"), DataType(DataType.Date), Required]

        public DateTime ExpiryDate { get; set; }

        [Required]
        public string CouponCode { get; set; }
		[Required]
		public string OfferType { get; set; }
        public int OfferValue {get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            if (ExpiryDate <= StartDate)
            {
                errors.Add(new ValidationResult($"{nameof(ExpiryDate)} needs to be greater than Start Date.", new List<string> { nameof(ExpiryDate) }));
            }
            return errors;
        }
    }
}
