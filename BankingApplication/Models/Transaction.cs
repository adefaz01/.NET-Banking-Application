using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BankingApplication.Models
{
    public class Transaction
    {
        public int TransactionID { get; set; }

        [Required]
        [RegularExpression("^[DWTSB]$", ErrorMessage = "TransactionType must be 'D', 'W', 'T', 'S', 'B'")]
        public string TransactionType { get; set; }

        [ForeignKey("Account")]
        public int AccountNumber { get; set; }
        public Account Account { get; set; }

        [ForeignKey("DestinationAccount")]
        public int? DestinationAccountNumber { get; set; }
        public Account DestinationAccount { get; set; }

        [Required]
        [Column(TypeName = "money")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal Amount { get; set; }

        [StringLength(30)]
        public string Comment { get; set; }

        [Required]
        public DateTime TransactionTimeUtc { get; set; }
    }
}
