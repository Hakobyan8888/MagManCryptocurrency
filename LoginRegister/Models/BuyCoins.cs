using System.ComponentModel.DataAnnotations;

namespace LoginRegister.Models
{
    public class BuyCoins
    {
        [Required]
        [Display(Name = "Bank Account")]
        public string BankAccount { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public string Amount { get; set; }
    }
}
