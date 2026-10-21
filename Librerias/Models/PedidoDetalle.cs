using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Librerias.Models
{
    [Table("PedidoDetalle")]
    public class PedidoDetalle
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Pedido")]
        public int IdPedido { get; set; }  

        
        public string CodigoArticulo { get; set; }

        public int Cantidad { get; set; }

        public decimal Precio { get; set; }

        [NotMapped]
        public string NombreArticulo { get; set; }

        public Pedido Pedido { get; set; }
    }
}
