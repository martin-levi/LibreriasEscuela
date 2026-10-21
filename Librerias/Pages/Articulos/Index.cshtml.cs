using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Librerias.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Librerias.Pages.Configuraciones.Articulos
{
    public class IndexModel : PageModel
    {
        private readonly LibreriasContext _context;
   
        public IList<Librerias.Models.Articulos> Articulos { get; set; }
        public IndexModel(LibreriasContext context)
        {
            _context = context;
        }
          
        public async Task OnGetAsync() {
            Articulos = await _context.Articulos.OrderBy(g => g.Codigo).ToListAsync();
               
     
        }
    }
}
