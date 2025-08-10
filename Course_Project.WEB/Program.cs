using Course_Project.Application.Services;
using Course_Project.DataAccess.Data;
using Course_Project.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("NotAuthenticated", policy =>
    {
        policy.RequireAssertion(context => !context.User.Identity.IsAuthenticated);
    });
});
builder.Services.AddIdentity<User, IdentityRole>(opt =>
{
    opt.Password.RequiredLength = 1;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireDigit = false;
    opt.SignIn.RequireConfirmedEmail = false;
    opt.User.RequireUniqueEmail = true;
    opt.User.AllowedUserNameCharacters = null;
}).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddControllersWithViews().AddDataAnnotationsLocalization().AddViewLocalization(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
