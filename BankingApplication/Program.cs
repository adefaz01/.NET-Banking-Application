using BankingApplication.BackgroundServices;
using BankingApplication.Data;
using BankingApplication.Wrapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISessionWrapper, SessionWrapper>();


builder.Services.AddDbContext<BankingApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BankingApplicationContext")));

builder.Services.AddHostedService<BillPayService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => 
{
    options.Cookie.Name = "CustomerCookie";
    options.Cookie.IsEssential = true;
});


var app = builder.Build();


// Seed data.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedData.InitDb(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}


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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
