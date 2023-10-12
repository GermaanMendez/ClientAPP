
using Microsoft.EntityFrameworkCore;
using MVC.CommonRequest;
using MVC.CommonRequest.Interfaces;

namespace MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //Sesiones usuairos
            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<ILeerContenidoBodyApi, LeerContenidoJsonBodyApi>();
            builder.Services.AddScoped<IObtenerTiposCabañas, ObtenerTiposCabañas>();
            builder.Services.AddScoped<IObtenerAlquilerPorId, ObtenerAlquilerPorId>();
            builder.Services.AddScoped<IObtenerCabañaPorId, ObtenerCabañaPorId>();
            builder.Services.AddScoped<IObtenerTipoCabañaPorId, ObtenerTipoCabañaPorId>();
            builder.Services.AddScoped<IObtenerUsuarioLogueado, ObtenerUsuarioLogueado>();
            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            //sesion usuairo
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}