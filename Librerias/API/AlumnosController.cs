using Librerias.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Librerias.API
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AlumnosController : ControllerBase
    {
        private readonly LibreriasContext _context;

        public AlumnosController(LibreriasContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public IActionResult Get()
        {
            var alumnos = _context.Alumnos
                .OrderBy(a => a.Nombre)
                .Select(a => new
                {
                    a.AlumnoId,
                    a.Nombre
                })
                .ToList();

            return Ok(alumnos);
        }
    }
}
