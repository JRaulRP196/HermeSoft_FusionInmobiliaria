namespace HermeSoft_Fusion.Models.ViewModels
{
    public class EscenarioTasaViewModel
    {

        public string Nombre { get; set; }
        public string TipoTasa { get; set; }

        public List<TramoTasaViewModel> Tramos { get; set; } = new();

    }
}
