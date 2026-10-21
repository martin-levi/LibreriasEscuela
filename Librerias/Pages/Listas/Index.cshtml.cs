using Librerias.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Librerias.Pages.Listas
{
    public class IndexModel : PageModel
    {
        private readonly LibreriasContext _context;

        public IndexModel(LibreriasContext context)
        {
            _context = context;
        }

        public List<Pedido> Pedidos { get; set; } = new();

        public void OnGet()
        {
            bool Admin = Convert.ToBoolean(User.Claims.FirstOrDefault(c => c.Type == "Admin").Value.ToString());
            int IdUsuario= Convert.ToInt16(User.Claims.FirstOrDefault(c => c.Type == "IdUsuario").Value.ToString());
            if (Admin == false)
            {
                Pedidos = _context.Pedido.Include(g => g.Usuarios).Where(g => g.IdUsuarioCreo == IdUsuario)
                .OrderByDescending(p => p.Fecha)
                .ToList();
            }
            else {
                Pedidos = _context.Pedido.Include(g => g.Usuarios)
                    .OrderByDescending(p => p.Fecha)
                    .ToList();
            }
            
        }
    }
}
