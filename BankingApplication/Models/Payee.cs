using BankingApplication.CustomAttribute;
using System.ComponentModel.DataAnnotations;

namespace BankingApplication.Models
{
    public class Payee
    {
        public int PayeeID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Length exceeded 50 characters")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Length exceeded 50 characters")]
        public string Address { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "Length exceeded 30 characters")]
        public string City { get; set; }

        [Required]
        [StringLength(3, ErrorMessage = "Not a Valid Australian State")]
        [AustralianState(ErrorMessage = "Not a valid Australian State")]
        public string State { get; set; }

        [Required]
        [StringLength(4, ErrorMessage = "Postcode must be a 4-digit number.")]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Postcode must be a 4-digit number.")]

        public string Postcode { get; set; }

        [Required]
        [StringLength(14, ErrorMessage = "Phone must be of the format: (0X) XXXX XXXX.")]
        [RegularExpression("^\\(0[0-9]\\) [0-9]{4} [0-9]{4}$", ErrorMessage = "Phone must be of the format: (0X) XXXX XXXX.")]
        public string Phone { get; set; }

        public List<BillPay> BillPays { get; set; }


    }
}
