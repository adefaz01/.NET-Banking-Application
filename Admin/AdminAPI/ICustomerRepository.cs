using Admin.Models;

namespace Admin.AdminAPI;

    // Interface for the Admin API
    public interface ICustomerRepository
    {
    IEnumerable<Customer> GetCustomers();
    Customer GetCustomerByID(int CustomerID);
    void UpdateCustomer(Customer customer);
    void LockCustomerLogin(int CustomerID);
    void UnlockCustomerLogin(int CustomerID);
}

