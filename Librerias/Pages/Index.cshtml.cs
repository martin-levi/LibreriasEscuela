
using Librerias.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Librerias.Pages
{
    public class IndexModel : PageModel {

        private readonly LibreriasContext _context;
    
          
     
       
        public IndexModel(LibreriasContext context) {
            _context = context;
        }

        public async Task OnGetAsync(  int? pageIndex = 1) {
             
              

        }



    }
}
