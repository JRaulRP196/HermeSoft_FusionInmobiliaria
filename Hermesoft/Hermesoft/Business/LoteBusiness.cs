using HermeSoft_Fusion.Models.Servicios;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class LoteBusiness
    {

        private LoteRepository _repository;
        private readonly VentaRepository _ventaBusiness;

        public LoteBusiness(LoteRepository loteRepository, VentaRepository ventaBusiness)
        {
            _repository = loteRepository;
            _ventaBusiness = ventaBusiness;
        }

        #region Utilidades

        public async Task<IEnumerable<Lote>> Obtener()
        {
            return await _repository.Obtener();
        }

        public async Task<Lote> Obtener(string codigoLote)
        {
            return await _repository.Obtener(codigoLote);
        }

        public async Task<IEnumerable<Lote>> ObtenerPorCondominio(string condominio)
        {
            return await _repository.ObtenerPorCondominio(condominio);
        }

        public async Task<IEnumerable<LoteDetalle>> ObtenerLotesMapa(int idMapa)
        {
            var lotes = await _repository.ObtenerLotesMapa(idMapa);
            List<LoteDetalle> lotesDetalles = new List<LoteDetalle>();
            foreach (var lote in lotes)
            {
                var venta = await _ventaBusiness.ObtenerPorLote(lote.Codigo);

                if(venta != null)
                {
                    var l = new LoteDetalle
                    {
                        Codigo = lote.Codigo,
                        Area = lote.Area,
                        PrecioVenta = lote.PrecioVenta,
                        FechaVenta = venta.FechaDeRegistro,
                        Condominio = lote.Condominio,
                        Estado = lote.Estado,
                        Fondo = lote.Fondo,
                        Frente = lote.Frente,
                        PrecioLista = lote.PrecioLista,
                        PrecioM2 = lote.PrecioM2,
                        X = lote.X,
                        Y = lote.Y,
                        Vendido = true
                    };
                    lotesDetalles.Add(l);
                }
                else
                {
                    var l = new LoteDetalle
                    {
                        Codigo = lote.Codigo,
                        Area = lote.Area,
                        PrecioVenta = lote.PrecioVenta,
                        FechaVenta = DateTime.MinValue,
                        Condominio = lote.Condominio,
                        Estado = lote.Estado,
                        Fondo = lote.Fondo,
                        Frente = lote.Frente,
                        PrecioLista = lote.PrecioLista,
                        PrecioM2 = lote.PrecioM2,
                        X = lote.X,
                        Y = lote.Y,
                        Vendido = false
                    };
                    lotesDetalles.Add(l);
                }
            }
            return lotesDetalles;
        }

        public async Task<Lote> Editar(Lote lote)
        {
            return await _repository.Editar(lote);
        }

        #endregion

    }
}
