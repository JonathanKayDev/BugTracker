using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Settings;
using BugTracker.Services;
using BugTracker.Services.Factories;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = ConnectionService.GetConnectionString(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, 
    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<BTUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true) // modified to use our custom identity BTUser
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<BTUserClaimsPrincipalFactory>()
    .AddDefaultUI() // this does not come by default with AddIdentity
    .AddDefaultTokenProviders();  // this does not come by default with AddIdentity

// Register custom services
builder.Services.AddScoped<IBTRolesService, BTRolesService>();
builder.Services.AddScoped<IBTCompanyInfoService, BTCompanyInfoService>();
builder.Services.AddScoped<IBTProjectService, BTProjectService>();
builder.Services.AddScoped<IBTTicketService, BTTicketService>();
builder.Services.AddScoped<IBTTicketHistoryService, BTTicketHistoryService>();
builder.Services.AddScoped<IBTNotificationService, BTNotificationService>();
builder.Services.AddScoped<IBTInviteService, BTInviteService>();
builder.Services.AddScoped<IBTFileService, BTFileService>();
builder.Services.AddScoped<IBTLookupService, BTLookupService>();

builder.Services.AddScoped<IEmailSender, BTEmailService>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

// register and inject IOptions of type AppSettings
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
// register data ultility service
builder.Services.AddTransient<DeployedDataUtility>();
// Register SecretsService
builder.Services.AddTransient<ISecretsService,SecretsService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// This is critical to be able to seed projects to database
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); //Allow datetime without tz to work

// Demo data - not for deployment
//await DataUtility.ManageDataAsync(app);

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
app.MapRazorPages();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// call SeedService, this must be after setswitch above
var dataService = app.Services.CreateScope().ServiceProvider.GetRequiredService<DeployedDataUtility>();
await dataService.ManageDataAsync(app);

app.Run();
