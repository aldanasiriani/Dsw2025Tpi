using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.Metrics;
using Dsw2025Tpi.Application.Services;


namespace Dsw2025Tpi.Api;


public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddDbContext<Dsw2025Tpi.Data.Dsw2025TpiContext>(options =>
        {
            options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;");


        }//Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Dsw2025;Integrated Security=True;
               );

        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>)); //Esto indica que para cualquier tipo T,
                                                                                   //cuando se pida IRepository<T>, se inyecte EfRepository<T>.

        builder.Services.AddScoped<ProductsManagementService>();// Servicio de aplicación

        // Controllers + Swagger opcional
        builder.Services.AddControllers();//
        builder.Services.AddEndpointsApiExplorer();//
        builder.Services.AddSwaggerGen();//
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
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
