using Librerias.Clases;
using Librerias.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Librerias.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string EmailOlvidado { get; set; }


        private IHostingEnvironment Environment;

        public string ContraseñaEnviada { get; set; }

        public string Mensaje { get; set; }
        private readonly LibreriasContext _context;

        public LoginModel(LibreriasContext context, IHostingEnvironment _environment) {
            _context = context;
            this.Environment = _environment;
        }

        

        public async Task<IActionResult> OnGet(int? x = 0)
        {
            ContraseñaEnviada = "";
            if (this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToPage("/Listas/Index");
            }

            if (x != 0)
            {
                var User = _context.Usuarios.Where(p => p.Id == x).FirstOrDefault();
                await this.SignInUser(User, false);
                return this.RedirectToPage("/Listas/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnGetLogout() {
            var authenticationManager = Request.HttpContext;
            await authenticationManager.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.RedirectToPage("/Login");
        }
 
        public async Task<IActionResult> OnPostResetPassword()
        {
            string mail = EmailOlvidado;
            string cuerpo ;

            StreamReader arch=new StreamReader(Path.Combine(Environment.WebRootPath,  "mails", "olvido.html"));
            string nuevaContraseña ="";
            Random rnd = new Random();
            for (int x = 1; x <= 8; x++) {
                nuevaContraseña +=  Convert.ToChar(rnd.Next(40) + 48).ToString();
            }
            nuevaContraseña = nuevaContraseña.Replace(";", "@");
            nuevaContraseña = nuevaContraseña.Replace("<", "#");
            nuevaContraseña = nuevaContraseña.Replace(":", "#");

            cuerpo = arch.ReadToEnd();
            cuerpo = cuerpo.Replace("@@USUARIO@@", EmailOlvidado);
            cuerpo = cuerpo.Replace("@@CLAVE@@", nuevaContraseña);
            cuerpo = cuerpo.Replace("@@LINK@@", "https://app.rmgconsultores.com.ar/");

            Models.Usuarios usu = _context.Usuarios.Where(m => m.Email == EmailOlvidado).FirstOrDefault();
            usu.Clave = Pages.LoginModel.EncodePasswordToBase64(nuevaContraseña);
            _context.Attach(usu).State = EntityState.Modified;
            await _context.SaveChangesAsync();


           // Funciones.EnviarMailSES(mail, "Recupero de contraseña", "",cuerpo);
            ContraseñaEnviada = "ok";
            return Page();            
        }



        public async Task<IActionResult> OnPost() {
             var User = _context.Usuarios.Where(p => p.Email == Email && p.Clave == EncodePasswordToBase64(Password)).FirstOrDefault();
            ContraseñaEnviada = "";
            if (User != null ) {             

                await this.SignInUser(User, false);
                return RedirectToPage("/Listas/Index");

            } else {
                Mensaje = "Usuario inválido";
                return Page();
            }
        }

        public static string EncodePasswordToBase64(string password) {
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            byte[] inArray = HashAlgorithm.Create("SHA1").ComputeHash(bytes);
            return Convert.ToBase64String(inArray);
        }

        private async Task SignInUser(Models.Usuarios user, bool isPersistent  ) {
            // Initialization.  
            var claims = new List<Claim>();

            try {
                // Setting  
                claims.Add(new Claim(ClaimTypes.Name, user.Id.ToString()));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())); 
                claims.Add(new Claim("Mail", user.Email.ToString()));
                claims.Add(new Claim("IdUsuario", user.Id.ToString()));
                claims.Add(new Claim("Admin", user.Admin.ToString()));

                var claimIdenties = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimPrincipal = new ClaimsPrincipal(claimIdenties);
                var authenticationManager = Request.HttpContext;

                // Sign In.  
                await authenticationManager.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, new AuthenticationProperties() { IsPersistent = isPersistent });
            } catch (Exception ex) {
                // Info  
                throw ex;
            }
        }
    }
}