
using Microsoft.AspNetCore.Mvc;
using MVC.Models.ViewModels;
using MVC.Models.ViewModels.Cabaña;
using MVC.Models.ViewModels.Usuario;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using MVC.CommonRequest.Interfaces;

namespace MVC.Controllers
{
    public class CabañaController : Controller
    {
        public string URLBaseApiCabañas { get; set; }
        public string URLBaseApiTiposCabañas { get; set; }
        public string URLBaseApiMantenimientos { get; set; }
        public string URLBaseUsuarios { get; set; } 
        public string URLBaseAlquileres { get; set; }

        public IWebHostEnvironment WHE { get; set; }
        public ILeerContenidoBodyApi CU_LeerContenido { get; set; }
        public IObtenerTiposCabañas CU_ObtenerTipos { get; set; }
        public IObtenerUsuarioLogueado CU_ObtenerUsuarioLogueado { get; set; }
        public IObtenerCabañaPorId CU_ObtenerCabañaPorId { get; set; }
        public IObtenerTipoCabañaPorId CU_ObtenerTipoCabañaPorId { get; set; }
        public CabañaController(IWebHostEnvironment wheParam, IConfiguration conf, ILeerContenidoBodyApi cuLeer, IObtenerTiposCabañas cuObtenerTipos,IObtenerUsuarioLogueado cuObtenerUsuario,IObtenerCabañaPorId cuObtenerCabañaId,IObtenerTipoCabañaPorId cuObtenerTipoId)
        {
            URLBaseApiCabañas = conf.GetValue<string>("ApiCabañas");
            URLBaseApiTiposCabañas = conf.GetValue<string>("ApiTipoCabañas");
            URLBaseApiMantenimientos = conf.GetValue<string>("ApiMantenimientos");
            URLBaseUsuarios = conf.GetValue<string>("ApiUsuarios");
            URLBaseAlquileres = conf.GetValue<string>("ApiAlquileres");
            WHE = wheParam;
            CU_LeerContenido= cuLeer;
            CU_ObtenerTipos= cuObtenerTipos;
            CU_ObtenerUsuarioLogueado = cuObtenerUsuario;
            CU_ObtenerCabañaPorId = cuObtenerCabañaId;
            CU_ObtenerTipoCabañaPorId= cuObtenerTipoId;
        }
        
        public ActionResult Index() //OK
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                HttpClient cliente = new HttpClient();
                string url = URLBaseApiCabañas;
                Task<HttpResponseMessage> tarea1 = cliente.GetAsync(url);
                tarea1.Wait();
                HttpResponseMessage respuesta = tarea1.Result;
                string bodyContenido = CU_LeerContenido.LeerContenido(respuesta);
                if (respuesta.IsSuccessStatusCode)
                {
                    MostrarListaCabañasRolViewModel cabañasViewModel = new MostrarListaCabañasRolViewModel();
                    cabañasViewModel.rolUsuarioLogueado= HttpContext.Session.GetString("usuarioLogueadoRol");
                    List<CabañaViewModel> listaCabañas = JsonConvert.DeserializeObject<List<CabañaViewModel>>(bodyContenido);
                    cabañasViewModel.cabañas = listaCabañas;

                    return View(cabañasViewModel);
                }
                else
                {
                    MostrarListaCabañasRolViewModel cabañasViewModel = new MostrarListaCabañasRolViewModel();
                    cabañasViewModel.rolUsuarioLogueado = HttpContext.Session.GetString("usuarioLogueadoRol");
                    cabañasViewModel.cabañas = new List<CabañaViewModel>();
                    ViewBag.Mensaje = bodyContenido;
                    return View(cabañasViewModel);
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

                string jsonBody = CU_LeerContenido.LeerContenido(tarea1.Result);

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
        public ActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                CabañaViewModel aEditar = CU_ObtenerCabañaPorId.ObtenerCabañaPorIdApi(id);
                if (aEditar != null)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CabañaViewModel cabañaVM)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                string emailUsuarioLogueado = HttpContext.Session.GetString("usuarioLogueadoMail").ToLower();
                string emailConverted =  emailUsuarioLogueado.Replace("@", "%40");
                UsuarioViewModel usario = CU_ObtenerUsuarioLogueado.ObtenerUsuarioLogueadoApi(emailUsuarioLogueado);
                TipoCabañaViewModel tipo = CU_ObtenerTipoCabañaPorId.ObtenerTipoCabañaPorIdApi(cabañaVM.IdTipoCabaña);
                cabañaVM.Usuario = usario;
                cabañaVM.TipoCabaña= tipo;
                    HttpClient cliente = new HttpClient();
                    string urlBase = "http://localhost:5126/api/Cabaña/";
                    //string url = URLBaseApiCabañas +"edit/"+ emailConverted;
                    string url = urlBase + "edit/" + emailConverted;
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

