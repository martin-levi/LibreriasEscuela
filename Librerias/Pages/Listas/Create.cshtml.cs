using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Librerias.Models; 
using System.Collections.Generic;
using System.Linq;

namespace Librerias.Pages.Listas
{
    public class CreateModel : PageModel
    {
        private readonly LibreriasContext _context;

        public CreateModel(LibreriasContext context)
        {
            _context = context;
        }

        public List<Librerias.Models.Articulos> ArticulosSelectList { get; set; }
        public List<SelectListItem> MaestrasSelecList { get; set; }


        public Dictionary<string, decimal> PreciosPorArticulo { get; set; }

        public void OnGet()
        {
            MaestrasSelecList = new SelectList(_context.Usuarios.Where (g=> g.Admin==false), "Id", "Nombre").ToList();

            ArticulosSelectList = _context.Articulos.OrderBy(g=> g.Descripcion).ToList();
            PreciosPorArticulo = _context.Precios.ToDictionary(p => p.Codigo, p => p.Precio);
        }
    }
}
