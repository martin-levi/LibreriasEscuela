using System.ComponentModel.DataAnnotations.Schema;

namespace Librerias.Models
{
    public class Articulos
    {
        public int ID { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }

        [NotMapped]
        public decimal Precio { get; set; }
        public string Imagen { get; set; }
    }
}