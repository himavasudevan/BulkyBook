using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class CombinedDashboardVM
    {


        public List<MonthlySalesData> MonthlySalesData { get; set; }
        public List<SalesReport> SalesReportData { get; set; }
    }
}
