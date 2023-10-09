using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ListCart { get; set; }
       
        public OrderHeader OrderHeader { get; set; }
        [ValidateNever]
        public double WalletBalance { get; set; }
        
        public double TotalOfferAmount { get; set; }
    }
}
