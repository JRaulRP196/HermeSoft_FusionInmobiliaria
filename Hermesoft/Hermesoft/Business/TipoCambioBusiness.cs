using MySql.Data.MySqlClient;

namespace HermeSoft_Fusion.Business
{
    public class TipoCambioBusiness
    {
        private readonly BccrService _bccr;
        private readonly IConfiguration _config;

        public TipoCambioBusiness(BccrService bccr, IConfiguration config)
        {
            _bccr = bccr;
            _config = config;
        }

        public async Task ActualizarTipoCambio()
        {
            var url = _config["BCCR:Url"];
            var user = _config["BCCR:Usuario"];
            var token = _config["BCCR:Token"];
            var indicadorTc = _config["BCCR:Indicadores:TC_USD"];
            var connStr = _config.GetConnectionString("MySqlConnection");

            var valor = await _bccr.ObtenerValorIndicador(url, user, token, indicadorTc);
            if (!valor.HasValue) return;

            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(
                "UPDATE bancos SET tipoCambio = @valor",
                conn);

            cmd.Parameters.AddWithValue("@valor", valor.Value.ToString());
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
