using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class AlquilerCabañaController : Controller
    {
        public string URLBaseApiCabañas { get; set; }
        public string URLBaseApiTiposCabañas { get; set; }
        public string URLBaseApiMantenimientos { get; set; }
        public string URLBaseUsuarios { get; set; }

        public IWebHostEnvironment WHE { get; set; }
        public AlquilerCabañaController(IWebHostEnvironment wheParam, IConfiguration conf)
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


        // GET: AlquilerCabañaController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AlquilerCabañaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AlquilerCabañaController/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: AlquilerCabañaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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
            return View();
        }

        // POST: AlquilerCabañaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
    }
}
