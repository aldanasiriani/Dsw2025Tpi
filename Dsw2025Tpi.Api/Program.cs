using Dsw2025Tpi.Application.Middleware;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Helpers;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ============================
            // CONTROLLERS
            // ============================
            builder.Services.AddControllers()
                .AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                    x.JsonSerializerOptions.WriteIndented = true;
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                });

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    return new BadRequestObjectResult(context.ModelState);
                };
            });

            // ============================
            // SWAGGER
            // ============================
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Desarrollo de Software",
                    Version = "v1"
                });

                o.CustomSchemaIds(type =>
                    type.FullName!
                        .Replace("+", ".")
                        .Replace("`1", "OfT")
                );

                o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Ingresar el token",
                    Type = SecuritySchemeType.ApiKey,
                });

                o.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // ============================
            // HEALTH CHECKS
            // ============================
            builder.Services.AddHealthChecks();

            // ============================
            // IDENTITY
            // ============================
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<AuthenticateContext>()
            .AddDefaultTokenProviders();

            // ============================
            // JWT
            // ============================
            var jwtConfig = builder.Configuration.GetSection("Jwt");
            var keyText = jwtConfig["Key"] ?? throw new ArgumentNullException("JWT Key");
            var key = Encoding.UTF8.GetBytes(keyText);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig["Issuer"],
                    ValidAudience = jwtConfig["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            // ============================
            // DB CONTEXTS
            // ============================
            builder.Services.AddDbContext<AuthenticateContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Dsw2025TpiDb"));
            });

            builder.Services.AddDbContext<Dsw2025TpiContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Dsw2025TpiDb"));

                options.UseSeeding((context, type) =>
                {
                    var db = (Dsw2025TpiContext)context;

                    db.Seedwork<Customer>("Sources\\customers.json");
                    db.Seedwork<Product>("Sources\\products.json");
                    db.Seedwork<Order>("Sources\\orders.json");
                });
            });

            // ============================
            // CORS
            // ============================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("PermitirFrontend", policy =>
                    policy.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

            // ============================
            // SERVICES
            // ============================
            builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            builder.Services.AddScoped<ProductsManagementService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<AuthenticateService>();
            builder.Services.AddScoped<CustomerService>();

            var app = builder.Build();

            // ============================
            // PIPELINE
            // ============================
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("PermitirFrontend");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHealthChecks("/healthcheck");

            // ======================================================
            // CREAR ROLES (Admin / Customer) AUTOMÁTICAMENTE
            // ======================================================
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles = { "Admin", "Customer" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            app.Run();
        }
    }
}


