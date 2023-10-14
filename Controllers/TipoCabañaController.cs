
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MVC.CommonRequest.Interfaces;
using MVC.Models.ViewModels;
using MVC.Models.ViewModels.Cabaña;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MVC.Controllers
{
    public class TipoCabañaController : Controller
    {
        public string URLBaseApiTiposCabañas { get; set; }
        public IObtenerTipoCabañaPorId CU_ObtenerTipoPorId { get; set; }
        public ILeerContenidoBodyApi CU_LeerContenido { get; set; }
        public TipoCabañaController(IConfiguration conf, IObtenerTipoCabañaPorId cuObtenerTipoId, ILeerContenidoBodyApi cU_LeerContenido)
        {
            URLBaseApiTiposCabañas = conf.GetValue<string>("ApiTipoCabañas");
            CU_ObtenerTipoPorId = cuObtenerTipoId;
            CU_LeerContenido = cU_LeerContenido;
        }
        public ActionResult Index() //OK
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                HttpClient cliente = new HttpClient();
                string url = URLBaseApiTiposCabañas;
                Task<HttpResponseMessage> tarea1 = cliente.GetAsync(url);
                tarea1.Wait();
                HttpResponseMessage respuesta = tarea1.Result;
                string bodyContenido = CU_LeerContenido.LeerContenido(respuesta);
                if (respuesta.IsSuccessStatusCode)
                {
                    ViewBag.RolUsuarioLogueado = HttpContext.Session.GetString("usuarioLogueadoRol").ToLower();
                    List<TipoCabañaViewModel> listaTiposCabañas = JsonConvert.DeserializeObject<List<TipoCabañaViewModel>>(bodyContenido);
                    return View(listaTiposCabañas);
                }
                else
                {
                    ViewBag.Mensaje = bodyContenido;
                    return View(new List<TipoCabañaViewModel>());
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

            

        // GET: TipoCabañaController/Details/5
        public ActionResult Details()
        {
                if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
                {
                    if (HttpContext.Session.GetString("usuarioLogueadoRol") == "administrador"){
                        return View();
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

        // GET: TipoCabañaController/Create
        public ActionResult Create()
        {
                if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
                {
                if (HttpContext.Session.GetString("usuarioLogueadoRol") == "administrador")
                {
                    return View();
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

        // POST: TipoCabañaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TipoCabañaViewModel vm)//TipoCabaña nuevo
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                if (vm != null)
                {
                    HttpClient cliente= new HttpClient();
                    string url = URLBaseApiTiposCabañas;
                                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                    var tarea1 = cliente.PostAsJsonAsync(url,vm);
                    tarea1.Wait();
                    string json = CU_LeerContenido.LeerContenido(tarea1.Result);
                    if (tarea1.Result.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewBag.Mensaje =json;
                        return View();
                    }

                }
                else
                {
                    ViewBag.Mensaje = "Cannot create a null cabin type";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        // GET: TipoCabañaController/Edit/5
        public ActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                TipoCabañaViewModel aEditar = CU_ObtenerTipoPorId.ObtenerTipoCabañaPorIdApi(id);
                if (aEditar!=null)
                { 
                    return View(aEditar);
                }
                else
                {
                    ViewBag.Mensaje = "Error";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        // POST: TipoCabañaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TipoCabañaViewModel tipoVM)//TipoCabaña aEditar
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                HttpClient cliente = new HttpClient();
                string email = HttpContext.Session.GetString("usuarioLogueadoMail").Replace("@", "%40");
                string url = URLBaseApiTiposCabañas + tipoVM.Id + "/" + email;
                                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
               
                var tarea1 = cliente.PutAsJsonAsync(url,tipoVM);
                tarea1.Wait();
                string json = CU_LeerContenido.LeerContenido(tarea1.Result);

                if (tarea1.Result.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Mensaje = json;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        // GET: TipoCabañaController/Delete/5
        public ActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                TipoCabañaViewModel aBorrar = CU_ObtenerTipoPorId.ObtenerTipoCabañaPorIdApi(id);
                if (aBorrar != null)
                {
                 return View(aBorrar);
                }
                else
                {
                    ViewBag.Mensaje = "Error";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        // POST: TipoCabañaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
                if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
                {
                    HttpClient cliente = new HttpClient();
                string email = HttpContext.Session.GetString("usuarioLogueadoMail").Replace("@", "%40");
                string url = URLBaseApiTiposCabañas + id+"/"+email;
                    cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

                    var tarea1 = cliente.DeleteAsync(url);
                    tarea1.Wait();
                    string json = CU_LeerContenido.LeerContenido(tarea1.Result);

                    if (tarea1.Result.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewBag.Mensaje = json;
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Usuario");
                }
        }

        public ActionResult BuscarPorNombreAdministrador()
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BuscarPorNombreAdministrador(string nombre)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                if (HttpContext.Session.GetString("usuarioLogueadoRol") =="administrador")
                {
                    if (!string.IsNullOrEmpty(nombre))
                    {
                        HttpClient client = new HttpClient();
                        string url = URLBaseApiTiposCabañas + "Nombre/" + nombre; 
                        var tarea1 = client.GetAsync(url);
                        string json = CU_LeerContenido.LeerContenido(tarea1.Result);
                        if (tarea1.Result.IsSuccessStatusCode)
                        {
                            TipoCabañaViewModel tipoCabaña = JsonConvert.DeserializeObject<TipoCabañaViewModel>(json);
                            return View(tipoCabaña);
                        }
                        else
                        {
                            ViewBag.Mensaje = json;
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.Mensaje = "You must add a name to search";
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
