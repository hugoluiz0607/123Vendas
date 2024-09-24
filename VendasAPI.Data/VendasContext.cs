using Microsoft.EntityFrameworkCore;
using VendasAPI.Domain.Entities;

namespace VendasAPI.Data
{
    public class VendasContext : DbContext
    {
        public VendasContext(DbContextOptions<VendasContext> options) : base(options) { }

        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Venda>()
                .Property(v => v.ClienteId)
                .IsRequired();

            modelBuilder.Entity<ItemVenda>()
                .Property(i => i.ProdutoId)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
