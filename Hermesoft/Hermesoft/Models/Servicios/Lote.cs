namespace HermeSoft_Fusion.Models.Servicios
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

    public class LoteAsignar : Lote
    {
        public bool Asignado { get; set; }
        public int? IdCoordenada { get; set; }
    }

    public class LoteDetalle : LoteMapa
    {
        public DateTime FechaVenta { get; set; }
        public bool Vendido { get; set; }
    }

}
