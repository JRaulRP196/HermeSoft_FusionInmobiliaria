using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class TipoAsalariadoBusiness
    {

        private TipoAsalariadoRepository _repository;

        public TipoAsalariadoBusiness(TipoAsalariadoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TipoAsalariado>> Obtener()
        {
            return await _repository.Obtener();
        }

    }
}
