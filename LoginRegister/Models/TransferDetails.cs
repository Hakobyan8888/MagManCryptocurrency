using System.ComponentModel.DataAnnotations;

namespace LoginRegister.Models
{
    public class TransferDetails
    {
        [Required]
        [Display(Name = "Balance")]
        public decimal Balance { get; set; }

        [Required]
        [Display(Name = "ToAddress")]
        public string ToAddress { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public string Amount { get; set; }
    }
}
