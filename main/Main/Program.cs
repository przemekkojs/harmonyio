using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Main.Data;
using Main.Models;
// using Main.Utils;
using Main.GradingAlgorithm;
using Main.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ";
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorizationBuilder();
    // .AddPolicy(Roles.GetRoleName(Role.Admin), policyBuilder => policyBuilder.RequireRole(Roles.GetRoleName(Role.Admin)))
    // .AddPolicy(Roles.GetRoleName(Role.Creator), policyBuilder => policyBuilder.RequireRole(Roles.GetRoleName(Role.Creator)))
    // .AddPolicy(Roles.GetRoleName(Role.Standard), policyBuilder => policyBuilder.RequireRole(Roles.GetRoleName(Role.Standard)));

builder.Services.AddScoped<ApplicationRepository>();
builder.Services.AddSingleton<IGradingAlgorithm, GradingAlgorithm>();


builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);


builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// using (var scope = app.Services.CreateScope())
// {
//     var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//     var roles = new[] { Role.Admin, Role.Creator, Role.Standard };

//     foreach (var role in roles)
//     {
//         if (!await roleManager.RoleExistsAsync(Roles.GetRoleName(role)))
//             await roleManager.CreateAsync(new IdentityRole(Roles.GetRoleName(role)));
//     }
// }

app.Run();
