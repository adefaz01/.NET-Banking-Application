using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Admin.CustomAttribute;


namespace Admin.Models;

    public class Customer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)] 
        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "CustomerID must be a 4-digit number.")]
        public int CustomerID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage ="Length exceeded 50 characters")]
        public string Name { get; set; }

        [StringLength(11, ErrorMessage = "TFN must be of the format: XXX XXX XXX")]
        [RegularExpression("^[0-9]{3} [0-9]{3} [0-9]{3}$", ErrorMessage = "TFN must be of the format: XXX XXX XXX.")]
        public string TFN { get; set; }

        [StringLength(50, ErrorMessage = "Length exceeded 50 characters")]
        public string Address { get; set; }

        [StringLength(40, ErrorMessage = "Length exceeded 30 characters")]
        public string City { get; set; }

        [StringLength(3, ErrorMessage ="Not a Valid Australian State")]
        [AustralianState(ErrorMessage = "Not a valid Australian State")]
        public string State { get; set; }

        [StringLength(4, ErrorMessage = "Postcode must be a 4-digit number.")]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Postcode must be a 4-digit number.")]
        public string Postcode { get; set; }

        [RegularExpression("^04\\d{2} \\d{3} \\d{3}$", ErrorMessage = "Mobile must be of the format: 04XX XXX XXX.")]
        [StringLength(12, ErrorMessage = "Mobile must be of the format: 04XX XXX XXX.")]
        public string Mobile { get; set; }
    
        [Required]
        public bool Islocked {  get; set; }

        
        public List<Account> Accounts { get; set; }

        [NotMapped]
        public Login Login { get; set; }
        
    }

