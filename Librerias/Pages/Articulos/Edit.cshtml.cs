using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Librerias.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Librerias.Pages.Configuraciones.Articulos
{
    public class archivoS3
    {
        public string Nombre;
        public string Ruta;
        public string Tipo;
        public string Posicion;
    }
    public class EditModel : PageModel {
        private readonly Librerias.Models.LibreriasContext _context; 
        public EditModel(LibreriasContext context) {
            _context = context;
        }

        [BindProperty]
        public Models.Articulos Articulos { get; set; }
        public SelectList MotosLista { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id == null) {
                return NotFound();
            }

            Articulos = await _context.Articulos.FirstOrDefaultAsync(m => m.ID == id); 
           
            if (Articulos == null) {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            List<string> lFotosEliminadas = new List<string>();
            if (Request.Form["archivosS3Eliminados"].ToString() != "")
            {
                lFotosEliminadas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(Request.Form["archivosS3Eliminados"].ToString());
            }

            if (lFotosEliminadas.Count() > 0)
            {
                var bucket = "multidealerfiles";
                using (var client = new AmazonS3Client(RegionEndpoint.SAEast1))
                {
                    string rutaEliminado = string.Empty;
                    var Stock2Aux = await _context.Articulos.AsNoTracking().FirstOrDefaultAsync(x => x.ID == Articulos.ID);
                    for (int x = 0; x <= lFotosEliminadas.Count - 1; x++)
                    {
                        
                            rutaEliminado = Stock2Aux.Imagen; 


                        if (!string.IsNullOrEmpty(rutaEliminado))
                        {
                            var deleteObjectRequest = new DeleteObjectRequest
                            {
                                BucketName = bucket,
                                Key = rutaEliminado
                            };
                            await client.DeleteObjectAsync(deleteObjectRequest);
                        }
                    }
                }
            }

            List<archivoS3> lFotos = new List<archivoS3>();
            if (Request.Form["archivosS3"].ToString() != "")
            {
                lFotos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<archivoS3>>(Request.Form["archivosS3"].ToString());
            }

            if (lFotos.Count > 0)
            {
                for (int x = 0; x <= lFotos.Count - 1; x++)
                {
                    if (lFotos[x].Posicion == "Logo")
                        Articulos.Imagen = "https://multidealerfiles.s3.sa-east-1.amazonaws.com/" + lFotos[x].Ruta;

                }
            }
            else {
                Articulos.Imagen = "";
            }




                _context.Attach(Articulos).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!UsuariosExists(Articulos.ID)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool UsuariosExists(int id) {
            return _context.Articulos.Any(e => e.ID == id);
        }
    }
}
