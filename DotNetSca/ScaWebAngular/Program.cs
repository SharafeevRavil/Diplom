using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScaWebAngular.Data;
using ScaWebAngular.Helpers;
using ScaWebAngular.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//// OLD:

builder.Services.TryAddScoped<IRoleValidator<IdentityRole>, RoleValidator<IdentityRole>>();
builder.Services.TryAddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>>();
builder.Services.TryAddScoped<SignInManager<ApplicationUser>>();
builder.Services.TryAddScoped<RoleManager<IdentityRole>>();
/*builder.Services.TryAddScoped<IUserValidator<ApplicationUser>, UserValidator<ApplicationUser>>();
builder.Services.TryAddScoped<IPasswordValidator<ApplicationUser>, PasswordValidator<ApplicationUser>>();
builder.Services.TryAddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
builder.Services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
builder.Services.TryAddScoped<IRoleValidator<IdentityRole>, RoleValidator<IdentityRole>>();

builder.Services.TryAddScoped<IdentityErrorDescriber>();
builder.Services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<ApplicationUser>>();
builder.Services.TryAddScoped<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<ApplicationUser>>();
builder.Services.TryAddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>>();
builder.Services.TryAddScoped<IUserConfirmation<ApplicationUser>, DefaultUserConfirmation<ApplicationUser>>();
builder.Services.TryAddScoped<UserManager<ApplicationUser>>();
builder.Services.TryAddScoped<SignInManager<ApplicationUser>>();
builder.Services.TryAddScoped<RoleManager<IdentityRole>>();*/




builder.Services.AddDefaultIdentity<ApplicationUser>(/*options => options.SignIn.RequireConfirmedAccount = true*/)
    ;
new IdentityBuilder(typeof(ApplicationUser), typeof(IdentityRole), builder.Services).AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI();
/*
//// NEW:
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);*/
//
/*builder.Services.AddAuthentication(o =>
    {
        o.DefaultScheme = IdentityConstants.ApplicationScheme;
        o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies(o => { });*/
/*builder.Services.AddIdentity<ApplicationUser, IdentityRole>(/*options => options.SignIn.RequireConfirmedAccount = true#1#)
    .AddDefaultUI()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();
////
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(/*options => options.SignIn.RequireConfirmedAccount = true#1#)
    .AddDefaultUI()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();*/


//*core
// Services identity depends on
//builder.Services.AddOptions().AddLogging();

// Services used by identity
//+builder.Services.TryAddScoped<IUserValidator<ApplicationUser>, UserValidator<ApplicationUser>>();
//+builder.Services.TryAddScoped<IPasswordValidator<ApplicationUser>, PasswordValidator<ApplicationUser>>();
//+builder.Services.TryAddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
//+builder.Services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
//+builder.Services.TryAddScoped<IUserConfirmation<ApplicationUser>, DefaulApplicationUserConfirmation<ApplicationUser>>();
// No interface for the error describer so we can add errors without rev'ing the interface
//+builder.Services.TryAddScoped<IdentityErrorDescriber>();
//-uilder.Services.TryAddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory<ApplicationUser>>();
//+builder.Services.TryAddScoped<UserManager<ApplicationUser>>();

/*if (setupAction != null)
{
    services.Configure(setupAction);
}*/

//return new IdentityBuilder(typeof(ApplicationUser), services);
//*
// Hosting doesn't add IHttpContextAccessor by default
//--builder.Services.AddHttpContextAccessor();
// Identity services
//+builder.Services.TryAddScoped<IUserValidator<ApplicationUser>, UserValidator<ApplicationUser>>();
//+builder.Services.TryAddScoped<IPasswordValidator<ApplicationUser>, PasswordValidator<ApplicationUser>>();
//+builder.Services.TryAddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
//+builder.Services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
//-builder.Services.TryAddScoped<IRoleValidator<IdentityRole>, RoleValidator<IdentityRole>>();
// No interface for the error describer so we can add errors without rev'ing the interface
//+builder.Services.TryAddScoped<IdentityErrorDescriber>();
//--builder.Services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<ApplicationUser>>();
//--builder.Services.TryAddScoped<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<ApplicationUser>>();
//-builder.Services.TryAddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>>();
//+builder.Services.TryAddScoped<IUserConfirmation<ApplicationUser>, DefaulApplicationUserConfirmation<ApplicationUser>>();
//+builder.Services.TryAddScoped<UserManager<ApplicationUser>>();
//--builder.Services.TryAddScoped<SignInManager<ApplicationUser>>();
//-builder.Services.TryAddScoped<RoleManager<IdentityRole>>();

/*if (setupAction != null)
{
    services.Configure(setupAction);
}*/

//return new IdentityBuilder(typeof(ApplicationUser), typeof(IdentityRole), services);
////
/*builder.Services.TryAddScoped<IRoleValidator<IdentityRole>, RoleValidator<IdentityRole>>();
builder.Services.TryAddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>>();
builder.Services.TryAddScoped<SignInManager<ApplicationUser>>();*/
////
builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

ServicesDiHelper.RegisterServices(builder.Services);

var app = builder.Build();

DatabaseHelper.UpdateDatabase(app);
await DatabaseHelper.EnsureDatabaseValid(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapFallbackToFile("index.html");
;

app.Run();