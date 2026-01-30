namespace HermeSoft_Fusion.Models
{
    public class Lote
    {
        public string Codigo { get; set; }
        public decimal Area { get; set; }
        public decimal Fondo { get; set; }
        public decimal Frente { get; set; }
        public decimal PrecioM2 { get; set; }
        public decimal PrecioLista { get; set; }
        public decimal PrecioVenta { get; set; }
        public string Condominio { get; set; }
        public string Estado { get; set; }
    }

    public class LoteMapa : Lote
    {
        public string X { get; set; }
        public string Y { get; set; }
    }

}
