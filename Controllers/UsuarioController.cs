
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Configuration;
using MVC.Models.ViewModels;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

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
        public IActionResult Login(UsuarioLoginViewModel usu) 
        {
            UsuarioViewModel logueado = null;
            HttpClient client = new HttpClient();
            string url = URLBaseApiUsuarios + "Login";
            var tarea = client.PostAsJsonAsync(url, usu);
            tarea.Wait();

            if (tarea.Result.IsSuccessStatusCode)
            {
                var tarea2 = tarea.Result.Content.ReadAsStringAsync();
                tarea2.Wait();

                var loginData = JsonConvert.DeserializeAnonymousType(tarea2.Result, new { Token = "", Rol = "" });
                HttpContext.Session.SetString("token", loginData.Token);
                HttpContext.Session.SetString("usuarioLogueadoMail", usu.Email);
                HttpContext.Session.SetString("usuarioLogueadoRol", loginData.Rol);
   
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
                    ViewBag.Mensaje = "Unexpected Error" + tarea.Result.RequestMessage;
                    return View();
                }
            }
        }
        [HttpGet]
        public IActionResult SignIn()
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
        public IActionResult SignIn(UsuarioViewModel usu)
        {
            HttpClient client = new HttpClient();
            string url = URLBaseApiUsuarios + "Registro";
            var tarea = client.PostAsJsonAsync(url, usu);
            tarea.Wait();

            if (tarea.Result.IsSuccessStatusCode)
            {
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
                    ViewBag.Mensaje = "Unexpected Error" + tarea.Result.RequestMessage;
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

