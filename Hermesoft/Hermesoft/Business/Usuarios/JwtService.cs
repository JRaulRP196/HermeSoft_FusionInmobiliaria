using HermeSoft_Fusion.Models.Usuarios;
using HermeSoft_Fusion.Repository.Servicios;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HermeSoft_Fusion.Business.Usuarios
{
    public class JwtService
    {

        private readonly Configuracion _configuracion;

        public JwtService(Configuracion configuracion)
        {
            _configuracion = configuracion;
        }

        public string GeneradorToken(Usuario usuario)
        {
            var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                        new Claim(ClaimTypes.Email, usuario.Correo)
                    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracion.ObtenerValor("JWT")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                    (
                        claims: claims,
                        expires: DateTime.UtcNow.AddHours(2),
                        signingCredentials: creds
                    );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
