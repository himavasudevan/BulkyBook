﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
	public class ProductImage
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public string ImageUrl { get; set; }
		public virtual Product Product { get; set; } // Navigation property for the Product
	}
}
