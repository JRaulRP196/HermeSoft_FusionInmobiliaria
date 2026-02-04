using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class IndicadorBancarioBusiness
    {
        private readonly BccrService _bccr;
        private readonly IndicadorBancarioRepository _repo;
        private readonly IConfiguration _config;

        public IndicadorBancarioBusiness(BccrService bccr, IndicadorBancarioRepository repo, IConfiguration config)
        {
            _bccr = bccr;
            _repo = repo;
            _config = config;
        }

        public async Task ActualizarIndicadores()
        {
            var url = _config["BCCR:Url"];
            var usuario = _config["BCCR:Usuario"];
            var token = _config["BCCR:Token"];

            var tbpId = _config["BCCR:Indicadores:TBP"];
            var tprId = _config["BCCR:Indicadores:TPR"];
            var sofrId = _config["BCCR:Indicadores:SOFR"];

            var fecha = DateTime.Today;

            var tbp = await _bccr.ObtenerValorIndicador(url, usuario, token, tbpId);
            if (tbp.HasValue)
                await _repo.Upsert("TBP", tbp.Value, fecha);

            var tpr = await _bccr.ObtenerValorIndicador(url, usuario, token, tprId);
            if (tpr.HasValue)
                await _repo.Upsert("TPR", tpr.Value, fecha);

            var sofr = await _bccr.ObtenerValorIndicador(url, usuario, token, sofrId);
            if (sofr.HasValue)
                await _repo.Upsert("SOFR", sofr.Value, fecha);
        }
    }
}
