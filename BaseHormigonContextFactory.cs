using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using CalculoBasesAIE.Models;

namespace CalculoBasesAIE
{
    public class BaseHormigonContextFactory : IDesignTimeDbContextFactory<BaseHormigonContext>
    {
        public BaseHormigonContext CreateDbContext(string[] args)
        {
            // Usar DATABASE_URL si está presente, o fallback local
            var rawUrl = Environment.GetEnvironmentVariable("DATABASE_URL")
                         ?? "postgresql://postgres:password@localhost:5432/railway";

            var uri = new Uri(rawUrl);
            var userInfo = uri.UserInfo.Split(':');

            var connStr = $"Host={uri.Host};Port={uri.Port};Username={userInfo[0]};Password={userInfo[1]};Database={uri.AbsolutePath.TrimStart('/')};SSL Mode=Require;Trust Server Certificate=true";

            var optionsBuilder = new DbContextOptionsBuilder<BaseHormigonContext>();
            optionsBuilder.UseNpgsql(connStr);

            return new BaseHormigonContext(optionsBuilder.Options);
        }
    }
}