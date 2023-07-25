using System.ComponentModel.DataAnnotations.Schema;
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
        public bool PoseeJacuzzi { get; set; }
        public bool EstaHabilitada { get; set; }
        public int CantidadPersonasMax { get; set; }

        public int IdTipoCabaña { get; set; }
        public TipoCabañaViewModel? TipoCabaña { get; set; }


    }


}

