using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class CoordenadasBusiness
    {

        private CoordenadasRepository _repository;

        public CoordenadasBusiness(CoordenadasRepository repository)
        {
            _repository = repository;
        }

        #region Utilidades

        public async Task<Coordenadas> Agregar(string lote, string X, string Y, int idMapa)
        {
            return await _repository.Agregar(lote, X, Y, idMapa);
        }

        public async Task<IEnumerable<Coordenadas>> GetCoordenadasPorMapa(int id)
        {
            return await _repository.GetCoordenadasPorMapa(id);
        }

        #endregion

    }
}
