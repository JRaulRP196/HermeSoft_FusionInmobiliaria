using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HermeSoft_Fusion.ViewModels
{
    public class BancoRegistroViewModel
    {
        [Required(ErrorMessage = "El nombre del banco es obligatorio.")]
        public string nombre { get; set; } = "";

        [Required(ErrorMessage = "El enlace web del banco es obligatorio.")]
        [Url(ErrorMessage = "El enlace debe ser una URL válida.")]
        public string enlace { get; set; } = "";

        [Required(ErrorMessage = "El plazo máximo de crédito es obligatorio.")]
        [Range(1, 100, ErrorMessage = "El plazo debe estar entre 1 y 100 años.")]
        public decimal maxCredito { get; set; }

        [Required(ErrorMessage = "La comisión bancaria es obligatoria.")]
        [Range(0, 100, ErrorMessage = "La comisión debe estar entre 0 y 100%.")]
        public decimal comision { get; set; }

        [Required(ErrorMessage = "Los honorarios de abogado son obligatorios.")]
        [Range(0, 100, ErrorMessage = "Los honorarios deben estar entre 0 y 100%.")]
        public decimal honorarioAbogado { get; set; }

        [Required(ErrorMessage = "El tipo de cambio es obligatorio.")]
        public string tipoCambio { get; set; } = "CRC";

        // Logo (archivo). Requiere columna logo si quieres persistirlo.
        public IFormFile? logoFile { get; set; }

        // Endeudamientos por tipo de asalariado
        public List<EndeudamientoInput> endeudamientos { get; set; } = new();

        // Seguros seleccionados
        [MinLength(1, ErrorMessage = "Debe seleccionar al menos un seguro.")]
        public List<int> segurosSeleccionados { get; set; } = new();

        // Escenarios de tasa
        [MinLength(1, ErrorMessage = "Debe registrar al menos un escenario de tasa.")]
        public List<EscenarioTasaInput> escenariosTasa { get; set; } = new();

        // Para pintar en la vista
        public List<SelectListItem> segurosDisponibles { get; set; } = new();
        public List<SelectListItem> tasasDisponibles { get; set; } = new();
    }

    public class EndeudamientoInput
    {
        public int idTipoAsalariado { get; set; }
        public string nombreTipo { get; set; } = "";
        [Range(0, 100, ErrorMessage = "El endeudamiento debe estar entre 0 y 100%.")]
        public decimal porcEndeudamiento { get; set; }
    }

    public class EscenarioTasaInput
    {
        [Required] public string nombre { get; set; } = "";
        [Range(0, 100)] public decimal porcAdicional { get; set; }
        [Range(0, 100)] public decimal porcDatoBancario { get; set; }
        [Range(1, 100)] public int plazo { get; set; }
        [Required] public int idTasaInteres { get; set; }
    }
}
//Hice la clase BancoRegistroViewModel porque el formulario combina datos de varias tablas (BANCOS, SEGUROS, TASAS, ENDEUDAMIENTOS),
//y el ViewModel permite validar y estructurar esos datos sin acoplar la vista al modelo de base de datos.