using System.Data.Entity;

namespace ParqueoCentralWeb.Models
{
    public class ParqueoContext : DbContext
    {
        public ParqueoContext() : base("ParqueoContext")
        {
        }

        public DbSet<Vehiculo> Vehiculos { get; set; }

        public DbSet<EspacioEstacionamiento> Espacios { get; set; }

        public DbSet<MovimientoEstacionamiento> Movimientos { get; set; }
    }
}