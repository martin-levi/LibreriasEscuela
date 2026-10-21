
using Librerias.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Librerias.Pages
{
    [AllowAnonymous]
    public class CrearUsuario : PageModel {

        private readonly LibreriasContext _context;
        [BindProperty]
        public Models.Usuarios Usuarios { get; set; }



        public CrearUsuario(LibreriasContext context) {
            _context = context;
        }

        public async Task OnGetAsync(  int? pageIndex = 1) {
             
              

        }

        public  string EncodePasswordToBase64(string password)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            byte[] inArray = HashAlgorithm.Create("SHA1").ComputeHash(bytes);
            return Convert.ToBase64String(inArray);
        }

        public async Task<IActionResult> OnPostAsync()
        {
         
            Usuarios.Clave = EncodePasswordToBase64(Usuarios.Clave);
            Usuarios.FechaGrabado = DateTime.Now;
            Usuarios.Borrado = false;
            Usuarios.Admin = false;
            _context.Usuarios.Add(Usuarios);
            await _context.SaveChangesAsync();

            return Redirect("/UsuarioCreado.html");
        }


    }
}
