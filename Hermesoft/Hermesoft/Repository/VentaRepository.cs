using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Models.Usuarios;
using HermeSoft_Fusion.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository
{
    public class VentaRepository
    {

        private readonly AppDbContext _context;
        private readonly LoteRepository _loteRepository;

        public VentaRepository(AppDbContext context, LoteRepository loteRepository)
        {
            _context = context;
            _loteRepository = loteRepository;
        }

        #region Utilidades

        public async Task Agregar(Venta venta)
        {
            _context.VENTAS.Add(venta);
        }

        public async Task<List<Venta>> Obtener()
        {
            return await _context.VENTAS
                    .Include(v => v.Prima)
                        .ThenInclude(p => p.DesglosesPrimas)
                    .Include(v => v.Usuario)
                    .ToListAsync();
        }

        public async Task<List<Venta>> ObtenerPaginado(int pagina, int tamanio, Usuario usuario)
        {
            if ( usuario.Rol.Nombre != "Administrador")
            {
                return await _context.VENTAS
                    .AsNoTracking()
                    .Where(v => v.IdUsuario == usuario.IdUsuario)
                    .OrderByDescending(x => x.FechaDeRegistro)
                    .Skip((pagina - 1) * tamanio)
                    .Take(tamanio)
                    .ToListAsync();
            }

            return await _context.VENTAS
                .AsNoTracking()
                .OrderByDescending(x => x.FechaDeRegistro)
                .Skip((pagina - 1) * tamanio)
                .Take(tamanio)
                .ToListAsync();
        }

        public async Task<int> ObtenerTotalVentas(Usuario usuario)
        {
            if (usuario.Rol.Nombre != "Administrador")
            {
                return await _context.VENTAS
                    .AsNoTracking()
                    .Where(v => v.IdUsuario == usuario.IdUsuario)
                    .CountAsync();
            }

            return await _context.VENTAS.CountAsync();
        }

        public async Task<List<Venta>> ObtenerFiltradoPaginado(
                                    DateTime? fechaInicio,
                                    DateTime? fechaFin,
                                    string? condominio,
                                    int pagina,
                                    int tamanio,
                                    Usuario usuario)
        {
            IQueryable<Venta> consulta;
            if (usuario.Rol.Nombre != "Administrador")
            {
                consulta = _context.VENTAS
                    .AsNoTracking()
                    .Where(v => v.IdUsuario == usuario.IdUsuario);
            }
            else
            {
                consulta = _context.VENTAS.AsNoTracking();
            }

            if (!string.IsNullOrEmpty(condominio))
            {
                var codigos = (await _loteRepository.ObtenerPorCondominio(condominio))
                                .Select(l => l.Codigo)
                                .ToList();
                consulta = consulta.Where(v => codigos.Contains(v.CodLote));
            }

            if (fechaInicio.HasValue && fechaInicio.Value != DateTime.MinValue)
                consulta = consulta.Where(v => v.FechaDeRegistro >= fechaInicio);

            if (fechaFin.HasValue && fechaFin.Value != DateTime.MinValue)
                consulta = consulta.Where(v => v.FechaDeRegistro <= fechaFin);

            return await consulta
                .OrderByDescending(v => v.FechaDeRegistro)
                .Skip((pagina - 1) * tamanio)
                .Take(tamanio)
                .ToListAsync();
        }

        public async Task<int> ContarVentasFiltradas(
                                DateTime? fechaInicio,
                                DateTime? fechaFin,
                                string? condominio,
                                Usuario usuario)
        {
            IQueryable<Venta> consulta;
            if (usuario.Rol.Nombre != "Administrador")
            {
                consulta = _context.VENTAS
                    .AsNoTracking()
                    .Where(v => v.IdUsuario == usuario.IdUsuario);
            }
            else
            {
                consulta = _context.VENTAS.AsNoTracking();
            }

            if (!string.IsNullOrEmpty(condominio))
            {
                var codigos = (await _loteRepository.ObtenerPorCondominio(condominio))
                                .Select(l => l.Codigo)
                                .ToList();
                consulta = consulta.Where(v => codigos.Contains(v.CodLote));
            }

            if (fechaInicio.HasValue && fechaInicio.Value != DateTime.MinValue)
                consulta = consulta.Where(v => v.FechaDeRegistro >= fechaInicio);

            if (fechaFin.HasValue && fechaFin.Value != DateTime.MinValue)
                consulta = consulta.Where(v => v.FechaDeRegistro <= fechaFin);

            return await consulta.CountAsync();
        }

        public async Task<List<Venta>> ObtenerPendientes()
        {
            return await _context.VENTAS
                    .Include(v => v.Prima)
                        .ThenInclude(p => p.DesglosesPrimas)
                    .Include(v => v.Usuario)
                    .Where(v => v.Estado == "EN PROCESO")
                    .ToListAsync();
        }

        public async Task<Venta> Obtener(int numContrato)
        {
            return await _context.VENTAS
                    .Include(v => v.Prima)
                        .ThenInclude(p => p.DesglosesPrimas)
                    .Include(v => v.Usuario)
                    .FirstOrDefaultAsync(v => v.NumContrato == numContrato);
        }

        public async Task<Venta> ObtenerPorLote(string codLote)
        {
             return await _context.VENTAS
                    .Include(v => v.Prima)
                        .ThenInclude(p => p.DesglosesPrimas)
                    .Include(v => v.Usuario)
                    .FirstOrDefaultAsync(v => v.CodLote == codLote && v.Estado != "ANULADA");
        }

        public async Task<List<Venta>> ObtenerVentasPorUsuario(int idUsuario)
        {
            return await _context.VENTAS
                .Include(v => v.Prima)
                    .ThenInclude(p => p.DesglosesPrimas)
                .Where(v => v.IdUsuario == idUsuario)
                .ToListAsync();

        }

        public async Task<PagoCondominioViewModel> ObtenerPagosPorCondominio(string? condominio, DateTime? fechaInicio, DateTime? fechaFinal)
        {
            IQueryable<Venta> consulta = _context.VENTAS
                .AsNoTracking()
                .Include(v => v.Prima)
                    .ThenInclude(p => p.DesglosesPrimas)
                .Where(v => v.Estado == "EN PROCESO");

            if (!string.IsNullOrEmpty(condominio))
            {
                var codigos = (await _loteRepository.ObtenerPorCondominio(condominio))
                                .Select(l => l.Codigo)
                                .ToList();
                consulta = consulta.Where(v => codigos.Contains(v.CodLote));
            }

            if (fechaInicio.HasValue && fechaInicio.Value != DateTime.MinValue)
                consulta = consulta.Where(v => v.FechaDeRegistro >= fechaInicio);

            if (fechaFinal.HasValue && fechaFinal.Value != DateTime.MinValue)
                consulta = consulta.Where(v => v.FechaDeRegistro <= fechaFinal);

            PagoCondominioViewModel resultado = new PagoCondominioViewModel
            {
                Pendientes = await consulta.SelectMany(v => v.Prima.DesglosesPrimas.Where(dp => dp.Estado == "Pendiente")).CountAsync(),
                Pagados = await consulta.SelectMany(v => v.Prima.DesglosesPrimas.Where(dp => dp.Estado == "Terminado")).CountAsync(),
                Atrasados = await consulta.SelectMany(v => v.Prima.DesglosesPrimas.Where(dp => dp.Estado == "Pendiente" &&
                                                                        dp.FechaCobro < DateTime.Today)).CountAsync()
            };

            return resultado;
        }

        public async Task<List<Venta>> ObtenerVentasEnProcesoPaginado(int pagina, int tamanio, Usuario usuario)
        {
            if (usuario.Rol.Nombre != "Administrador")
            {
                return await _context.VENTAS
                    .Include(v => v.Prima)
                        .ThenInclude(p => p.DesglosesPrimas)
                    .Include(v => v.Usuario)
                    .AsNoTracking()
                    .Where(v => v.IdUsuario == usuario.IdUsuario && v.Estado == "EN PROCESO")
                    .OrderByDescending(x => x.FechaDeRegistro)
                    .Skip((pagina - 1) * tamanio)
                    .Take(tamanio)
                    .ToListAsync();
            }

            return await _context.VENTAS
                .Include(v => v.Prima)
                       .ThenInclude(p => p.DesglosesPrimas)
                .Include(v => v.Usuario)
                .AsNoTracking()
                .Where(v => v.Estado == "EN PROCESO")
                .OrderByDescending(x => x.FechaDeRegistro)
                .Skip((pagina - 1) * tamanio)
                .Take(tamanio)
                .ToListAsync();
        }

        #endregion

    }
}
