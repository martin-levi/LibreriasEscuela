using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.Spreadsheet;
using Librerias.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using static ClosedXML.Excel.XLPredefinedFormat;

namespace Librerias.API
{
    [Route("api/pedidos")]
    [Authorize]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly LibreriasContext _context;

        public PedidosController(LibreriasContext context)
        {
            _context = context;
        }

        
        [HttpPost("guardarAlumnos")]
        public IActionResult GuardarAlumnos([FromBody] PedidoCompletoRequest data)
        {
            if (data == null)
                return BadRequest("Datos no válidos.");

            if (data.Detalles == null || data.Detalles.Count == 0)
                return BadRequest("No se recibieron productos.");

            // Obtener el ID del usuario (desde el request o desde el usuario logueado)
            int idUsuario;
            if (data.IdUsuario.HasValue)
            {
                idUsuario = data.IdUsuario.Value;
            }
            else
            {
                var claim = User.FindFirst("IdUsuario");
                if (claim == null)
                    return Unauthorized("Usuario no identificado.");

                idUsuario = int.Parse(claim.Value);
            }

            var pedido = _context.Pedido.FirstOrDefault(p => p.Id == data.PedidoId);
            if (pedido == null)
            {
                pedido = new Pedido
                {
                    NombreLista = string.IsNullOrWhiteSpace(data.NombreLista)
                        ? "Pedido sin nombre"
                        : data.NombreLista,
                    CantidadAlumnos = data.CantidadAlumnos,
                    Fecha = System.DateTime.Now,
                    ImporteTotal = data.Detalles.Sum(d => d.Cantidad * d.Precio),
                    IdUsuarioCreo = idUsuario,
                    NombreEscuela = data.NombreEscuela,
                    NombreSalita = data.NombreSalita,
                    Observaciones = data.Observaciones
                };

                _context.Pedido.Add(pedido);
                _context.SaveChanges();
                data.PedidoId = pedido.Id;
            }
            else
            {
                pedido.NombreLista = data.NombreLista;
                pedido.CantidadAlumnos = data.CantidadAlumnos;
                pedido.ImporteTotal = data.Detalles.Sum(d => d.Cantidad * d.Precio);

                pedido.NombreEscuela = data.NombreEscuela;
                pedido.NombreSalita = data.NombreSalita;
                pedido.Observaciones = data.Observaciones;
                _context.Pedido.Update(pedido);
                _context.SaveChanges();
            }

            // === Guardar productos ===
            var detallesExistentes = _context.PedidoDetalles
                .Where(d => d.IdPedido == pedido.Id)
                .ToList();

            if (detallesExistentes.Any())
            {
                _context.PedidoDetalles.RemoveRange(detallesExistentes);
                _context.SaveChanges();
            }

            foreach (var det in data.Detalles)
            {
                _context.PedidoDetalles.Add(new PedidoDetalle
                {
                    IdPedido = pedido.Id,
                    CodigoArticulo = det.CodigoArticulo,
                    Cantidad = det.Cantidad,
                    Precio = det.Precio
                });
            }

            _context.SaveChanges();

            // Inicializar alumnos
            var alumnos = Enumerable.Range(1, data.CantidadAlumnos)
                .Select(id => new AsignacionAlumno { AlumnoId = id })
                .ToList();

            var ordenados = _context.PedidoDetalles
                .Where(g => g.IdPedido == pedido.Id)
                .OrderByDescending(i => i.Precio)
                .ToList();

            var detallePorCodigo = ordenados.ToDictionary(
                d => d.CodigoArticulo,
                d => new { d.Precio, d.Id }
            );

            foreach (var it in ordenados)
            {
                if (it.Cantidad <= 0) continue;

                int basePorAlumno = it.Cantidad / data.CantidadAlumnos;
                int resto = it.Cantidad % data.CantidadAlumnos;

                // 1) Base igualitaria
                if (basePorAlumno > 0)
                {
                    foreach (var a in alumnos)
                        a.Agregar(it.CodigoArticulo, basePorAlumno, it.Precio);
                }

                // 2) Reparto de restos
                if (resto > 0)
                {
                    var pq = new PriorityQueue<int, decimal>();
                    for (int idx = 0; idx < alumnos.Count; idx++)
                        pq.Enqueue(idx, alumnos[idx].TotalValor);

                    for (int k = 0; k < resto; k++)
                    {
                        pq.TryDequeue(out int idxMin, out _);
                        var alum = alumnos[idxMin];
                        alum.Agregar(it.CodigoArticulo, 1, it.Precio);
                        pq.Enqueue(idxMin, alum.TotalValor);
                    }
                }
            }

            int x = 1;
            foreach (var g in alumnos)
            {
                foreach (var kv in g.CantidadesPorItem)
                {
                    if (!detallePorCodigo.TryGetValue(kv.Key, out var det))
                        throw new InvalidOperationException($"No se encontró detalle para el artículo {kv.Key}.");

                    var nuevo = new PedidoAlumnos
                    {
                        NumeroAlumno = x,
                        IdPedido = pedido.Id,
                        CodigoArticulo = kv.Key,
                        Cantidad = kv.Value,
                        Precio = det.Precio
                    };

                    _context.PedidoAlumnos.Add(nuevo);
                }
                x++;
            }

            _context.SaveChanges();

            return Ok(new
            {
                message = "Pedido y productos guardados correctamente.",
                pedidoId = pedido.Id
            });
        }


