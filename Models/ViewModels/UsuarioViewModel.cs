﻿namespace MVC.Models.ViewModels
{
    public class UsuarioViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Contraseña { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Rol { get; set; }


        public UsuarioViewModel()
        {
            
        }
    }
}
