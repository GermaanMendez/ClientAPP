using MVC.CommonRequest.Interfaces;
using MVC.Models.ViewModels.Alquiler;
using Newtonsoft.Json;

namespace MVC.CommonRequest
{
    public class ObtenerAlquilerPorId:IObtenerAlquilerPorId
    {
        string URLBaseAlquileres { get;set; }
        public ILeerContenidoBodyApi CU_LeerContenido { get; set; }
        public ObtenerAlquilerPorId(ILeerContenidoBodyApi leerContenido, IConfiguration conf)
        {
            URLBaseAlquileres = conf.GetValue<string>("ApiAlquileres");
            CU_LeerContenido = leerContenido;
        }
        public AlquilerCabañaViewModel ObtenerAlquiLerPorIdApi(int id)
        {
            HttpClient client = new HttpClient();
            string url = URLBaseAlquileres + id;
            var tarea1 = client.GetAsync(url);
            tarea1.Wait();
            var respuesta = tarea1.Result;
            string json = CU_LeerContenido.LeerContenido(respuesta);
            if (respuesta.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<AlquilerCabañaViewModel>(json);
            }
            else
            {
                return null;
            }
        }
    }
}