        [HttpGet("detalle/{id}")]
        public IActionResult ObtenerDetallePedido(int id)
        {
            var pedido = _context.Pedido.FirstOrDefault(p => p.Id == id);
            if (pedido == null)
                return NotFound();

            var detalles = _context.PedidoDetalles
                .Where(d => d.IdPedido == id)
                .Select(d => new
                {
                    CodigoArticulo = d.CodigoArticulo,
                    Cantidad = d.Cantidad,
                    Precio = d.Precio,
                    Imagen = _context.Articulos.Where(g => g.Codigo == d.CodigoArticulo).FirstOrDefault().Imagen,
                    NombreArticulo = _context.Articulos.Where (g=> g.Codigo==d.CodigoArticulo).FirstOrDefault().Descripcion
                })
                .ToList();

            //// No se recuperan alumnos de la BD: solo se devuelve el monto dividido
            //var alumnosSimulados = Enumerable.Range(1, pedido.CantidadAlumnos)
            //    .Select(i => new
            //    {
            //        Nombre = $"alumno{i}",
            //        Monto = pedido.CantidadAlumnos > 0
            //            ? pedido.ImporteTotal / pedido.CantidadAlumnos
            //            : 0
            //    })
            //    .ToList();

            var alumnosSimulados = _context.PedidoAlumnos
                                .Where(g => g.IdPedido == id)
                                .GroupBy(g => g.NumeroAlumno)
                                .Select(grp => new
                                {

                                    Nombre = grp.Key,
                                    Monto = grp.Sum(x => x.Cantidad * x.Precio),
                                    Id=_context.PedidoAlumnos.Where(g => g.IdPedido == id && g.NumeroAlumno==grp.Key).Min( h => h.Id)
                                })
                                .ToList();


            return Ok(new
            {
                detalles,
                alumnos = alumnosSimulados,
                 nombreEscuela = pedido.NombreEscuela,
                nombreSalita = pedido.NombreSalita,
                observaciones = pedido.Observaciones
            });
        }

        
        public class PedidoCompletoRequest
        {
            public int PedidoId { get; set; }
            public string? NombreLista { get; set; }
            public int CantidadAlumnos { get; set; }
            public int? IdUsuario { get; set; }
            public string? NombreEscuela { get; set; }
            public string? NombreSalita { get; set; }
            public string? Observaciones { get; set; }
            public List<DetalleDTO> Detalles { get; set; } = new();
        }

        public class DetalleDTO
        {
            public string CodigoArticulo { get; set; }
            public int Cantidad { get; set; }
            public decimal Precio { get; set; }
        }
    }
}
