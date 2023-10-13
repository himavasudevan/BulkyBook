using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
	
	public class MonthlySalesData
	{
		public int Month {  get; set; }
		public string MonthName { get; set; }
		public double TotalSaleAmount { get; set; }
		public int TotalSaleCount { get; set; }
	}
}
