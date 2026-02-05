using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class EscenarioTasaInteresBusiness
    {

        private EscenarioTasaInteresRepository _repository;


        public EscenarioTasaInteresBusiness(EscenarioTasaInteresRepository repository)
        {
            _repository = repository;
        }

        public async Task<EscenarioTasaInteres> Agregar(EscenarioTasaInteres escenario)
        {
            return await _repository.Agregar(escenario);
        }

    }
}
