namespace HermeSoft_Fusion.Models
{
    public class UsuarioViewModel
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Iniciales { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }
        public DateTime? UltimoAcceso { get; set; }

        // Nueva propiedad para usar las fotos del proyecto
        public string FotoUrl { get; set; }
    }
}
