using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Data.Helpers
{
    public static class DbContextSeeder
    {
        public static void Seedwork<T>(this DbContext context, string filePath) where T : class
        {
            var set = context.Set<T>();

            if (!set.Any())
            {
                var fullPath = Path.Combine(AppContext.BaseDirectory, filePath);
                if (File.Exists(fullPath))
                {
                    var json = File.ReadAllText(fullPath);//ruta completa del json
                    var data = JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });

                    if (data is not null)
                    {
                        set.AddRange(data);
                        context.SaveChanges();
                        Console.WriteLine($" Cargado seed de {typeof(T).Name} desde {filePath}");
                    }
                }
                else
                {
                    Console.WriteLine($" Archivo no encontrado: {fullPath}");
                }
            }
            else
            {
                Console.WriteLine($"Ya existen registros de tipo {typeof(T).Name}, no se hizo seed.");
            }
        }
    }
}
