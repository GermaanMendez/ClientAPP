using MVC.Models.ViewModels;
using Newtonsoft.Json;

namespace MVC.CommonRequest.Interfaces
{
    public interface IObtenerTipoCabañaPorId
    {
        TipoCabañaViewModel ObtenerTipoCabañaPorIdApi(int id);
    }
}
