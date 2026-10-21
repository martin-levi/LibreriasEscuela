using Librerias.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;

namespace Librerias.API
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UsuariosController : ControllerBase
    {

        private readonly LibreriasContext _context;

        public UsuariosController(LibreriasContext context)
        {
            _context = context;
        }
        
        
        [HttpGet]
        [Route("validarUsuarioExistente/{usuario}")]
        public bool ValidarUsuarioExistente(string usuario)
        {
            return _context.Usuarios.Count(ar => ar.Email.ToUpper() == usuario.ToUpper()) > 0;
        }


        [HttpPost("PostBlanquearClave/{idUsuario}")]
        public ActionResult PostBlanquearClave(int idUsuario)
        {
            Models.Usuarios usu = _context.Usuarios.Where(m => m.Id == idUsuario).FirstOrDefault();

            usu.Clave = Pages.LoginModel.EncodePasswordToBase64("123456");
            _context.Attach(usu).State = EntityState.Modified;
            _context.SaveChangesAsync();

            return Ok(new { status = true, message = "Clave modificada con exito" });


        }

    }
}
