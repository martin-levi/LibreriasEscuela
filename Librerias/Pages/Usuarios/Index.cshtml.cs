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

namespace Librerias.Pages.Configuraciones.Usuarios
{
    public class IndexModel : PageModel
    {
        private readonly LibreriasContext _context;
   
        public IList<Librerias.Models.Usuarios> Usuarios { get; set; }
        public IndexModel(LibreriasContext context)
        {
            _context = context;
        }
          
        public async Task OnGetAsync() { 
            Usuarios = await _context.Usuarios.OrderBy(g => g.Email).ToListAsync();
               
     
        }
    }
}
