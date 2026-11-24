using ExamenFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamenFinal.Data
{
    public class BaseDeDatos : DbContext
    {
        public BaseDeDatos(DbContextOptions<BaseDeDatos> options) : base(options) { }

        public DbSet<Tutor> tutores { get; set; }
        public DbSet<Tesis> tesis { get; set; }
        public DbSet<Usuario> usuarios { get; set; }
    }
}