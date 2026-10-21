using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
    using Librerias.Models;

namespace Librerias.Pages.Configuraciones.Usuarios
{
    public class DeleteModel : PageModel
    {
    private readonly LibreriasContext _context;

    public DeleteModel(LibreriasContext context)
    {
    _context = context;
    }

    [BindProperty]
    public Librerias.Models.Usuarios Usuarios { get; set; }

    public async Task<IActionResult>
        OnGetAsync(int? id)
        {
        if (id == null) return NotFound();

      
        Usuarios = await _context.Usuarios.FirstOrDefaultAsync(m => m.Id == id  );

        if (Usuarios == null) return NotFound();

        return Page();
        }

        public async Task<IActionResult>
            OnPostAsync(int? id)
            {
            if (id == null) return NotFound();

            Usuarios = await _context.Usuarios.FindAsync(id);

            if (Usuarios != null) {
            _context.Usuarios.Remove(Usuarios);
            await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
            }
            }
            }
