using HermeSoft_Fusion.Models.Usuarios;
using HermeSoft_Fusion.Repository.Usuarios;

namespace HermeSoft_Fusion.Business.Usuarios
{
    public class RolBusiness
    {

        private readonly RolRepository _rolRepository;

        public RolBusiness(RolRepository rolRepository)
        {
            _rolRepository = rolRepository;
        }

        public async Task<IEnumerable<Rol>> Obtener()
        {
            return await _rolRepository.Obtener();
        }

    }
}
