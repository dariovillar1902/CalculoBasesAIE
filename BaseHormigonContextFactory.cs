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
            // Obtenemos la URL de la base de datos desde la variable de entorno DATABASE_URL
            // Si no existe, usamos una URL local por defecto
            var rawUrl = Environment.GetEnvironmentVariable("DATABASE_URL")
                         ?? "postgresql://postgres:password@localhost:5432/railway";

            // Convertimos la URL en un objeto Uri para extraer host, puerto y credenciales
            var uri = new Uri(rawUrl);

            // Obtenemos usuario y contraseña a partir del UserInfo de la URI
            var userInfo = uri.UserInfo.Split(':');

            // Construimos la cadena de conexión para Npgsql (PostgreSQL)
            var connStr = $"Host={uri.Host};Port={uri.Port};Username={userInfo[0]};Password={userInfo[1]};Database={uri.AbsolutePath.TrimStart('/')};SSL Mode=Require;Trust Server Certificate=true";

            // Configuramos las opciones del DbContext con la cadena de conexión
            var optionsBuilder = new DbContextOptionsBuilder<BaseHormigonContext>();
            optionsBuilder.UseNpgsql(connStr);

            // Retornamos una instancia del DbContext con la configuración indicada
            return new BaseHormigonContext(optionsBuilder.Options);
        }
    }
}
