using Librerias.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Librerias.Pages.Configuraciones.Usuarios
{
    public class EditModel : PageModel {
        private readonly Librerias.Models.LibreriasContext _context; 
        public EditModel(LibreriasContext context) {
            _context = context;
        }

        [BindProperty]
        public Models.Usuarios Usuarios { get; set; }
        public SelectList MotosLista { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id == null) {
                return NotFound();
            }
          
            Usuarios = await _context.Usuarios.FirstOrDefaultAsync(m => m.Id == id); 
           
            if (Usuarios == null) {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }
              
            
          
            Usuarios.Clave = Usuarios.Clave; 
            _context.Attach(Usuarios).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!UsuariosExists(Usuarios.Id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool UsuariosExists(int id) {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
