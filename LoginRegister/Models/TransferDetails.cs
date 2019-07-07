using System.ComponentModel.DataAnnotations;

namespace LoginRegister.Models
{
    public class TransferDetails
    {
        [Required]
        [Display(Name = "Balance")]
        public decimal Balance { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public string Amount { get; set; }
    }
}
