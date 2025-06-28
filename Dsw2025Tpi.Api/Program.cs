using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Helpers;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.Metrics;


namespace Dsw2025Tpi.Api;


public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Servicios
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks();


        builder.Services.AddDbContext<Dsw2025TpiContext>(options =>
        {
            options.UseSeeding((context, type) =>
            {
                var db = (Dsw2025TpiContext)context;
                db.Products.RemoveRange(db.Products);
                db.SaveChanges();

                db.Seedwork<Customer>("Sources\\customers.json");
                db.Seedwork<Product>("Sources\\products.json");
            });

            options.UseSqlServer(builder.Configuration.GetConnectionString("Dsw2025TpiDb"));
            options.UseSeeding((c, t) =>
            {
                ((Dsw2025TpiContext)c).Seedwork<Customer>("Sources\\customers.json");
                ((Dsw2025TpiContext)c).Seedwork<Product>("Sources\\products.json");
            });
            Console.WriteLine(">> Ejecutando seeding de productos...");

        });

        builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        builder.Services.AddScoped<ProductsManagementService>();

        var app = builder.Build(); 

        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/healthcheck");

        

        app.Run();
    }
}