namespace HermeSoft_Fusion.Repository.Servicios
{
    using HermeSoft_Fusion.Models;
    using HermeSoft_Fusion.Models.Banco;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;

    public class ComprobanteDocument : IDocument
    {
        public string CorreoCliente { get; set; }
        public decimal PrecioLote { get; set; }
        public decimal AreaLote { get; set; }
        public string NumeroLote { get; set; }

        public List<DesglosesPrimas> PrimaPagos { get; set; }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(40);

                page.Header().Element(Header);

                page.Content().Column(column =>
                {
                    column.Spacing(20);

                    column.Item().Element(DatosCliente);
                    column.Item().Element(DatosLote);

                    column.Item().Text("Plan de Pago - Prima")
                        .FontSize(16)
                        .Bold();

                    column.Item().Element(TablaPrima);
                });

                page.Footer()
                    .AlignCenter()
                    .Text("Gracias por su compra")
                    .FontSize(10);
            });
        }

        void Header(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem()
                    .Text("Sistema de Ventas de Lotes")
                    .FontSize(18)
                    .Bold();

                row.RelativeItem()
                    .AlignRight()
                    .Text($"Comprobante de Venta\n{DateTime.Now:dd/MM/yyyy}");
            });
        }

        void DatosCliente(IContainer container)
        {
            container.Border(1)
                .Padding(10)
                .Column(col =>
                {
                    col.Item().Text("Datos del Cliente")
                        .Bold()
                        .FontSize(14);

                    col.Item().Text($"Correo: {CorreoCliente}");
                });
        }

        void DatosLote(IContainer container)
        {
            container.Border(1)
                .Padding(10)
                .Column(col =>
                {
                    col.Item().Text("Datos del Lote")
                        .Bold()
                        .FontSize(14);

                    col.Item().Text($"Número de lote: {NumeroLote}");
                    col.Item().Text($"Área: {AreaLote} m²");
                    col.Item().Text($"Precio: ₡{PrecioLote:N2}");
                });
        }

        void TablaPrima(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Fecha de Pago").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Monto").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Estado").Bold();
                });

                foreach (var pago in PrimaPagos)
                {
                    table.Cell().Padding(5).Text(pago.FechaCobro.ToString("dd/MM/yyyy"));
                    table.Cell().Padding(5).Text($"₡{pago.Monto:N2}");
                    table.Cell().Padding(5).Text(pago.Estado);
                }
            });
        }
    }
}
