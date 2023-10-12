using MVC.CommonRequest.Interfaces;
using MVC.Models.ViewModels.Usuario;
using Newtonsoft.Json;

namespace MVC.CommonRequest
{
    public class ObtenerUsuarioLogueado:IObtenerUsuarioLogueado
    {
        string URLBaseUsuarios { get; set; }
        public ILeerContenidoBodyApi CU_LeerContenido { get; set; }
        public ObtenerUsuarioLogueado(ILeerContenidoBodyApi leerContenido, IConfiguration conf)
        {
            URLBaseUsuarios = conf.GetValue<string>("ApiUsuarios");
            CU_LeerContenido = leerContenido;
        }
        public UsuarioViewModel ObtenerUsuarioLogueadoApi(string email)
        {
            HttpClient cliente = new HttpClient();
            string emailConverted = "$" + email.Replace("@", "%40");
            string url = URLBaseUsuarios + "Usuario/" + emailConverted;
            Task<HttpResponseMessage> tarea1 = cliente.GetAsync(url);
            tarea1.Wait();
            HttpResponseMessage respuesta = tarea1.Result;
            string bodyContenido = CU_LeerContenido.LeerContenido(respuesta);
            if (respuesta.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<UsuarioViewModel>(bodyContenido);
            }
            else
            {
                return null;
            }
        }
    }
}
