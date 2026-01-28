using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class CondominioBusiness
    {

        private CondominioRepository _condominioRepository;

        public CondominioBusiness(CondominioRepository condominioRepository)
        {
            _condominioRepository = condominioRepository;
        }

        #region Utilidades

        public async Task<IEnumerable<Condominio>> Obtener()
        {
            return await _condominioRepository.Obtener();
        }

        #endregion

    }
}
