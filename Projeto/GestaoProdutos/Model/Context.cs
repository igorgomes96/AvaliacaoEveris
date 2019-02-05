using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoProdutos.Model
{
    public class Context: DbContext
    {
        public Context(DbContextOptions<Context> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Empresa>()
                .HasIndex(e => e.Nome)
                .IsUnique();

            modelBuilder.Entity<Estoque>()
                .HasIndex(h => new { h.Data, h.ProdutoId, h.EmpresaId })
                .IsUnique();


            modelBuilder.Entity<Produto>()
                .HasIndex(p => p.Nome)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Empresa> Empresas { get; set; }
        public virtual DbSet<Produto> Produtos { get; set; }
        public virtual DbSet<Estoque> Estoque { get; set; }

    }
}
