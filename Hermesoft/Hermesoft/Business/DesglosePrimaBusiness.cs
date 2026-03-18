using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class DesglosePrimaBusiness
    {

        private readonly DesglosePrimaRepository _repository;

        public DesglosePrimaBusiness(DesglosePrimaRepository repository)
        {
            _repository = repository;
        }

        public async Task<DesglosesPrimas> Obtener(int idDesglose)
        {
            return await _repository.Obtener(idDesglose);
        }
    }
}
