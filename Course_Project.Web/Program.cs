using Course_Project.Application.Interfaces;
using Course_Project.Application.Services;
using Course_Project.DataAccess.Data;
using Course_Project.DataAccess.Interfaces;
using Course_Project.DataAccess.Repostories;
using Course_Project.Domain.Models.UserModels;
using Course_Project.Web.Filters;
using Course_Project.Web.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString, sqlOptions => sqlOptions.MigrationsAssembly("Course_Project.DataAccess"));
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie().AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GoogleClientId"];
    options.ClientSecret = builder.Configuration["GoogleClientSecret"];
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.Events = new OAuthEvents
    {
        OnRemoteFailure = context =>
        {
            Console.WriteLine("OAuth error: " + context.Failure?.Message);
            context.Response.Redirect("/home");
            context.HandleResponse();
            return Task.CompletedTask;
        }
    };
}).AddLinkedIn("LinkedIn","LinkedIn",options=>
{
    options.ClientId = builder.Configuration["LinkedinClientId"];
    options.ClientSecret = builder.Configuration["LinkedinClientSecret"];
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.Events = new OAuthEvents
    {
        OnRemoteFailure = context =>
        {
            context.Response.Redirect("/home");
            context.HandleResponse(); 
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("NotAuthenticated", policy =>
    {
        policy.RequireAssertion(context => !context.User.Identity.IsAuthenticated);
    });
    options.AddPolicy("AdminOrOwner", policy =>
        policy.RequireAssertion(context =>
        {
            if (context.User.Identity==null || !context.User.Identity.IsAuthenticated)
                return false;
            if (context.User.IsInRole("Admin"))
                return true;
            string f_id = string.Empty;
            if(context.Resource is HttpContext hcontext) f_id = Uri.UnescapeDataString(hcontext.Request.Path).Split("/").Last();
            string currentUserId = context.User.Identity.Name;
            return currentUserId == f_id;
        }));

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
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
    builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "ru" };
    options.SetDefaultCulture(CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddHttpClient<ISalesforceService, SalesforceService>();
builder.Services.AddScoped<ICloudService,CloudService>(opt=>
{
    return new CloudService(
           builder.Configuration["GoogleClientId"],
           builder.Configuration["GoogleClientSecret"],
           builder.Configuration["AccessToken"],
           builder.Configuration["RefreshToken"],
           builder.Configuration["ProjectName"],
           builder.Configuration["Mail"]
       );
});
builder.Services.AddSignalR();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddControllersWithViews(opt =>
{
    opt.Filters.Add(typeof(BlockFilter));
})
    .AddViewLocalization() 
    .AddDataAnnotationsLocalization().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate(); 
}
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
    }
}
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
app.UseRequestLocalization();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<CommentHub>("/commentHub");
app.Run();
