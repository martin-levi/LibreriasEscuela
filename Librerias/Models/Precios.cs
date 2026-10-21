using System.ComponentModel.DataAnnotations;

namespace Librerias.Models
{
    public class Precios
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; }
        public decimal Precio { get; set; }
    }
}
