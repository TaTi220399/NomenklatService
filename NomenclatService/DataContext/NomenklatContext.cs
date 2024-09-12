using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using NomenklatService.Models;

namespace NomenklatService.DataContext
{
    public class NomenklatContext : DbContext
    {
        public DbSet<Nomenklature> Nomenklature { get; set; }

        public DbSet<Links> Links { get; set; }

        public NomenklatContext()
        {
            try
            {
                var databaseCreator = (Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator);
                databaseCreator.CreateTables();
            }
            catch (Exception ex)
            { }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=SGMC_Test;Username=postgres;Password=123");
        }
    }
}
