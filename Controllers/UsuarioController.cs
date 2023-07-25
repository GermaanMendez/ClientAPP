
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MVC.Models.ViewModels;
using Newtonsoft.Json;

namespace MVC.Controllers
{
    public class UsuarioController : Controller
    {
        public IConfiguration Conf { get; set; }
        public string URLBaseApiUsuarios { get; set; }
        public UsuarioController( IConfiguration cn)
        {
            Conf = cn;
            URLBaseApiUsuarios = Conf.GetValue<string>("ApiUsuarios");
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        public IActionResult Login(UsuarioViewModel usu) 
        {
            HttpClient client = new HttpClient();
            string url = URLBaseApiUsuarios + "Login";
            var tarea = client.PostAsJsonAsync(url, usu);
            tarea.Wait();

            if (tarea.Result.IsSuccessStatusCode)
            {
                var tarea2 = tarea.Result.Content.ReadAsStringAsync();
                tarea2.Wait();

                DTOLogin login = JsonConvert.DeserializeObject<DTOLogin>(tarea2.Result);
                HttpContext.Session.SetString("token", login.Token);
                HttpContext.Session.SetString("usuarioLogueadoMail", usu.Email);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (tarea.Result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ViewBag.Mensaje = "Email and/or password invalid";
                    return View();
                }
                else
                {
                    ViewBag.Mensaje = "Unexpected Error";
                    return View();
                }
            }
        }



    
        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        public IActionResult Logout(string n)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Usuario");
        }








    }
 }