                    var tarea1 = cliente.PutAsJsonAsync(url, cabañaVM);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int numeroHabitacion)
        {
       
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null || HttpContext.Session.GetString("token") != null)
            {
                HttpClient client = new HttpClient();
                string email = HttpContext.Session.GetString("usuarioLogueadoMail").ToLower().Replace("@","%40");
                string url = URLBaseApiCabañas+"delete" +"/"+ email + "/" +numeroHabitacion;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                var tarea1 = client.DeleteAsync(url);
                tarea1.Wait();
                var respuesta = tarea1.Result;
                string json = CU_LeerContenido.LeerContenido(respuesta);
                if (tarea1.Result.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "Deleted";
                    return RedirectToAction("ListarCabañasPropias", "Usuario");
                }
                else
                {
                    ViewBag.Mensaje = "Error: " + json;
                    return RedirectToAction("Details", "Cabaña", new {id=numeroHabitacion});
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        public ActionResult Create() //OK
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") == null || HttpContext.Session.GetString("token") == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            else if (HttpContext.Session.GetString("usuarioLogueadoRol") != "usuario")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                List<TipoCabañaViewModel> tipos = CU_ObtenerTipos.ObtenerTiposCabañasApi();
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
                if (HttpContext.Session.GetString("usuarioLogueadoRol") == "usuario") {
                    if (vm.Foto == null)
                    {
                        PrepararViewModelYMensajeError(vm, "The Photo cannot be null");
                        return View(vm);
                    }
                    FileInfo fi = new FileInfo(vm.Foto.FileName);
                    string Extension = fi.Extension;
                    if(Extension.ToLower()!=".png" && Extension.ToLower() != ".jpg" && Extension.ToLower() != ".jpeg")
                    {
                        PrepararViewModelYMensajeError(vm, "The photo can only be a .png or .jpg file type");
                        return View(vm);
                    }
                        string nomArchivoFoto = vm.Cabaña.Nombre + Extension;
                    if (nomArchivoFoto.Contains(" "))
                    {
                        nomArchivoFoto = nomArchivoFoto.Replace(" ", "_");
                    }
                    string nom = nomArchivoFoto;
                    vm.Cabaña.Foto = nomArchivoFoto;
                    vm.Cabaña.IdTipoCabaña = vm.IdTipoCabaña;
                    vm.Cabaña.Usuario = CU_ObtenerUsuarioLogueado.ObtenerUsuarioLogueadoApi(HttpContext.Session.GetString("usuarioLogueadoMail"));
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
                        fi = null;
                        List<TipoCabañaViewModel> tipos = CU_ObtenerTipos.ObtenerTiposCabañasApi();
                        ViewBag.Mensaje = CU_LeerContenido.LeerContenido2(tarea.Result);
                        vm.TiposCabañas = tipos;
                        return View(vm);
                    }
                } else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        private void PrepararViewModelYMensajeError(AltaCabañaViewModel vm, string mensaje)
        {
            List<TipoCabañaViewModel> tipos = CU_ObtenerTipos.ObtenerTiposCabañasApi();
            ViewBag.Mensaje = mensaje;
            vm.TiposCabañas = tipos;
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
                    string jsonBody = CU_LeerContenido.LeerContenido(tarea1.Result);

                    if (tarea1.Result.IsSuccessStatusCode)
                    {
                        MostrarListaCabañasRolViewModel cabañasViewModel = new MostrarListaCabañasRolViewModel();
                        cabañasViewModel.rolUsuarioLogueado = HttpContext.Session.GetString("usuarioLogueadoRol");
                        List<CabañaViewModel> listaCabañas = JsonConvert.DeserializeObject<List<CabañaViewModel>>(jsonBody);
                        cabañasViewModel.cabañas = listaCabañas;
                        return View(cabañasViewModel);
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
                viewModel.RolUsuarioLogueado = HttpContext.Session.GetString("usuarioLogueadoRol");
                viewModel.TiposCabañas = CU_ObtenerTipos.ObtenerTiposCabañasApi();
                if (viewModel.TiposCabañas == null)
                {
                    ViewBag.Mesaje = "There is not any type of cabin in the system";
                    return RedirectToAction("Index", "Home");
                }
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
                        string jsonBody = CU_LeerContenido.LeerContenido(tarea1.Result);

                        if (tarea1.Result.IsSuccessStatusCode)
                        {
                            List<CabañaViewModel> listaCabañas = JsonConvert.DeserializeObject<List<CabañaViewModel>>(jsonBody);
                            ViewBag.Cabañas = listaCabañas;
                            viewModel.RolUsuarioLogueado = HttpContext.Session.GetString("usuarioLogueadoRol");
                            viewModel.TiposCabañas = CU_ObtenerTipos.ObtenerTiposCabañasApi();
                        return View(viewModel);
                        }
                        else
                        {
                            ViewBag.Mensaje = jsonBody;
                            viewModel.TiposCabañas = CU_ObtenerTipos.ObtenerTiposCabañasApi();
                        return View(viewModel);
                        }
                }
                else
                {
                    ViewBag.Mensaje = "You must select a type to search";
                    viewModel.TiposCabañas = CU_ObtenerTipos.ObtenerTiposCabañasApi();
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
                        string jsonBody = CU_LeerContenido.LeerContenido(tarea1.Result);
                        if (tarea1.Result.IsSuccessStatusCode)
                        {
                            MostrarListaCabañasRolViewModel cabañasViewModel = new MostrarListaCabañasRolViewModel();
                            cabañasViewModel.rolUsuarioLogueado = HttpContext.Session.GetString("usuarioLogueadoRol");
                            List<CabañaViewModel> listaCabañas = JsonConvert.DeserializeObject<List<CabañaViewModel>>(jsonBody);
                            cabañasViewModel.cabañas = listaCabañas;
                            return View(cabañasViewModel);
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


        public ActionResult ListarNOHabilitadas()
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                HttpClient client = new HttpClient();
                string url = URLBaseApiCabañas + "NOHabilitadas";
                var tarea1 = client.GetAsync(url);
                tarea1.Wait();
                string json = CU_LeerContenido.LeerContenido(tarea1.Result);
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


        public ActionResult ListarPorMonto()
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
        public IActionResult ListarPorMonto(int monto )
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                if (monto > 0)
                {
                    HttpClient client = new HttpClient();
                    string url = URLBaseApiCabañas + "Monto/" + monto; 
                    var tarea1 = client.GetAsync(url);
                    tarea1.Wait();
                    string json = CU_LeerContenido.LeerContenido(tarea1.Result);
                    if (tarea1.Result.IsSuccessStatusCode)
                    {
                        MostrarListaCabañasRolViewModel cabañasViewModel = new MostrarListaCabañasRolViewModel();
                        cabañasViewModel.rolUsuarioLogueado = HttpContext.Session.GetString("usuarioLogueadoRol");
                        List<CabañaViewModel> listaCabañas = JsonConvert.DeserializeObject<List<CabañaViewModel>>(json);
                        cabañasViewModel.cabañas = listaCabañas;
                        return View(cabañasViewModel);
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

        public ActionResult ListarDisponiblesEnRangoFechas()
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
        //[ValidateAntiForgeryToken] 
        [HttpPost]
        public ActionResult ListarDisponiblesEnRangoFechas(DateTime fDesde, DateTime fHasta)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                if (fDesde != null && fHasta != null && fDesde <= fHasta)
                {
                    HttpClient client = new HttpClient();
                    string url = URLBaseApiCabañas + "desde/" + fDesde.ToString("yyyy-MM-dd") + "/hasta" + fHasta.ToString("yyyy-MM-dd");
                    var tarea1 = client.GetAsync(url);
                    tarea1.Wait(); 
                    string json = CU_LeerContenido.LeerContenido(tarea1.Result);
                    if (tarea1.Result.IsSuccessStatusCode)
                    {
                        MostrarListaCabañasRolViewModel cabañasViewModel = new MostrarListaCabañasRolViewModel();
                        cabañasViewModel.rolUsuarioLogueado = HttpContext.Session.GetString("usuarioLogueadoRol");
                        List<CabañaViewModel> listaCabañas = JsonConvert.DeserializeObject<List<CabañaViewModel>>(json);
                        cabañasViewModel.cabañas = listaCabañas;
                        return View(cabañasViewModel);
                    }
                    else
                    {
                        ViewBag.Mensaje = json;
                        return View();
                    }
                }
                else
                {
                    ViewBag.Mensaje = "From Date and Until Date cannot be null and From Date must be less than Until Date";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        //[ValidateAntiForgeryToken] 
        [HttpPost]
        public ActionResult DeshabilitarCabaña (int idCabaña)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                    HttpClient client = new HttpClient();
                    string email = HttpContext.Session.GetString("usuarioLogueadoMail").Replace("@", "%40").ToLower();
                    string url = URLBaseApiCabañas + "deshabilitar/" + email + "/" + idCabaña;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                var tarea1 = client.PostAsync(url,null);
                    tarea1.Wait();
                    string json = CU_LeerContenido.LeerContenido(tarea1.Result);
                    if (tarea1.Result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Cabaña");
                    }
                    else
                    {
                        ViewBag.Mensaje = json;
                        return RedirectToAction("Index", "Cabaña");
                    }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        [HttpPost]
        public ActionResult HabilitarCabaña(int idCabaña)
        {
            if (HttpContext.Session.GetString("usuarioLogueadoMail") != null)
            {
                HttpClient client = new HttpClient();
                string email = HttpContext.Session.GetString("usuarioLogueadoMail").Replace("@", "%40").ToLower();
                string url = URLBaseApiCabañas + "habilitar/" + email + "/" + idCabaña;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                var tarea1 = client.PostAsync(url,null);
                tarea1.Wait();
                string json = CU_LeerContenido.LeerContenido(tarea1.Result);
                if (tarea1.Result.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListarNOHabilitadas", "Cabaña");
                }
                else
                {
                    ViewBag.Mensaje = json;
                    return RedirectToAction("ListarNOHabilitadas", "Cabaña");
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

    }
}
