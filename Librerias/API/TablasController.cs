using Amazon;
using Microsoft.AspNetCore.Authorization;
using Amazon.S3;
using Amazon.S3.Model;
using Librerias.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.IO;
using System.Linq;


namespace Librerias.API
{
    public partial class ResponseS3
    {
        public bool success;
        public string key;
        public string url;
    }


    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TablasController : ControllerBase {
        private readonly LibreriasContext _context;

        public TablasController(LibreriasContext context) {
            _context = context;
        }
 
        

      
        [HttpPost("PostArchivo")]
        public ActionResult PostArchivo([FromForm] IFormFile std) {

            

            var archivo = std;

            if (archivo.Length > 0) {
                using (var fileStream = new FileStream(archivo.FileName, FileMode.Create)){
                    archivo.CopyTo(fileStream);
                    //Aca meto codigo de excel? funcionara en AWS?
                }
            }
            return Ok(new { status = true, message = "La subida fue corecta" });
        }





        // GET: api/ImagenS3/
        [HttpGet("getPreSignerURL")]
        public string getPreSignerURL(string filename, string carpeta, string ContentType)
        {
            ResponseS3 response = new ResponseS3();
           
            var Bucket = "multidealerfiles";

            try
            {
                using (var client = new AmazonS3Client(  RegionEndpoint.SAEast1))
                {

                    Amazon.AWSConfigsS3.UseSignatureVersion4 = true;
                    GetPreSignedUrlRequest req = new GetPreSignedUrlRequest
                    {
                        BucketName = Bucket,
                        Key = carpeta + filename,
                        Expires = DateTime.Now.AddHours(24),
                        Verb = HttpVerb.PUT

                    };
                    response.key = carpeta + filename;
                    response.url = client.GetPreSignedURL(req);
                }

            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }

            return response.url;
        }


        [HttpPost("PostClave/{idUsuario}/{clave}")]
        public ActionResult PostClave(int idUsuario,string clave)
        { 
            Models.Usuarios usu =   _context.Usuarios.Where(m => m.Id == idUsuario).FirstOrDefault();

            usu.Clave = Pages.LoginModel.EncodePasswordToBase64(clave);
            _context.Attach(usu).State = EntityState.Modified;
            _context.SaveChangesAsync();

            return Ok(new { status = true, message = "Clave modificada con exito" });  

        }


      
    }
}