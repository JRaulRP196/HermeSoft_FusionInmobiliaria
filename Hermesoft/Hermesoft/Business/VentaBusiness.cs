using HermeSoft_Fusion.Business.Usuarios;
using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;
using HermeSoft_Fusion.Repository.Servicios;
using QuestPDF.Fluent;

namespace HermeSoft_Fusion.Business
{
    public class VentaBusiness
    {

        private readonly VentaRepository _ventaRepository;
        private readonly AppDbContext _context;
        private readonly PrimaBusiness _primaBusiness;
        private readonly LoteBusiness _loteBusiness;
        private readonly EmailService _emailService;

        public VentaBusiness(VentaRepository ventaRepository, AppDbContext context, PrimaBusiness primaBusiness, LoteBusiness loteBusiness, EmailService emailService)
        {
            _ventaRepository = ventaRepository;
            _context = context;
            _primaBusiness = primaBusiness;
            _loteBusiness = loteBusiness;
            _emailService = emailService;
        }

        #region Utilidades

        public async Task<Venta> Agregar(Venta venta)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Console.WriteLine($"[Business] IdPrima que entra: {venta.IdPrima}");
                Console.WriteLine($"[Business] CorreoCliente que entra: {venta.CorreoCliente}");

                venta.FechaDeRegistro = DateTime.Now;
                venta.Estado = "EN PROCESO";

                if (!venta.IdPrima.HasValue)
                {
                    throw new Exception("Debe seleccionar una prima para registrar la venta");
                }

                var prima = await _primaBusiness.ObtenerPorId(venta.IdPrima.Value);
                if (prima == null)
                {
                    throw new Exception("La prima seleccionada no existe");
                }

                if (!string.Equals(prima.CorreoCliente, venta.CorreoCliente, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("La prima seleccionada no pertenece al cliente indicado");
                }

                if (prima.Asignado)
                {
                    throw new Exception("La prima seleccionada ya fue asignada a otra venta");
                }

                if (!string.Equals(prima.CodLote, venta.CodLote, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("La prima seleccionada no pertenece al lote indicado");
                }

                prima.Asignado = true;

                await _ventaRepository.Agregar(venta);

                var lote = await _loteBusiness.Obtener(venta.CodLote);
                lote.Estado = "En Venta";
                await _loteBusiness.Editar(lote);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return venta;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new Exception(e.Message);
            }
        }

        public async Task<byte[]> GenerarComprobanteVenta(int numContrato)
        {
            var venta = await _ventaRepository.Obtener(numContrato);
            var lote = await _loteBusiness.Obtener(venta.CodLote);
            var doc = new ComprobanteDocument
            {
                AreaLote = lote.Area,
                CorreoCliente = venta.CorreoCliente,
                NumeroLote = lote.Codigo,
                PrecioLote = lote.PrecioVenta,
                PrimaPagos = venta.Prima.DesglosesPrimas
            };
            byte[] pdf = doc.GeneratePdf();
            return pdf;
        }

        public async Task<bool> EnviarComprobante(string pdf, string correo)
        {
            var rutaFisica = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pdf.TrimStart('/'));
            byte[] archivo = System.IO.File.ReadAllBytes(rutaFisica);
            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "email", "Comprobante.html");
            string html = System.IO.File.ReadAllText(ruta);
            await _emailService.EnviarCorreoAsync(correo, "Comprobante de Venta", html, archivo, "Comprobante.pdf");
            return true;
        }

        public async Task<Venta> AnularVenta(int numContrato, string motivo)
        {
            using var trasaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var venta = await _ventaRepository.Obtener(numContrato);
                if (venta == null)
                    return null;
                var lote = await _loteBusiness.Obtener(venta.CodLote);
                lote.Estado = "Disponible";
                lote = await _loteBusiness.Editar(lote);
                if (lote == null)
                    return null;
                venta.Estado = "ANULADA";
                venta.MotivoNulidad = motivo;

                await _context.SaveChangesAsync();
                await trasaction.CommitAsync();
                return venta;
            }
            catch
            {
                throw new Exception("Ocurrio un error a la hora de editar los datos de la venta");
            }
        }

        public async Task<string> MotivoNulidad(int numContrato)
        {
            var venta = await _ventaRepository.Obtener(numContrato);
            if (venta == null)
            {
                return "Esta venta no se encuentra en el sistema";
            }
            if (venta.Estado != "ANULADA")
            {
                return "No se encuentra anulada esta venta";
            }
            return venta.MotivoNulidad;
        }

        public async Task<List<Venta>> Filtro(List<Venta> ventas, DateTime fechaInicio, DateTime fechaCierre, string condominio)
        {
            List<Venta> ventasFiltradas = new List<Venta>();
            if (condominio != null)
            {
                var loteCondominio = await _loteBusiness.ObtenerPorCondominio(condominio);
                ventasFiltradas = ventas.Where(v => loteCondominio.Any(l => l.Codigo == v.CodLote)).ToList();
            }

            if (fechaCierre != DateTime.MinValue && fechaInicio != DateTime.MinValue && ventasFiltradas.Any())
            {
                ventasFiltradas = ventasFiltradas.Where(v => v.FechaDeRegistro >= fechaInicio && v.FechaDeRegistro <= fechaCierre).ToList();
            }else if (fechaCierre != DateTime.MinValue && fechaInicio != DateTime.MinValue)
            {
                ventasFiltradas = ventas.Where(v => v.FechaDeRegistro >= fechaInicio && v.FechaDeRegistro <= fechaCierre).ToList();
            }
            return ventasFiltradas;
        }

        public async Task<List<Venta>> Obtener()
        {
            return await _ventaRepository.Obtener();
        }

        public async Task<Venta> Obtener(int numContrato)
        {
            return await _ventaRepository.Obtener(numContrato);
        }

        #endregion

    }
}
