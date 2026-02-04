namespace HermeSoft_Fusion.Models.ViewModels
{
    public class BancoViewModel
    {

        public string Nombre { get; set; }
        public string Enlace { get; set; }

        public List<EscenarioTasaViewModel> Escenarios { get; set; } = new();

    }
}
