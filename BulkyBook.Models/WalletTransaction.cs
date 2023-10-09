using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class WalletTransaction
    {

       public int Id { get; set; }    
       public int WalletId { get; set; }
        [ForeignKey("WalletId")]
        
        public Wallet Wallet { get; set; }
        public DateTime TransactionDate { get; set; }
        public double TransactionAmount { get; set; }
        [ValidateNever]
        public int? OrderId { get; set; }
        
        [ForeignKey("OrderId")]
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }
        public string description { get; set; }



    }
}
