using MVC.Models.ViewModels.Usuario;

namespace MVC.CommonRequest.Interfaces
{
    public interface IObtenerUsuarioLogueado
    {

        UsuarioViewModel ObtenerUsuarioLogueadoApi(string email);
    }
}
