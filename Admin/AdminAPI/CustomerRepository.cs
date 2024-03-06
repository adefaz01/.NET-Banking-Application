using Admin.Models;
using Admin.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.AdminAPI;



public class CustomerRepository : ICustomerRepository

{ 
    private readonly BankingApplicationContext _context;

    public CustomerRepository(BankingApplicationContext context)
    {
        _context = context;
    }
    // Get a single customer based on Customer ID
    public Customer GetCustomerByID(int CustomerID)
    {
        return _context.Customers.Find(CustomerID);
    }
    // Get all Customers in the DB 
    public IEnumerable<Customer> GetCustomers()
    {
        return _context.Customers.ToList();
    }
    // Lock Customer login based on Customer ID
    public void LockCustomerLogin(int CustomerID)
    {
        var customer = _context.Customers.Find(CustomerID);

        if(customer != null)
        {
            customer.Islocked = true;
            _context.SaveChanges();
        }

    }
    // Unlock Customer Login based on Customer ID
    public void UnlockCustomerLogin(int CustomerID)
    {
        var customer = _context.Customers.Find(CustomerID);

        if (customer != null)
        {
            customer.Islocked = false;
            _context.SaveChanges();
        }
    }
    // Update Customer information
    public void UpdateCustomer(Customer customer)
    {

        _context.Update(customer);
        _context.SaveChanges();
  

    }
}