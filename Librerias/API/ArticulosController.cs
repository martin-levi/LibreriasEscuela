using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Authorization;
using Librerias.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using static Librerias.API.PedidosController;

namespace Librerias.API
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ArticulosController : ControllerBase
    {
        private readonly LibreriasContext _context;

        public ArticulosController(LibreriasContext context)
        {
            _context = context;
        }


        // Lo usa el SincronizadorTangoLibrerias, que no tiene login
        [AllowAnonymous]
        [HttpPost("multiple")]
        public IActionResult GuardarArticulos(List<Articulos> ListaArticulos)
        {
            bool Error = false;
            List<string> errores=new List<string>();
            if (ListaArticulos == null)
                return BadRequest("Datos no válidos.");

            
            foreach (Articulos art in ListaArticulos) {
                try
                {
                    //Reviso que exista el producto
                    var existe = _context.Articulos.Where(g => g.Codigo == art.Codigo).FirstOrDefault();
                    if (existe == null)
                    {
                        //INSERTO
                        _context.Articulos.Add(art);
                    }
                    else
                    {
                        //MODIFICO
                        existe.Categoria = art.Categoria;
                        existe.Descripcion=art.Descripcion;
                        _context.Attach(existe).State = EntityState.Modified;
                    }
                    _context.SaveChanges();


                    //Aplico lo mismo con el precio
                    var existePrecio = _context.Precios.Where(g => g.Codigo == art.Codigo).FirstOrDefault();
                    if (existePrecio == null)
                    {
                        Precios prec = new Precios();
                        prec.Codigo = art.Codigo;
                        prec.Precio = art.Precio;
                        //INSERTO
                        _context.Precios.Add(prec);
                    }
                    else
                    {
                        //MODIFICO
                        existePrecio.Precio = art.Precio;
                        _context.Attach(existePrecio).State = EntityState.Modified;
                    }
                    _context.SaveChanges();
                }
                catch (Exception ex) {
                    Error = true;
                    errores.Add($"articulo:{art.Codigo} Error:{ex.Message}");

                } 
                
            }


            if (Error == false) {
                return Ok(new
                {
                    message = "Productos y Precios"
                });
            }
            else {
                return BadRequest(new
                {
                    message = string.Join(" | ", errores) 
                });
            }
        }
    }
}
