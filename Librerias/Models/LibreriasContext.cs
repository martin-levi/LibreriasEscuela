using Microsoft.EntityFrameworkCore;


namespace Librerias.Models
{
    public partial class LibreriasContext : DbContext
    {
        public LibreriasContext()
        {
        }

        public LibreriasContext(DbContextOptions<LibreriasContext> options)
            : base(options)
        {
        }
         
       

       
        public virtual DbSet<Usuarios> Usuarios { get; set; }
        public  virtual DbSet<Articulos> Articulos { get; set; }

        public virtual DbSet<Precios> Precios { get; set; }
        public virtual DbSet<Pedido> Pedido { get; set; }
        public virtual DbSet<PedidoDetalle> PedidoDetalles { get; set; }
        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<PedidoAlumnos> PedidoAlumnos { get; set; }


         


    }
}
