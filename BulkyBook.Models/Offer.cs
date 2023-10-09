using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class Offer : IValidatableObject
    {


        public int Id { get; set; }
        public double Percentage { get; set; }
        public string OfferType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int? CategoryId { get; set; }
		[ValidateNever]
		public Category Category { get; set; }
        public int? ProductId { get; set; }
		[ValidateNever]
		public Product Product { get; set; }
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


