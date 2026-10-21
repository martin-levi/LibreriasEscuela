using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Librerias.Models;

namespace Librerias.Pages.Configuraciones.Usuarios {
    public class CreateModel : PageModel {
        private readonly LibreriasContext _context;
      

        [BindProperty]
        public Models.Usuarios Usuarios { get; set; }

        public SelectList MotosLista { get; set; }
        public CreateModel(LibreriasContext context) {
            _context = context;
        }

        public IActionResult OnGet() {
            

            return Page();
        }

       

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) return Page(); 
          
            Usuarios.Clave = Pages.LoginModel.EncodePasswordToBase64(Usuarios.Clave);
            Usuarios.FechaGrabado = DateTime.Now; 
            Usuarios.Borrado = false; 

            _context.Usuarios.Add(Usuarios);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
