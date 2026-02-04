using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Business
{
    public class BancoBusiness
    {

        private BancoRepository _bancoRepository;

        public BancoBusiness(BancoRepository bancoRepository)
        {
            _bancoRepository = bancoRepository;
        }

        public async Task<Banco> Agregar(Banco banco, IFormFile LogoFile)
        {
            string carpetaLogos = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/logos");
            if (!Directory.Exists(carpetaLogos))
                Directory.CreateDirectory(carpetaLogos);

            string extension = Path.GetExtension(LogoFile.FileName);
            string nombreArchivo = Guid.NewGuid().ToString() + extension;
            string rutaFisica = Path.Combine(carpetaLogos, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                await LogoFile.CopyToAsync(stream);
            }

            string rutaBD = $"/uploads/logos/{nombreArchivo}";
            banco.Logo = rutaBD;
            return await _bancoRepository.Agregar(banco);
        }

        public async Task<IEnumerable<Banco>> ObtenerTodos()
        {
            return await _bancoRepository.ObtenerTodos();
        }

        public async Task<Banco?> ObtenerPorId(int id)
        {
            return await _bancoRepository.ObtenerPorId(id);
        }

    }
}
