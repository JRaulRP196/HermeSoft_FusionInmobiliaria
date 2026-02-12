using HermeSoft_Fusion.Models.Usuarios;
using Microsoft.AspNetCore.Identity;

namespace HermeSoft_Fusion.Business.Usuarios
{
    public class PasswordService
    {
        private PasswordHasher<Usuario> _hasher = new();

        public string HashPassword(Usuario user, string password)
        {
            return _hasher.HashPassword(user, password);
        }

        public bool VerifyPassword(Usuario user, string password, string hash)
        {
            var result = _hasher.VerifyHashedPassword(user, hash, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
