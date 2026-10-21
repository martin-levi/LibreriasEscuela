using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Librerias.Models
{
    public partial class Usuarios
    {

      
            public int Id { get; set; }
            public string Clave { get; set; }
            public string Email { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Dni { get; set; }
            public DateTime FechaNacimiento { get; set; }
            public DateTime? FechaGrabado { get; set; }
            public bool Admin { get; set; }        
            
            public bool Borrado { get; set; }
            public DateTime? FechaBorrado { get; set; }
        



    }
}
