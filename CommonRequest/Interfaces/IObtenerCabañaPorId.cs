using MVC.Models.ViewModels.Cabaña;

namespace MVC.CommonRequest.Interfaces
{
    public interface IObtenerCabañaPorId
    {
        CabañaViewModel ObtenerCabañaPorIdApi(int id);
    }
}
