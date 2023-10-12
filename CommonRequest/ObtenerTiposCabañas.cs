using MVC.CommonRequest.Interfaces;
using MVC.Models.ViewModels;
using Newtonsoft.Json;

namespace MVC.CommonRequest
{
    public class ObtenerTiposCabañas:IObtenerTiposCabañas
    {
        string URLBaseApiTiposCabañas { get; set; }
        public ILeerContenidoBodyApi CU_LeerContenido { get; set; }
        public ObtenerTiposCabañas( ILeerContenidoBodyApi leerContenido, IConfiguration conf)
        {
            URLBaseApiTiposCabañas = conf.GetValue<string>("ApiTipoCabañas");
            CU_LeerContenido = leerContenido;
        }
        public List<TipoCabañaViewModel> ObtenerTiposCabañasApi()
        {
            HttpClient cliente = new HttpClient();
            string url = URLBaseApiTiposCabañas;
            Task<HttpResponseMessage> tarea1 = cliente.GetAsync(url);
            tarea1.Wait();
            HttpResponseMessage respuesta = tarea1.Result;
            if (respuesta.IsSuccessStatusCode)
            {
                string bodyContenido = CU_LeerContenido.LeerContenido(respuesta);
                return JsonConvert.DeserializeObject<List<TipoCabañaViewModel>>(bodyContenido);
            }
            else
            {
                return null;
            }
        }

    }
}
