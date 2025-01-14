using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using AAAErp.IProvider;
using AAAErp.Models;
using AAAErp.Provider;
using AAAErp.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using AAAErp.Models.MISModel;
using Hangfire;
using Hangfire.MySql;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
var misConnectionString = builder.Configuration.GetConnectionString("MisConnectionString");

//builder.Services.AddDbContext<CoreDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<CoreDbContext>(option => option.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddDbContext<MisDbContext>(option => option.UseMySql(misConnectionString, ServerVersion.AutoDetect(misConnectionString)));

builder.Services.AddHangfire(x => x.UseStorage(new MySqlStorage(connectionString, new MySqlStorageOptions { TablesPrefix = "Hangfire_" })));
builder.Services.AddHangfireServer();

builder.Services.AddScoped<ICronJobProvider, CronJobProvider>();
builder.Services.AddTransient<IEmailProvider, EmailProvider>();
builder.Services.AddTransient<IRawQuery, RawQuery>();
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopCenter;
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);//You can set Time   
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/home/login";
            options.SlidingExpiration = true;
            options.AccessDeniedPath = "/home/denied";
            //options.Cookie.Name = ".AspNetCore.Cookies";
            //options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        });

// Add services to the container.
builder.Services.AddControllersWithViews();

// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseHangfireDashboard();
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseNotyf();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.UseRotativa();

DatabaseUpdator.UpdateDatabase(app);
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var recurringJob = services.GetRequiredService<IRecurringJobManager>();
    var cronJob = services.GetRequiredService<ICronJobProvider>();
    //var context = services.GetRequiredService<MisDbContext>();

    //recurringJob.AddOrUpdate("BirthdayMails", () => cronJob.SendBirtdayMails(), Cron.Daily(4, 0));
    recurringJob.AddOrUpdate("BirthdayMails", () => cronJob.SendBirtdayMails(), Cron.Hourly);

    //Cron.Daily();  -  "0 0 * * *"  Every night at 12:00 AM (default UTC time)
    //recurringJob.AddOrUpdate("AttendanceBackup1", () => cronJob.BackupAttendance(), Cron.Daily(backup1Time.Hours, backup1Time.Minutes), TimeZoneInfo.Local);
    //recurringJob.AddOrUpdate("IngressBackup", () => cronJob.AddReccuringJob(), Cron.Daily(6, 30));  // "30 9 * * *"
}

app.Run();