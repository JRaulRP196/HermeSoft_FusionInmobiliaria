using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class EndeudamientoMaximoBusiness
    {

        private EndeudamientoMaximoRepository _repository;

        public EndeudamientoMaximoBusiness(EndeudamientoMaximoRepository repository)
        {
            _repository = repository;
        }

        public async Task<EndeudamientoMaximo> Agregar(decimal porcentaje, int idBanco, int idTipoAsalariado)
        {
            EndeudamientoMaximo endeudamiento = new EndeudamientoMaximo
            {
                PorcEndeudamiento = porcentaje,
                IdBanco = idBanco,
                IdTipoAsalariado = idTipoAsalariado
            };
            return await _repository.Agregar(endeudamiento);
        }

        public async Task<IEnumerable<EndeudamientoMaximo>> ObtenerPorBanco(int idBanco)
        {
            return await _repository.ObtenerPorBanco(idBanco);
        }

    }
}
