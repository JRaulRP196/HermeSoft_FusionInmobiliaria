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
                page.Margin(35);
                page.PageColor("#F8F9FB");

                page.Header().Element(Header);

                page.Content().PaddingVertical(10).Column(column =>
                {
                    column.Spacing(20);


                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Element(DatosCliente);
                        row.Spacing(15);
                        row.RelativeItem().Element(DatosLote);
                    });


                    column.Item().Text("PLAN DE PAGO - PRIMA")
                        .FontSize(16)
                        .Bold()
                        .FontColor("#1F3A5F");

                    column.Item().Element(TablaPrima);


                    column.Item().AlignRight().Element(TotalPrima);
                });

                page.Footer().Element(Footer);
            });
        }

        void Header(IContainer container)
        {
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images/logos/logo_fusion_dark.png");

            var logoBytes = System.IO.File.Exists(logoPath)
                ? System.IO.File.ReadAllBytes(logoPath)
                : null;

            container.Column(header =>
            {
                header.Item().Row(row =>
                {
                    if (logoBytes != null)
                    {
                        row.ConstantItem(80).Image(logoBytes);
                    }

                    row.RelativeItem().AlignMiddle().Column(col =>
                    {
                        col.Item().Text("COMPROBANTE DE VENTA")
                            .FontSize(18)
                            .Bold()
                            .FontColor("#1F3A5F");

                        col.Item().Text("Sistema de Ventas de Lotes")
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken1);
                    });

                    row.ConstantItem(150).AlignRight().Column(col =>
                    {
                        col.Item().Text("Fecha")
                            .SemiBold().FontSize(10);

                        col.Item().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                            .Bold().FontSize(12);
                    });
                });

                header.Item().PaddingTop(5)
                    .LineHorizontal(2)
                    .LineColor("#1F3A5F");
            });
        }

        void DatosCliente(IContainer container)
        {
            container.Background("#FFFFFF")
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .CornerRadius(8)
                .Padding(15)
                .Column(col =>
                {
                    col.Item().Text("Cliente")
                        .Bold()
                        .FontSize(13)
                        .FontColor("#1F3A5F");

                    col.Item().LineHorizontal(1)
                        .LineColor(Colors.Grey.Lighten2);

                    col.Item().PaddingTop(5)
                        .Text($"Correo: {CorreoCliente}");
                });
        }

        void DatosLote(IContainer container)
        {
            container.Background("#FFFFFF")
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .CornerRadius(8)
                .Padding(15)
                .Column(col =>
                {
                    col.Item().Text("Lote")
                        .Bold()
                        .FontSize(13)
                        .FontColor("#1F3A5F");

                    col.Item().LineHorizontal(1)
                        .LineColor(Colors.Grey.Lighten2);

                    col.Item().PaddingTop(5).Text($"Número: {NumeroLote}");
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
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                });


                table.Header(header =>
                {
                    header.Cell().Background("#1F3A5F").Padding(8)
                        .Text("Fecha").FontColor(Colors.White).Bold();

                    header.Cell().Background("#1F3A5F").Padding(8)
                        .Text("Monto").FontColor(Colors.White).Bold();

                    header.Cell().Background("#1F3A5F").Padding(8)
                        .Text("Estado").FontColor(Colors.White).Bold();
                });

                int index = 0;

                foreach (var pago in PrimaPagos)
                {
                    var bgColor = index % 2 == 0 ? "#FFFFFF" : "#F5F7FA";

                    table.Cell().Background(bgColor).Padding(8)
                        .Text(pago.FechaCobro.ToString("dd/MM/yyyy"));

                    table.Cell().Background(bgColor).Padding(8)
                        .Text($"₡{pago.Monto:N2}");

                    table.Cell().Background(bgColor).Padding(8)
                        .Text(text =>
                        {
                            var color = pago.Estado.ToLower() switch
                            {
                                "pagado" => Colors.Green.Darken2,
                                "pendiente" => Colors.Orange.Darken2,
                                "atrasado" => Colors.Red.Darken2,
                                _ => Colors.Black
                            };

                            text.Span(pago.Estado).FontColor(color).Bold();
                        });

                    index++;
                }
            });
        }


        void TotalPrima(IContainer container)
        {
            var total = PrimaPagos.Sum(x => x.Monto);

            container.Background("#EEF2F7")
                .Padding(10)
                .CornerRadius(6)
                .Text($"Total Prima: ₡{total:N2}")
                .Bold()
                .FontSize(12);
        }

        void Footer(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Text("Fusion Inmobiliaria")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Darken1);

                row.ConstantItem(150).AlignRight().Text(text =>
                {
                    text.Span("Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            });
        }
    }
}