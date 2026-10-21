using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Librerias.Models
{
    [Table("PedidoAlumnos")]
    public class PedidoAlumnos
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Pedido")]
        public int IdPedido { get; set; }
        public int NumeroAlumno { get; set; }


        public string CodigoArticulo { get; set; }
        public int Cantidad { get; set; } 

        public decimal Precio { get; set; }
         

        public Pedido Pedido { get; set; }
    }
}
