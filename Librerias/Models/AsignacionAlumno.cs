using System.Collections.Generic;

namespace Librerias.Models
{
    public class AsignacionAlumno
    {
        public int AlumnoId { get; set; }
        public Dictionary<string, int> CantidadesPorItem { get; } = new();
        public decimal TotalValor { get; set; }

        public void Agregar(string codigo, int cantidad, decimal precioUnit)
        {
            if (cantidad <= 0) return;

            if (!CantidadesPorItem.ContainsKey(codigo))
                CantidadesPorItem[codigo] = 0;

            CantidadesPorItem[codigo] += cantidad;
            TotalValor += cantidad * precioUnit;
        }
    }
}
