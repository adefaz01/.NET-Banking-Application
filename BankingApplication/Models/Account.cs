using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BankingApplication.Models
{
    public class Account
    {
        [Key]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "CustomerID must be a 4-digit number.")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccountNumber { get; set; }

        [Required]
        [RegularExpression("^[CS]$", ErrorMessage = "AccountType must be 'C' or 'S'.")]
        public string AccountType { get; set; }

        public int CustomerID { get; set; }
        public Customer Customer { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal Balance { get; set; }

        [NotMapped]
        public List<Transaction> Transactions { get; set; }
        public List<BillPay> BillPays { get; set; }



    }
}
