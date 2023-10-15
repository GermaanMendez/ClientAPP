using MVC.CommonRequest.Interfaces;

namespace MVC.CommonRequest
{
    public class LeerContenidoJsonBodyApi:ILeerContenidoBodyApi
    {
        public string LeerContenido(HttpResponseMessage respuesta)
        {
            HttpContent contenido = respuesta.Content;
            Task<string> tarea2 = contenido.ReadAsStringAsync();
            tarea2.Wait();
            string bodyContenido = tarea2.Result;
            return bodyContenido;
        }

        public string LeerContenido2(HttpResponseMessage respuesta)
        {
            HttpContent contenido = respuesta.Content;
            Task<string> tarea2 = contenido.ReadAsStringAsync();
            tarea2.Wait();
            string bodyContenido = tarea2.Result;
            return $"Error: {respuesta.ReasonPhrase}. Detalles: {bodyContenido}";
        }

    }
}
