using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class EscenarioTasaInteresBusiness
    {

        private EscenarioTasaInteresRepository _repository;
        private TasaInteresRepository _tasaInteresRepository;
        private PlazosEscenariosRepository _plazoEscenariosRepository;

        public EscenarioTasaInteresBusiness(EscenarioTasaInteresRepository repository, TasaInteresRepository tasaInteresRepository, PlazosEscenariosRepository plazosEscenariosRepository)
        {
            _repository = repository;
            _tasaInteresRepository = tasaInteresRepository;
            _plazoEscenariosRepository = plazosEscenariosRepository;
        }

        public async Task<EscenarioTasaInteres> Agregar(EscenarioTasaInteres escenario)
        {
            return await _repository.Agregar(escenario);
        }

        public async Task<IEnumerable<EscenarioTasaInteresResponse>> ObtenerPorBanco(int idBanco)
        {
            var listaEscenarios = await _repository.ObtenerPorBanco(idBanco);
            List<EscenarioTasaInteresResponse> escenarios = new List<EscenarioTasaInteresResponse>();

            foreach(EscenarioTasaInteres escenario in listaEscenarios)
            {
                var tasaInteres = await _tasaInteresRepository.Obtener(escenario.IdTasaInteres);
                var plazos = await _plazoEscenariosRepository.ObtenerPorEscenario(escenario.IdEscenario);
                EscenarioTasaInteresResponse e = new EscenarioTasaInteresResponse
                {
                    Nombre = escenario.Nombre,
                    IdEscenario = escenario.IdEscenario,
                    IdBanco = escenario.IdBanco,
                    IdTasaInteres = escenario.IdTasaInteres,
                    TasaInteres = tasaInteres,
                    PlazosEscenarios = plazos
                };
                escenarios.Add(e);
            }

            return escenarios;
        }

    }
}
