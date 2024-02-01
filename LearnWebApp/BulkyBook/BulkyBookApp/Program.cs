using BulkyBookApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BulkyBookApp.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.Services;
using BulkyBookApp.Services;

var builder = WebApplication.CreateBuilder(args);



//Google credential for accessing google api
var configuration = builder.Configuration;
builder.Services.AddAuthentication().AddGoogle(GoogleDefaults.AuthenticationScheme ,options =>
{
	//Force user to choose a Google account to login
    options.Events.OnRedirectToAuthorizationEndpoint = context =>
    {
        context.Response.Redirect(context.RedirectUri + "&prompt=consent");
        return Task.CompletedTask;
    };

    options.ClientId = configuration["Authentication:Google:ClientId"];
	options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
});



// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
	builder.Configuration.GetConnectionString("DefaultConnection")
	));



builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<Cart>(sp => Cart.GetCart(sp));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	//options.IdleTimeout = TimeSpan.FromSeconds(10);
});



//have to conform the email
builder.Services.AddDefaultIdentity<DefaultUser>(options => options.SignIn.RequireConfirmedAccount = false)
    //Add roles
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


//config password validation
builder.Services.Configure<IdentityOptions>(options =>
{
	options.Password.RequireUppercase = false;
	options.Password.RequireNonAlphanumeric = false;
});

var app = builder.Build();


//Seed book data to database
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		SeedData.Initialize(services);
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occured while attempting to seed the database");
	}
}




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
