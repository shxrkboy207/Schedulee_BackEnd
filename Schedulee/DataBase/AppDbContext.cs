using Microsoft.EntityFrameworkCore;
using Schedulee.Models;

namespace Schedulee.DataBase
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Postagem> Postagens { get; set; }

        public DbSet<MeiCadastro> MeiCadastros { get; set; }

        public DbSet<Contrato> Contratos { get; set; }

    }
}
