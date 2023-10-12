namespace MVC.Models.ViewModels.Cabaña
{
    public class AltaCabañaViewModel
    {
        public CabañaViewModel Cabaña { get; set; }

        public List<TipoCabañaViewModel> TiposCabañas { get; set; }
        public int IdTipoCabaña { get; set; }
        public int PrecioDiario { get; set; }

        public IFormFile Foto { get; set; }


    }
}
