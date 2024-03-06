using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BankingApplication.Models
{
   public class Login
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(8)]
        [RegularExpression("^[0-9]{8}$", ErrorMessage = "LoginID must be 8 digits.")]
        public string LoginID { get; set; }

        public int CustomerID { get; set; }
        public Customer Customer { get; set; }

        [Required]
        [StringLength(94)]
        public string PasswordHash { get; set; }
    }
}
