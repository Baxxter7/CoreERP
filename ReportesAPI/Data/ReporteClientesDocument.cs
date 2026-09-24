using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportesAPI.Models;

namespace ReportesAPI.Data
{
    public class ReporteClientesDocument : IDocument
    {
        private readonly List<ClienteDto> _clientes;

        public ReporteClientesDocument(List<ClienteDto> clientes)
        {
            _clientes = clientes;
        }

        public void Compose(IDocumentContainer container)
        {
            var cultura = new System.Globalization.CultureInfo("es-HN");

            var zonaCentroAmerica = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
            var fechaActual = TimeZoneInfo.ConvertTime(DateTime.Now, zonaCentroAmerica);

            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);

                page.Header()
                    .Background(Colors.Blue.Lighten5)
                    .Padding(20)
                    .Column(header =>
                    {
                        header.Spacing(5);

                        header.Item().Row(row =>
                        {
                            row.ConstantItem(200).Column(col =>
                            {
                                col.Item()
                                    .Text("CoreERP")
                                    .FontSize(22)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken2);

                                col.Item()
                                    .Text("Soluciones Josue")
                                    .FontSize(11)
                                    .FontColor(Colors.Grey.Darken1);
                            });

                            row.RelativeItem()
                                .AlignRight()
                                .Column(col =>
                                {
                                    col.Item()
                                        .Text($"Reporte de Clientes")
                                        .FontSize(24)
                                        .Bold()
                                        .FontColor(Colors.Blue.Darken3);

                                    col.Item()
                                        .Text($"Generado el: {fechaActual.ToString("dddd, dd MMMM yyyy", cultura)}")
                                        .FontSize(10)
                                        .FontColor(Colors.Grey.Darken1);
                                });
                        });

                        header.Item()
                            .PaddingTop(5)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten2);
                    });

                page.Content()
                    .PaddingVertical(20)
                    .Column(content =>
                    {
                        content.Spacing(15);

                        content.Item().Row(row =>
                        {
                            row.RelativeItem()
                                .Text($"Total de Clientes: {_clientes.Count}")
                                .FontSize(12)
                                .Bold()
                                .FontColor(Colors.Grey.Darken3);
                        });

                        content.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50); // ID
                                columns.RelativeColumn(3);  // Nombre
                                columns.RelativeColumn(2);  // Telefono
                                columns.RelativeColumn(4);  // Direccion
                            });

                            table.Header(header =>
                            {
                                string[] headers =
                                {
                                    "ID",
                                    "Nombre",
                                    "Telefono",
                                    "Direccion"
                                };

                                foreach (var headerText in headers)
                                {
                                    header
                                        .Cell()
                                        .Background(Colors.Blue.Darken2)
                                        .Padding(8)
                                        .Text(headerText)
                                        .FontColor(Colors.White)
                                        .Bold()
                                        .FontSize(10)
                                        .AlignCenter();
                                }
                            });

                            bool alt = false;

                            foreach (var cliente in _clientes)
                            {
                                var bg = alt
                                    ? Colors.Grey.Lighten5
                                    : Colors.White;

                                alt = !alt;

                                table.Cell()
                                    .Background(bg)
                                    .Border(0.5f)
                                    .BorderColor(Colors.Grey.Lighten3)
                                    .Padding(6)
                                    .AlignCenter()
                                    .Text(cliente.Id.ToString())
                                    .FontSize(9);

                                table.Cell()
                                    .Background(bg)
                                    .Border(0.5f)
                                    .BorderColor(Colors.Grey.Lighten3)
                                    .Padding(6)
                                    .AlignCenter()
                                    .Text(cliente.Nombre)
                                    .FontSize(9);

                                table.Cell()
                                    .Background(bg)
                                    .Border(0.5f)
                                    .BorderColor(Colors.Grey.Lighten3)
                                    .Padding(6)
                                    .AlignCenter()
                                    .Text(cliente.Telefono)
                                    .FontSize(9);

                                table.Cell()
                                    .Background(bg)
                                    .Border(0.5f)
                                    .BorderColor(Colors.Grey.Lighten3)
                                    .Padding(6)
                                    .AlignCenter()
                                    .Text(cliente.Direccion)
                                    .FontSize(9);
                            }
                        });
                    });

                page.Footer()
                    .PaddingTop(10)
                    .Row(row =>
                    {
                        row.RelativeItem()
                            .AlignLeft()
                            .Text("CoreERP | Josue")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Darken1);

                        row.RelativeItem()
                            .AlignRight()
                            .Text(text =>
                            {
                                text.Span("Página ").FontSize(8).FontColor(Colors.Grey.Darken1);
                                text.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Darken1);
                                text.Span(" de ").FontSize(8).FontColor(Colors.Grey.Darken1);
                                text.TotalPages().FontSize(8).FontColor(Colors.Grey.Darken1);
                            });
                    });
            });
        }
    }
}
