using Librerias.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
namespace Librerias.Pages.Listas
{
    public class ResumenModel : PageModel
    {
        public List<SelectListItem> MaestrasSelecList { get; set; }

        private readonly LibreriasContext _context;

        public ResumenModel(LibreriasContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            MaestrasSelecList = new SelectList(_context.Usuarios.Where(g => g.Admin == false), "Id", "Nombre").ToList();
        }
    }
}
