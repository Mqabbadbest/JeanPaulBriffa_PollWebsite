using DataAccess.DataContext;
using DataAccess.Repositories;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Presentation.ActionFilters;

namespace Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Connection string not found");
            builder.Services.AddDbContext<PollDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<PollDbContext>();
            builder.Services.AddControllersWithViews();

            builder.Services.AddControllers();
            builder.Services.AddScoped<PollRepository>();
            builder.Services.AddScoped<VoteRepository>();
            builder.Services.AddScoped<PollFileRepository>();

            builder.Services.AddRazorPages();

            int pollsSetting = builder.Configuration.GetValue<int>("PollsSetting", 0);

            if (pollsSetting == 0)
            {
                builder.Services.AddScoped<IPollRepository, PollRepository>();
            }
            else if(pollsSetting == 1)
            {
                builder.Services.AddScoped<IPollRepository, PollFileRepository>();
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.MapRazorPages();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
