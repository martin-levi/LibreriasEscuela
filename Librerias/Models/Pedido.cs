using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Librerias.Models
{
    [Table("Pedido")]
    public class Pedido
    {
        [Key]
        public int Id { get; set; }

       
        public string NombreLista { get; set; }

        public int CantidadAlumnos { get; set; }

        public DateTime Fecha { get; set; }

        public decimal ImporteTotal { get; set; }

        
        public int IdUsuarioCreo { get; set; }
        [ForeignKey("IdUsuarioCreo")]
        public Usuarios Usuarios { get; set; }

        public string NombreEscuela { get; set; }
        public string NombreSalita { get; set; }
        public string Observaciones { get; set; }

        public List<PedidoDetalle> Detalles { get; set; } = new();
    }
}
