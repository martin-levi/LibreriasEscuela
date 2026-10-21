using System;
using System.Collections.Generic;
using System.Text;

namespace Sincronizador.Models
{
    public class Articulo
    {
        public int ID { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public decimal Precio { get; set; }
        public Int32 HASH { get; set; }
        public Int32 ID_TANGO { get; set; }
    }
}
