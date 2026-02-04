using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class PlazosEscenariosBusiness
    {

        private PlazosEscenariosRepository _repository;

        public PlazosEscenariosBusiness(PlazosEscenariosRepository repository)
        {
            _repository = repository;
        }


        public async Task<PlazosEscenarios> Agregar(PlazosEscenarios plazo)
        {
            return await _repository.Agregar(plazo);
        }

    }
}
