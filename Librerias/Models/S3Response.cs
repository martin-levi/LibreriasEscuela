using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Librerias.Models
{
    public class S3Response
    {
        public System.Net.HttpStatusCode Status { get; set; }
        public string Message { get; set; }
    }
}
