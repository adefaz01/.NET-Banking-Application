using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Admin.Models
{
    public class BillPay
    {
        public int BillPayID { get; set; }

        [ForeignKey("Account")]
        public int AccountNumber { get; set; }
        public Account Account { get; set; }

        public int PayeeID { get; set; }
        public Payee Payee { get; set; }

        [Required]
        [Column(TypeName = "money")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime ScheduleTimeUtc { get; set; }

        [Required]
        [RegularExpression("^[OM]$", ErrorMessage = "Period must be 'O' or 'M'.")]
        public string Period { get; set; }
    }
}
