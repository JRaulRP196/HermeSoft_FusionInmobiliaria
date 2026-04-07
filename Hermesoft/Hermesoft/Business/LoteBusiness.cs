using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Models.Servicios;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class LoteBusiness
    {

        private LoteRepository _repository;
        private readonly VentaRepository _ventaBusiness;
        private readonly CoordenadasRepository _coordenadasRepository;

        public LoteBusiness(LoteRepository loteRepository, VentaRepository ventaBusiness, CoordenadasRepository coordenadasRepository)
        {
            _repository = loteRepository;
            _ventaBusiness = ventaBusiness;
            _coordenadasRepository = coordenadasRepository;
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

        public async Task<IEnumerable<LoteAsignar>> ObtenerPorCondominio(string condominio)
        {
            var lotes = await _repository.ObtenerPorCondominio(condominio);
            List<LoteAsignar> lotesAsignar = new List<LoteAsignar>();
            foreach (var lote in lotes)
            {
                var coordenada = await _coordenadasRepository.GetCoordenada(lote.Codigo);
                var l = new LoteAsignar
                {
                    Codigo = lote.Codigo,
                    Area = lote.Area,
                    PrecioVenta = lote.PrecioVenta,
                    Condominio = lote.Condominio,
                    Estado = lote.Estado,
                    Fondo = lote.Fondo,
                    Frente = lote.Frente,
                    PrecioLista = lote.PrecioLista,
                    PrecioM2 = lote.PrecioM2,
                    Asignado = coordenada != null,
                    IdCoordenada = coordenada != null ? coordenada.IdCoordenada : (int?)null

                };
                lotesAsignar.Add(l);
            }
            return lotesAsignar;
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
