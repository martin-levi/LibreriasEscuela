namespace Librerias.Models
{
    public class PedidoAlumnoBorrar
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int AlumnoId { get; set; }

        public Pedido Pedido { get; set; }
        public Alumno Alumno { get; set; }
    }
}
