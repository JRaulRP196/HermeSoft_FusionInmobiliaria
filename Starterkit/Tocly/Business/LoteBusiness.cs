using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class LoteBusiness
    {

        private LoteRepository _repository;

        public LoteBusiness(LoteRepository loteRepository)
        {
            _repository = loteRepository;
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

        public async Task<IEnumerable<LoteMapa>> ObtenerLotesMapa(int idMapa)
        {
            return await _repository.ObtenerLotesMapa(idMapa);
        }

        #endregion

    }
}
