using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginRegister.Models
{
    public class BuyCoins
    {
        [Required]
        [Display(Name = "Bank Account")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public string Amount { get; set; }
    }
}
