
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Configuration;
using MVC.CommonRequest.Interfaces;
using MVC.Models.ViewModels.Alquiler;
using MVC.Models.ViewModels.Cabaña;
using MVC.Models.ViewModels.Usuario;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace MVC.Controllers
{
    public class UsuarioController : Controller
    {
        public IConfiguration Conf { get; set; }
        public string URLBaseApiUsuarios { get; set; }
        public string URLBaseApiCabañas { get; set; }
        public ILeerContenidoBodyApi CU_LeerContenidoBody { get; set; }
        public UsuarioController( IConfiguration cn, ILeerContenidoBodyApi cuLeerContenidoBody)
        {
            Conf = cn;
            URLBaseApiUsuarios = Conf.GetValue<string>("ApiUsuarios");
            URLBaseApiCabañas = Conf.GetValue<string>("ApiCabañas");
            CU_LeerContenidoBody=cuLeerContenidoBody;
        }


        //private string LeerContenido(HttpResponseMessage respuesta)
        //{
        //    HttpContent contenido = respuesta.Content;
        //    Task<string> tarea2 = contenido.ReadAsStringAsync();
        //    tarea2.Wait();
        //    string bodyContenido = tarea2.Result;
        //    return bodyContenido;
        //}

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
                HttpContext.Session.SetString("usuarioLogueadoMail", usu.Email.ToLower());
                HttpContext.Session.SetString("usuarioLogueadoRol", loginData.Rol.ToLower());
   
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
            usu.Rol = "usuario";
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

        [HttpPost]
        public IActionResult Logout(string n)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Usuario");
        }


        [HttpGet]
        public IActionResult ListarCabañasPropias()
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                if (HttpContext.Session.GetString("usuarioLogueadoRol") == "usuario")
                {
                    HttpClient client = new HttpClient();
                    string email = HttpContext.Session.GetString("usuarioLogueadoMail");
                    string emailConverted = "$" + email.Replace("@", "%40");
                    string url = URLBaseApiUsuarios + "Listadas/" + emailConverted;
                    var tarea = client.GetAsync(url);
                    tarea.Wait();
                    var respuesta = tarea.Result;
                    string contentBody = CU_LeerContenidoBody.LeerContenido(respuesta);
                    if (respuesta.IsSuccessStatusCode)
                    {
                        List<CabañaViewModel> listaCabañas = JsonConvert.DeserializeObject<List<CabañaViewModel>>(contentBody);
                        return View(listaCabañas);
                    }
                    else
                    {
                        ViewBag.Mensaje = contentBody;
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        [HttpGet]
        public IActionResult ListarMisAlquileresPropias()
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                if (HttpContext.Session.GetString("usuarioLogueadoRol") == "usuario")
                {
                    HttpClient client = new HttpClient();
                    string email = HttpContext.Session.GetString("usuarioLogueadoMail"); 
                    string emailConverted = email.Replace("@", "%40");
                    string url = URLBaseApiUsuarios + "alquileresUsuario/" + emailConverted;
                    var tarea = client.GetAsync(url);
                    tarea.Wait();
                    var respuesta = tarea.Result;
                    string contentBody = CU_LeerContenidoBody.LeerContenido(respuesta);
                    if (respuesta.IsSuccessStatusCode)
                    {
                        List<AlquilerCabañaViewModel> listaAlquileres = JsonConvert.DeserializeObject<List<AlquilerCabañaViewModel>>(contentBody);
                        return View(listaAlquileres);
                    }
                    else
                    {
                        ViewBag.Mensaje = contentBody;
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        

    }
 }

