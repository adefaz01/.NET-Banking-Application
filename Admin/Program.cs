using Admin.AdminAPI;
using Admin.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://localhost:7087");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();


builder.Services.AddDbContext<BankingApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BankingApplicationContext")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "AdminCookie";
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

/*
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
*/



app.UseHttpsRedirection();
app.UseStaticFiles();

//app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
