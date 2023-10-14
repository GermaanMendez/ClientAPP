using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.CommonRequest.Interfaces;
using MVC.Models;
using MVC.Models.ViewModels.Alquiler;
using MVC.Models.ViewModels.Cabaña;
using MVC.Models.ViewModels.Usuario;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MVC.Controllers
{
    public class AlquilerCabañaController : Controller
    {
        public string URLBaseApiCabañas { get; set; }
        public string URLBaseApiTiposCabañas { get; set; }
        public string URLBaseApiMantenimientos { get; set; }
        public string URLBaseUsuarios { get; set; }
        public string URLBaseAlquileres { get; set; }

        public IWebHostEnvironment WHE { get; set; }
        public IObtenerUsuarioLogueado CU_ObtenerUsuarioLogueado { get; set; }
        public IObtenerCabañaPorId CU_ObtenerCabañaPorId { get; set; }
        public IObtenerAlquilerPorId CU_ObtenerAlquilerPorId { get; set; }
        public ILeerContenidoBodyApi CU_LeerContenidoBody { get; set; }
        public AlquilerCabañaController(IWebHostEnvironment wheParam, IConfiguration conf, IObtenerUsuarioLogueado cuObtenerLogueado, IObtenerCabañaPorId cU_ObtenerCabañaPorId, IObtenerAlquilerPorId cU_ObtenerAlquilerPorId, ILeerContenidoBodyApi cU_LeerContenidoBody)
        {
            URLBaseApiCabañas = conf.GetValue<string>("ApiCabañas");
            URLBaseApiTiposCabañas = conf.GetValue<string>("ApiTipoCabañas");
            URLBaseApiMantenimientos = conf.GetValue<string>("ApiMantenimientos");
            URLBaseUsuarios = conf.GetValue<string>("ApiUsuarios");
            URLBaseAlquileres = conf.GetValue<string>("ApiAlquileres");
            WHE = wheParam;
            CU_ObtenerUsuarioLogueado = cuObtenerLogueado;
            CU_ObtenerCabañaPorId = cU_ObtenerCabañaPorId;
            CU_ObtenerAlquilerPorId = cU_ObtenerAlquilerPorId;
            CU_LeerContenidoBody = cU_LeerContenidoBody;
        }
      
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create(int id) //OK
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") == null || HttpContext.Session.GetString("token") == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            else if (HttpContext.Session.GetString("usuarioLogueadoRol").ToLower() != "usuario")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                CabañaViewModel seleccionada = CU_ObtenerCabañaPorId.ObtenerCabañaPorIdApi(id);
                if(seleccionada ==null)
                {
                    ViewBag.Mensaje = "Try Again";
                    return View();
                }
                AlquilerCabañaViewModel nuevoAlquiler = new AlquilerCabañaViewModel();
                nuevoAlquiler.Cabaña = seleccionada;
  
                return View(nuevoAlquiler);
            }
        }

        // POST: AlquilerCabañaController/Create
        [HttpPost]
        public ActionResult Create(AlquilerCabañaViewModel nuevoAlquiler)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null || HttpContext.Session.GetString("token") != null)
            {
                if(HttpContext.Session.GetString("usuarioLogueadoRol") == "usuario")
                {
                   UsuarioViewModel logueado = CU_ObtenerUsuarioLogueado.ObtenerUsuarioLogueadoApi(HttpContext.Session.GetString("usuarioLogueadoMail"));
                    if (logueado != null)
                    {
                        nuevoAlquiler.Usuario = logueado;
                        nuevoAlquiler.UsuarioId = logueado.Id;

                        HttpClient cliente = new HttpClient();
                        string url = URLBaseAlquileres;
                        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                        var tarea1 = cliente.PostAsJsonAsync(url, nuevoAlquiler);
                        tarea1.Wait();
                        string json = CU_LeerContenidoBody.LeerContenido(tarea1.Result);
                        if (tarea1.Result.IsSuccessStatusCode)
                        {
                            return RedirectToAction("ListarMisAlquileresPropias", "Usuario");
                        }
                        else
                        {
                            nuevoAlquiler.Cabaña = CU_ObtenerCabañaPorId.ObtenerCabañaPorIdApi(nuevoAlquiler.CabañaId);
                            ViewBag.Mensaje = json;
                            return View(nuevoAlquiler);
                        }
                    }
                    else
                    {
                        nuevoAlquiler.Cabaña = CU_ObtenerCabañaPorId.ObtenerCabañaPorIdApi(nuevoAlquiler.CabañaId);
                        ViewBag.Mensaje = "Try Again";
                        return View(nuevoAlquiler);
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

        // GET: AlquilerCabañaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AlquilerCabañaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

// GET: AlquilerCabañaController/Delete/5
public ActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null || HttpContext.Session.GetString("token") != null)
            {
                AlquilerCabañaViewModel alquiler = CU_ObtenerAlquilerPorId.ObtenerAlquiLerPorIdApi(id);
                if(alquiler != null)
                {
                    return View(alquiler);
                }
                ViewBag.Mensaje = "Error";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        // POST: AlquilerCabañaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(AlquilerCabañaViewModel alquiler)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null || HttpContext.Session.GetString("token") != null)
            {
                HttpClient client = new HttpClient();
                string email = HttpContext.Session.GetString("usuarioLogueadoMail");
                string url = URLBaseAlquileres + email + "/" + alquiler.Id;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                var tarea1 = client.DeleteAsync(url);
                tarea1.Wait();
                var respuesta = tarea1.Result;
                string json = CU_LeerContenidoBody.LeerContenido(respuesta);
                if (tarea1.Result.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "Deleted successfully";
                    return RedirectToAction("ListarMisAlquileresPropias", "Usuario");
                }
                else
                {
                    ViewBag.Mensaje = "Error: " + json;
                    return View(alquiler);
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }


    }
}
