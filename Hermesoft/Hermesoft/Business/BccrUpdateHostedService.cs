using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HermeSoft_Fusion.Business
{
    public class BccrHostedService : BackgroundService
    {
        private readonly IServiceProvider _sp;

        public BccrHostedService(IServiceProvider sp)
        {
            _sp = sp;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _sp.CreateScope();

            
            var indicadores = scope.ServiceProvider
                .GetRequiredService<IndicadorBancarioBusiness>();

            await indicadores.ActualizarIndicadores();
        }
    }
}
