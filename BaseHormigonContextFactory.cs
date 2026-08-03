using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using CalculoBasesAIE.Models;

namespace CalculoBasesAIE
{
    // Clase para crear el DbContext en tiempo de diseño (migraciones, scaffolding, etc.)
    public class BaseHormigonContextFactory : IDesignTimeDbContextFactory<BaseHormigonContext>
    {
        public BaseHormigonContext CreateDbContext(string[] args)
        {
            // Obtenemos la cadena de conexión desde la variable de entorno DATABASE_URL
            // Si no existe, usamos una conexión local por defecto (SQL Server LocalDB/Express)
            var connStr = Environment.GetEnvironmentVariable("DATABASE_URL")
                         ?? "Server=MSI\\SQLEXPRESS;Database=CalculoBasesDB;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true";

            // Configuramos las opciones del DbContext con la cadena de conexión
            var optionsBuilder = new DbContextOptionsBuilder<BaseHormigonContext>();
            optionsBuilder.UseSqlServer(connStr);

            // Retornamos una instancia del DbContext con la configuración indicada
            return new BaseHormigonContext(optionsBuilder.Options);
        }
    }
}
