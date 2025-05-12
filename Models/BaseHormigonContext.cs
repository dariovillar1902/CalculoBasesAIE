using Microsoft.EntityFrameworkCore;

namespace CalculoBasesAIE.Models
{
    public class BaseHormigonContext(DbContextOptions<BaseHormigonContext> options) : DbContext(options)
    {
        public DbSet<BaseHormigon> BasesHormigon { get; set; } = null!;
    }
}
