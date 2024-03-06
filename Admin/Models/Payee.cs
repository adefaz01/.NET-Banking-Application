using Admin.CustomAttribute;
using System.ComponentModel.DataAnnotations;

namespace Admin.Models
{
    public class Payee
    {
        public int PayeeID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Address { get; set; }

        [Required]
        [StringLength(40)]
        public string City { get; set; }

        [Required]
        [StringLength(3)]
        [AustralianState(ErrorMessage = "Not a valid Australian State")]
        public string State { get; set; }

        [Required]
        [StringLength(4)]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Postcode must be a 4-digits.")]

        public string Postcode { get; set; }

        [Required]
        [StringLength(14)]
        [RegularExpression("^\\(0[0-9]\\) [0-9]{4} [0-9]{4}$", ErrorMessage = "Phone must be of the format: (0X) XXXX XXXX.")]
        public string Phone { get; set; }

        public List<BillPay> BillPays { get; set; }


    }
}
