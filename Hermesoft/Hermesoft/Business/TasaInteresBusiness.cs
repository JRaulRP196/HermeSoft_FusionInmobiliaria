using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class TasaInteresBusiness
    {

        private TasaInteresRepository _repository;

        public TasaInteresBusiness(TasaInteresRepository repository)
        {
            _repository = repository;
        }

        #region Utilidades

        public async Task<IEnumerable<TasaInteres>> Obtener()
        {
            return await _repository.Obtener();
        }

        public async Task<TasaInteres> Obtener(int idTasaInteres)
        {
            return await _repository.Obtener(idTasaInteres);
        }

        #endregion

    }
}
