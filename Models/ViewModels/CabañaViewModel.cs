
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace MVC.Models.ViewModels
{
    [JsonObject]
    public class CabañaViewModel
    {
        public int NumeroHabitacion { get; set; }

        public string Nombre { get; set; }
        public string Foto { get; set; }
        public string Descripcion { get; set; }
        [Display(Name = "Jacuzzi")]
        public bool PoseeJacuzzi { get; set; }
        [Display(Name = "Habilitada")]
        public bool EstaHabilitada { get; set; }
        [Display(Name = "Huespedes")]
        public int CantidadPersonasMax { get; set; }
        public int PrecioDiario { get; set; }
        public int IdTipoCabaña { get; set; }
        [Display(Name ="Tipo de Cabaña")]
        public TipoCabañaViewModel? TipoCabaña { get; set; }

        public int IdDueño { get; set; }    
        public UsuarioViewModel? Dueño { get; set; }    

    }


}

