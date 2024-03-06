using Microsoft.AspNetCore.Mvc;
using Admin.CustomAttribute;
using Admin.AdminAPI;
using Admin.Models;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace Admin.Controllers
{

    [AdminAuthentication]
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _client;
        private HttpClient Client => _client.CreateClient("api");

        public AdminController(IHttpClientFactory client)
        {
            _client = client;
        }
        
        // GET all customers
        public async Task<IActionResult> Index()
        {
            var response = await Client.GetAsync("api/customer");

            var result = await response.Content.ReadAsStringAsync();

            var customers = JsonConvert.DeserializeObject<List<Customer>>(result);

            return View(customers);

        }


        // GET customer by ID
        public async Task<IActionResult> Modify(int id)
        {
            var response = await Client.GetAsync($"api/customer/{id}");

            var result = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(result);

            return View(customer);

        }


        // Update Customer
        [HttpPost]
        public IActionResult Modify(Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            var content =
              new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = Client.PutAsync("api/customer", content).Result;

            return RedirectToAction("Index");

        }

        // lock customer
        public async Task<IActionResult> Lock(int id)
        {
            await Client.PutAsync($"api/customer/lock/{id}",null);

            return RedirectToAction("Index");


        }

        // unlock customer 
        public async Task<IActionResult> Unlock(int id)
        {
            await Client.PutAsync($"api/customer/unlock/{id}", null);

            return RedirectToAction("Index");


        }


    }
}
