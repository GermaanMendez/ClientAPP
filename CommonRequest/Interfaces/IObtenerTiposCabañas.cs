using MVC.Models.ViewModels;

namespace MVC.CommonRequest.Interfaces
{
    public interface IObtenerTiposCabañas
    {
        public List<TipoCabañaViewModel> ObtenerTiposCabañasApi();
    }
}
