using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class SeguroBusiness
    {

        private SeguroRepository _repository;

        public SeguroBusiness(SeguroRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Seguro>> Obtener()
        {
            return await _repository.Obtener();
        }

    }
}
