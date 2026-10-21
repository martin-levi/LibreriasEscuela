using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;

namespace Multidealer.API
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ReporteController : ControllerBase
    {
        public IConfiguration Configuration { get; }

        public ReporteController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

          [HttpGet]
        [Route("{Id}")]
        public IActionResult GetReporte(int Id)
        {
            try
            {
                string conexion;
                
                conexion = Configuration.GetConnectionString("db");

                DbConnectionStringBuilder builder = new();

                builder.ConnectionString = conexion;

                string servidor = builder["Server"].ToString();
                string bdd = builder["Database"].ToString();
                string usuario = builder["Uid"].ToString();
                string clave = builder["Pwd"].ToString();
                conexion = $"servidor={servidor}&bdd={bdd}&usuario={usuario}&clave={clave}";

                //aca hacer IF
                string urlRPT = "https://multidealerfiles.s3.sa-east-1.amazonaws.com/otros/Reporte.rpt";
                string pdfUrl = $"http://ec2-18-229-247-17.sa-east-1.compute.amazonaws.com:81/api/ReporteCustom?urlPdf={urlRPT}&{conexion}&id={Id}";

                Dictionary<string, string> adjunto = new Dictionary<string, string>();
                using WebClient client = new();
                byte[] Adjunto = client.DownloadData(pdfUrl);


                MemoryStream ms = new MemoryStream(Adjunto);
                return new FileStreamResult(ms, "application/pdf");

            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }
        
     
    }
}
