

using MVC.Models.ViewModels;

namespace MVC.Models
{
    public class AltaCabañaViewModel
    {
        public CabañaViewModel Cabaña { get; set; }  

        public List<TipoCabañaViewModel> TiposCabañas { get; set; }  
        public int IdTipoCabaña { get; set; }   

        public IFormFile Foto { get; set; }


    }
}
