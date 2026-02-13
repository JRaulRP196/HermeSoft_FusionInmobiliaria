using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;

namespace HermeSoft_Fusion.Business.Usuarios
{
    public class TokenService
    {
        public string GenerarToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return WebEncoders.Base64UrlEncode(bytes);
        }

    }
}
