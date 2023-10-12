using MVC.Models.ViewModels.Alquiler;
using Newtonsoft.Json;

namespace MVC.CommonRequest.Interfaces
{
    public interface IObtenerAlquilerPorId
    {
        AlquilerCabañaViewModel ObtenerAlquiLerPorIdApi(int id);
    }
}
