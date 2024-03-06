using BankingApplication.Models;
namespace BankingApplication.ViewModel;

public class BillPayViewModel
{
    public BillPay BillPay { get; set; }
    public List<Account> accounts { get; set; }
    public List<Payee> Payees { get; set; }


}   
