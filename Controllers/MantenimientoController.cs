
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Models.ViewModels;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MVC.Controllers
{
    public class MantenimientoController : Controller
    {
        public string URLBaseApiCabañas { get; set; }
        public string URLBaseApiTiposCabañas { get; set; }
        public string URLBaseApiMantenimientos { get; set; }
        public string URLBaseUsuarios { get; set; }

        public IWebHostEnvironment WHE { get; set; }
        public MantenimientoController(IWebHostEnvironment wheParam, IConfiguration conf)
        {
            URLBaseApiCabañas = conf.GetValue<string>("ApiCabañas");
            URLBaseApiTiposCabañas = conf.GetValue<string>("ApiTipoCabañas");
            URLBaseApiMantenimientos = conf.GetValue<string>("ApiMantenimientos");
            URLBaseUsuarios = conf.GetValue<string>("ApiUsuarios");
            WHE = wheParam;
        }

        private string LeerContenido(HttpResponseMessage respuesta)
        {
            HttpContent contenido = respuesta.Content;
            Task<string> tarea2 = contenido.ReadAsStringAsync();
            tarea2.Wait();
            string bodyContenido = tarea2.Result;
            return bodyContenido;
        }

        public ActionResult Index()
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                HttpClient client = new HttpClient();
                string url = URLBaseApiMantenimientos;
                var tarea1 = client.GetAsync(url);
                tarea1.Wait();
                string json = LeerContenido(tarea1.Result);
                if (tarea1.Result.IsSuccessStatusCode)
                {
                    List<MantenimientoViewModel> mantenimientos = JsonConvert.DeserializeObject<List<MantenimientoViewModel>>(json);
                    return View(mantenimientos);
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


        // GET: MantenimientoController/Details/5
        public ActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                if (id <= 0)
                {
                    HttpClient client = new HttpClient();
                    string url = URLBaseApiMantenimientos + id;
                    var tarea1 = client.GetAsync(url);
                    tarea1.Wait();
                    string json = LeerContenido(tarea1.Result);
                    if (tarea1.Result.IsSuccessStatusCode)
                    {
                        MantenimientoViewModel mantenimiento = JsonConvert.DeserializeObject<MantenimientoViewModel>(json);
                        return View(mantenimiento);
                    }
                    else
                    {
                        ViewBag.Mensaje = json;
                        return View();
                    }
                }
                else
                {
                    ViewBag.Mensaje = "The id cannot be null";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }
        private CabañaViewModel ObtenerCabaña(int NumeroHabitacion)
        {
            HttpClient cliente = new HttpClient();
            string url = URLBaseApiCabañas + NumeroHabitacion;
            var tarea1 = cliente.GetAsync(url);
            tarea1.Wait();
            string json = LeerContenido(tarea1.Result);
            if (tarea1.Result.IsSuccessStatusCode)
            {
                CabañaViewModel buscada = JsonConvert.DeserializeObject<CabañaViewModel>(json);
                return buscada;
            }
            else
            {
                return null;
            }
        }
        public ActionResult Create(int NumeroHabitacion) 
            {
                if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
                {
                    CabañaViewModel aAgregar = ObtenerCabaña(NumeroHabitacion);
                    MantenimientoViewModel nuevo = new MantenimientoViewModel();
                    nuevo.IdCabaña = aAgregar.NumeroHabitacion;
                    return View(nuevo);
                }
                else
                {
                    return RedirectToAction("Login", "Usuario");
                }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MantenimientoViewModel mantenimientoNuevo)
        {
                if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
                {
                    HttpClient cliente = new HttpClient();
                    string url = URLBaseApiMantenimientos;
                    cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                    var tarea1 = cliente.PostAsJsonAsync(url, mantenimientoNuevo);
                    tarea1.Wait();
                    string json = LeerContenido(tarea1.Result);
                    if (tarea1.Result.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewBag.Mensaje = json;
                        return View(mantenimientoNuevo);
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Usuario");
                }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ListarPorCabaña(int NumeroHabitacion)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                if (NumeroHabitacion > 0)
                {
                    HttpClient client = new HttpClient();
                    string url = URLBaseApiMantenimientos+ "Cabaña/"+ NumeroHabitacion;
                    var tarea1 = client.GetAsync(url);
                    tarea1.Wait();
                    string json = LeerContenido(tarea1.Result);
                    if (tarea1.Result.IsSuccessStatusCode)
                    {
                        List<MantenimientoViewModel> mantenimiento = JsonConvert.DeserializeObject<List<MantenimientoViewModel>>(json);
                        return View(mantenimiento);
                    }
                    else
                    {
                        ViewBag.Mensaje = json;
                        return View();
                    }
                }
                else
                {
                    ViewBag.Mensaje = "The cabin number must be valid ";
                    return View();
                }
            }
            return RedirectToAction("Login", "Usuario");
        }
        
    

        public IActionResult ListarMantenimientosPorFecha(int NumeroHabitacion)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                ViewBag.num = NumeroHabitacion;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ListarMantenimientosPorFecha(int Id, DateTime fecha1, DateTime fecha2)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                if (Id > 0 && fecha1 != null && fecha2 != null & fecha1 < fecha2)
                {
                    HttpClient client = new HttpClient();
                    string url = URLBaseApiMantenimientos + Id + "+" + fecha1.ToString("yyyy-MM-ddTHH:mm:ss") + "+" + fecha2.ToString("yyyy-MM-ddTHH:mm:ss");
                    var tarea1 = client.GetAsync(url);
                    tarea1.Wait();
                    string json = LeerContenido(tarea1.Result);
                    if (tarea1.Result.IsSuccessStatusCode)
                    {
                        List<MantenimientoViewModel> mantenimiento = JsonConvert.DeserializeObject<List<MantenimientoViewModel>>(json);
                        ViewBag.num = Id;
                        return View(mantenimiento);
                    }
                    else
                    {
                        ViewBag.Mensaje = json;
                        ViewBag.num = Id;
                        return View();
                    }
                }
                else
                {
                    ViewBag.Mensaje = "The cabin number must be valid, dates cannot be null and date 1 must be less than the date 2";
                    ViewBag.num = Id;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

            public IActionResult ListarMantenimientosPorValores()
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
        public IActionResult ListarMantenimientosPorValores(double valor1, double valor2)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                if (valor1>0 && valor2>=0 && valor1< valor2)
                {
                    HttpClient client = new HttpClient();
                    string url = URLBaseApiMantenimientos + "valor1=" + valor1 +"&valor2="+valor2; 
                    var tarea1 = client.GetAsync(url);
                    tarea1.Wait();
                    string json = LeerContenido(tarea1.Result);
                    if (tarea1.Result.IsSuccessStatusCode)
                    {
                        List<MantenimientoViewModel> mantenimientos = JsonConvert.DeserializeObject<List<MantenimientoViewModel>>(json);
                        return View(mantenimientos);
                    }
                    else
                    {
                        ViewBag.Mensaje = json;
                        return View();
                    }
                }
                else
                {
                    ViewBag.Mensaje = "The values cannot be less than 0 and the value 1 must be less than the value 2";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }


    }

    }

