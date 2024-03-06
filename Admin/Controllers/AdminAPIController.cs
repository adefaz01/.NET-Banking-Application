    using Admin.AdminAPI;
    using Microsoft.AspNetCore.Mvc;
    using Admin.Models;

    namespace Admin.Controllers;

    [ApiController]
    [Route("api/customer")]
    public class AdminAPIController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public AdminAPIController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // display all customers
        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return _customerRepository.GetCustomers();
        }

        // display customer 
        [HttpGet("{id}")]
        public Customer Get(int id)
        {
            return _customerRepository.GetCustomerByID(id);
        }

        // update customer
        [HttpPut]
        public void Put(Customer customer)
        {
            _customerRepository.UpdateCustomer(customer);
        }

        // lock customer
        [HttpPut("lock/{id}")]
        public void Lock(int id)
        {
            _customerRepository.LockCustomerLogin(id);
        }

        // unlock customer
        [HttpPut("unlock/{id}")]
        public void Unlock(int id)
        {
            _customerRepository.UnlockCustomerLogin(id);
        }







    }

