namespace MVC.Models.ViewModels.Cabaña
{
    public class BuscarPorTipoViewModel
    {
        public List<TipoCabañaViewModel> TiposCabañas { get; set; }
        public int IdTipoCabaña { get; set; }
        public string RolUsuarioLogueado { get; set; }
    }

}
