using MVC.CommonRequest.Interfaces;
using MVC.Models.ViewModels.Cabaña;
using Newtonsoft.Json;

namespace MVC.CommonRequest
{
    public class ObtenerCabañaPorId : IObtenerCabañaPorId
    {
        string URLBaseApiCabañas { get; set; }
        public ILeerContenidoBodyApi CU_LeerContenido { get; set; }
        public ObtenerCabañaPorId(ILeerContenidoBodyApi leerContenido, IConfiguration conf)
        {
            URLBaseApiCabañas = conf.GetValue<string>("ApiCabañas");
            CU_LeerContenido = leerContenido;
        }

        public CabañaViewModel ObtenerCabañaPorIdApi(int id)
        {
            HttpClient client = new HttpClient();
            string url = URLBaseApiCabañas + id;
            var tarea1 = client.GetAsync(url);
            tarea1.Wait();
            var respuesta = tarea1.Result;
            string json = CU_LeerContenido.LeerContenido(respuesta);
            if (respuesta.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<CabañaViewModel>(json);
            }
            else
            {
                return null;
            }
        }

    }
}
