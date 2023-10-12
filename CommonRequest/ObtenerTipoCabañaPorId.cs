using MVC.CommonRequest.Interfaces;
using MVC.Models.ViewModels;
using Newtonsoft.Json;

namespace MVC.CommonRequest
{
    public class ObtenerTipoCabañaPorId:IObtenerTipoCabañaPorId
    {
        string URLBaseApiTiposCabañas { get; set; }
        public ILeerContenidoBodyApi CU_LeerContenido { get; set; }
        public ObtenerTipoCabañaPorId(ILeerContenidoBodyApi leerContenido, IConfiguration conf)
        {
            URLBaseApiTiposCabañas = conf.GetValue<string>("ApiTipoCabañas");
            CU_LeerContenido = leerContenido;
        }
        public TipoCabañaViewModel ObtenerTipoCabañaPorIdApi(int id)
        {
            HttpClient client = new HttpClient();
            string url = URLBaseApiTiposCabañas + id;
            var tarea1 = client.GetAsync(url);
            tarea1.Wait();
            var respuesta = tarea1.Result;
            string json = CU_LeerContenido.LeerContenido(respuesta);
            if (respuesta.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<TipoCabañaViewModel>(json);
            }
            else
            {
                return null;
            }
        }
    }
}
