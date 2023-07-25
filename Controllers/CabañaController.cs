
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using MVC.Models;
using MVC.Models.ViewModels;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MVC.Controllers
{
    public class CabañaController : Controller
    {
        public string URLBaseApiCabañas { get; set; }
        public string URLBaseApiTiposCabañas { get; set; }
        public string URLBaseApiMantenimientos { get; set; }
        public string URLBaseUsuarios { get; set; } 

        public IWebHostEnvironment WHE { get; set; }
        public CabañaController(IWebHostEnvironment wheParam, IConfiguration conf)
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
        private List<TipoCabañaViewModel> ObtenerTiposCabañas()
        {
            HttpClient cliente = new HttpClient();
            string url = URLBaseApiTiposCabañas;
            Task<HttpResponseMessage> tarea1 = cliente.GetAsync(url);
            tarea1.Wait();
            HttpResponseMessage respuesta = tarea1.Result;
            if (respuesta.IsSuccessStatusCode)
            {
            string bodyContenido = LeerContenido(respuesta);
                return JsonConvert.DeserializeObject<List<TipoCabañaViewModel>>(bodyContenido);
            }
            else
            {
                return null;
            }
        }

        // GET: CabañaController
        public ActionResult Index() //OK
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                HttpClient cliente = new HttpClient();
                string url = URLBaseApiCabañas;
                Task<HttpResponseMessage> tarea1 = cliente.GetAsync(url);
                tarea1.Wait();
                HttpResponseMessage respuesta = tarea1.Result;
                string bodyContenido = LeerContenido(respuesta);
                if (respuesta.IsSuccessStatusCode)
                {
                    List<CabañaViewModel> listaCabañas = JsonConvert.DeserializeObject<List<CabañaViewModel>>(bodyContenido);
                    return View(listaCabañas);
                }
                else
                {
                    ViewBag.Mensaje = bodyContenido;
                    return View(new List<CabañaViewModel>());
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        // GET: CabañaController/Details/5
        public ActionResult Details(int id) //OK
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                CabañaViewModel buscado = null;
                HttpClient cliente = new HttpClient();
                string url = URLBaseApiCabañas + id;
                Task<HttpResponseMessage> tarea1 = cliente.GetAsync(url);
                tarea1.Wait();

                string jsonBody = LeerContenido(tarea1.Result);

                if (tarea1.Result.IsSuccessStatusCode)
                {
                    buscado = JsonConvert.DeserializeObject<CabañaViewModel>(jsonBody);
                    return View(buscado);
                }
                else
                {
                    ViewBag.Mensaje = jsonBody;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        // GET: CabañaController/Create
        public ActionResult Create() //OK
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") == null || HttpContext.Session.GetString("token")==null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            else
            {
                List<TipoCabañaViewModel> tipos = ObtenerTiposCabañas();
                if (tipos!=null)
                {
                    AltaCabañaViewModel av= new AltaCabañaViewModel();
                    av.TiposCabañas = tipos;
                    return View(av);
                }
                else
                {
                    ViewBag.Mensaje = "Error";
                    return View();
                }
            }
        }

        // POST: CabañaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AltaCabañaViewModel vm) //OK
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null || HttpContext.Session.GetString("token") != null)
            {
                FileInfo fi = new FileInfo(vm.Foto.FileName);
                string Extension = fi.Extension;
                string nomArchivoFoto = vm.Cabaña.Nombre + Extension;
                if (nomArchivoFoto.Contains(" "))
                {
                    nomArchivoFoto = nomArchivoFoto.Replace(" ", "_");
                }
                string nom = nomArchivoFoto;
                vm.Cabaña.Foto = nomArchivoFoto;
                vm.Cabaña.IdTipoCabaña = vm.IdTipoCabaña;

                HttpClient cliente = new HttpClient();
                string url = URLBaseApiCabañas;
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

                var tarea = cliente.PostAsJsonAsync(url, vm.Cabaña);
                tarea.Wait();
                if (tarea.Result.IsSuccessStatusCode)
                {
                    string rutaWwwRoot = WHE.WebRootPath;
                    string rutaCarpeta = Path.Combine(rutaWwwRoot, "Imagenes");
                    string rutaArchivo = Path.Combine(rutaCarpeta, nomArchivoFoto);
                    FileStream fs = new FileStream(rutaArchivo, FileMode.Create);
                    vm.Foto.CopyTo(fs);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    List<TipoCabañaViewModel> tipos = ObtenerTiposCabañas();
                    ViewBag.Mensaje = LeerContenido(tarea.Result);
                    vm.TiposCabañas = tipos;
                    return View(vm);
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        // GET: CabañaController/Edit/5


        
        public ActionResult BuscarPorTextoNombre()
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
        public ActionResult BuscarPorTextoNombre(string texto)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                 if (texto != null)
                  {
                    HttpClient cliente = new HttpClient();
                    string url = URLBaseApiCabañas + "texto/" + texto;
                    Task<HttpResponseMessage> tarea1 = cliente.GetAsync(url);
                    tarea1.Wait();
                    string jsonBody = LeerContenido(tarea1.Result);

                    if (tarea1.Result.IsSuccessStatusCode)
                    {
                        List<CabañaViewModel> listaCabañas = JsonConvert.DeserializeObject<List<CabañaViewModel>>(jsonBody);
                        return View(listaCabañas);
                    }
                    else
                    {
                        ViewBag.Mensaje = jsonBody;
                        return View();
                    }
                }
                else
                {
                    ViewBag.Mensaje = "The text cannot be null";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        public IActionResult BuscarPorTipo()
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                var viewModel = new BuscarPorTipoViewModel();
                viewModel.TiposCabañas = ObtenerTiposCabañas();
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BuscarPorTipo(BuscarPorTipoViewModel viewModel)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                if (viewModel.IdTipoCabaña >0)
                {
                        HttpClient cliente = new HttpClient();
                        string url = URLBaseApiCabañas + "tipo/" + viewModel.IdTipoCabaña;
                        Task<HttpResponseMessage> tarea1 = cliente.GetAsync(url);
                        tarea1.Wait();
                        string jsonBody = LeerContenido(tarea1.Result);

                        if (tarea1.Result.IsSuccessStatusCode)
                        {
                            List<CabañaViewModel> listaCabañas = JsonConvert.DeserializeObject<List<CabañaViewModel>>(jsonBody);
                            ViewBag.Cabañas = listaCabañas;
                            viewModel.TiposCabañas = ObtenerTiposCabañas();
                            return View(viewModel);
                        }
                        else
                        {
                            ViewBag.Mensaje = jsonBody;
                            viewModel.TiposCabañas = ObtenerTiposCabañas();
                            return View(viewModel);
                        }
                }
                else
                {
                    ViewBag.Mensaje = "You must select a type to search";
                    viewModel.TiposCabañas = ObtenerTiposCabañas();
                    return View(viewModel);
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        public ActionResult BuscarPorCantPersonas()
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
        public ActionResult BuscarPorCantPersonas(int numero)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                if (numero > 0)
                {
                    HttpClient cliente = new HttpClient();
                        string url = URLBaseApiCabañas + "cantidadPersonas/" + numero;
                        Task<HttpResponseMessage> tarea1 = cliente.GetAsync(url);
                        tarea1.Wait();
                        string jsonBody = LeerContenido(tarea1.Result);
                        if (tarea1.Result.IsSuccessStatusCode)
                        {
                            List<CabañaViewModel> cabañas = JsonConvert.DeserializeObject<List<CabañaViewModel>>(jsonBody);
                            return View(cabañas);
                        }
                        else
                        {
                            ViewBag.Mensaje = jsonBody;
                            return View();
                        }
                }
                else
                {
                    ViewBag.Mensaje = "You must add an amount of type numeric and big than 0 ";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login","Usuario");
            }
        }


        public ActionResult ListarSoloHabilitadas()
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                HttpClient client = new HttpClient();
                string url = URLBaseApiCabañas + "Habilitadas";
                var tarea1 = client.GetAsync(url);
                tarea1.Wait();
                string json = LeerContenido(tarea1.Result);
                if (tarea1.Result.IsSuccessStatusCode)
                {
                    List<CabañaViewModel> listaCabañas = JsonConvert.DeserializeObject<List<CabañaViewModel>>(json);
                    return View(listaCabañas);
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


    public ActionResult ListarPorTipoYMonto()
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
        public IActionResult ListarPorTipoYMonto(int monto )
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                if (monto > 0)
                {
                    HttpClient client = new HttpClient();
                    string url = URLBaseApiCabañas + "Monto/" + monto; 
                    var tarea1 = client.GetAsync(url);
                    tarea1.Wait();
                    string json = LeerContenido(tarea1.Result);
                    if (tarea1.Result.IsSuccessStatusCode)
                    {
                        List<CabañaViewModel> cabañas = JsonConvert.DeserializeObject<List<CabañaViewModel>>(json);
                        return View(cabañas);
                    }
                    else
                    {
                        ViewBag.Mensaje = "We don't have cabins with that amount";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Mensaje = "You must add an amount bigger than 0";
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
