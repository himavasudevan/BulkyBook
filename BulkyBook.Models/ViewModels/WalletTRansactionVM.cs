using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BulkyBook.Models.ViewModels
{
    public class WalletTRansactionVM
    {

        public int Id { get; set; }

        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }

        [Display(Name = "Transaction Amount")]
        public double TransactionAmount { get; set; }

        [Display(Name = "Order Summary")]
        public int? OrderId { get; set; }

        [Display(Name = "Transaction Description")]
        public string Description { get; set; }

    }
}
