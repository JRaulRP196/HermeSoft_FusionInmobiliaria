using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class SeguroBancoBusiness
    {

        private SeguroBancoRepository _repository;

        public SeguroBancoBusiness(SeguroBancoRepository repository)
        {
            _repository = repository;
        }

        public async Task<SeguroBanco> Agregar(decimal porcentaje, int idBanco, int idSeguro)
        {
            SeguroBanco seguroBanco = new SeguroBanco 
            { 
                PorcSeguro = porcentaje,
                IdBanco = idBanco,
                IdSeguro = idSeguro
            };
            return await _repository.Agregar(seguroBanco);
        }

        public async Task<IEnumerable<SeguroBanco>> ObtenerPorBanco(int idBanco)
        {
            return await _repository.ObtenerPorBanco(idBanco);
        }

    }
}
