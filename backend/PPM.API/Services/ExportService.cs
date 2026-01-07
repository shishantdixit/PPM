using System.Text;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PPM.API.Services;

public interface IExportService
{
    byte[] GeneratePdf(string title, string[] headers, List<string[]> rows, Dictionary<string, string>? summary = null);
    byte[] GenerateExcel(string sheetName, string[] headers, List<string[]> rows, Dictionary<string, string>? summary = null);
    byte[] GenerateCsv(string[] headers, List<string[]> rows);
}

public class ExportService : IExportService
{
    public ExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GeneratePdf(string title, string[] headers, List<string[]> rows, Dictionary<string, string>? summary = null)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(c => ComposeHeader(c, title));
                page.Content().Element(c => ComposeContent(c, headers, rows, summary));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, string title)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("PPM System").Bold().FontSize(16);
                    col.Item().Text(title).FontSize(14);
                });
                row.ConstantItem(150).Column(col =>
                {
                    col.Item().AlignRight().Text($"Generated: {DateTime.Now:dd-MMM-yyyy HH:mm}").FontSize(9);
                });
            });
            column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);
        });
    }

    private void ComposeContent(IContainer container, string[] headers, List<string[]> rows, Dictionary<string, string>? summary)
    {
        container.PaddingVertical(10).Column(column =>
        {
            // Summary section if provided
            if (summary != null && summary.Count > 0)
            {
                column.Item().PaddingBottom(15).Row(row =>
                {
                    int i = 0;
                    foreach (var item in summary)
                    {
                        row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Column(col =>
                        {
                            col.Item().Text(item.Key).FontSize(9).FontColor(Colors.Grey.Darken1);
                            col.Item().Text(item.Value).Bold().FontSize(12);
                        });
                        i++;
                    }
                });
            }

            // Table
            column.Item().Table(table =>
            {
                // Define columns
                table.ColumnsDefinition(columns =>
                {
                    for (int i = 0; i < headers.Length; i++)
                    {
                        columns.RelativeColumn();
                    }
                });

                // Header row
                table.Header(header =>
                {
                    foreach (var h in headers)
                    {
                        header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                            .Text(h).FontColor(Colors.White).Bold().FontSize(9);
                    }
                });

                // Data rows
                bool alternate = false;
                foreach (var row in rows)
                {
                    var bgColor = alternate ? Colors.Grey.Lighten4 : Colors.White;
                    foreach (var cell in row)
                    {
                        table.Cell().Background(bgColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                            .Padding(5).Text(cell ?? "-").FontSize(9);
                    }
                    alternate = !alternate;
                }
            });

            column.Item().PaddingTop(10).Text($"Total Records: {rows.Count}").FontSize(9).Italic();
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
            column.Item().PaddingTop(5).Row(row =>
            {
                row.RelativeItem().Text("PPM - Petrol Pump Management System").FontSize(8).FontColor(Colors.Grey.Darken1);
                row.RelativeItem().AlignRight().Text(x =>
                {
                    x.Span("Page ").FontSize(8);
                    x.CurrentPageNumber().FontSize(8);
                    x.Span(" of ").FontSize(8);
                    x.TotalPages().FontSize(8);
                });
            });
        });
    }

    public byte[] GenerateExcel(string sheetName, string[] headers, List<string[]> rows, Dictionary<string, string>? summary = null)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(sheetName.Length > 31 ? sheetName[..31] : sheetName);

        int startRow = 1;

        // Add summary if provided
        if (summary != null && summary.Count > 0)
        {
            int col = 1;
            foreach (var item in summary)
            {
                worksheet.Cell(1, col).Value = item.Key;
                worksheet.Cell(1, col).Style.Font.Bold = true;
                worksheet.Cell(1, col).Style.Fill.BackgroundColor = XLColor.LightGray;
                worksheet.Cell(2, col).Value = item.Value;
                col++;
            }
            startRow = 4;
        }

        // Add headers
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cell(startRow, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.DarkBlue;
            cell.Style.Font.FontColor = XLColor.White;
        }

        // Add data rows
        for (int r = 0; r < rows.Count; r++)
        {
            for (int c = 0; c < rows[r].Length; c++)
            {
                worksheet.Cell(startRow + 1 + r, c + 1).Value = rows[r][c] ?? "";
            }
        }

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();

        // Add table styling
        var tableRange = worksheet.Range(startRow, 1, startRow + rows.Count, headers.Length);
        tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GenerateCsv(string[] headers, List<string[]> rows)
    {
        var sb = new StringBuilder();

        // Add headers
        sb.AppendLine(string.Join(",", headers.Select(EscapeCsvField)));

        // Add rows
        foreach (var row in rows)
        {
            sb.AppendLine(string.Join(",", row.Select(EscapeCsvField)));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string EscapeCsvField(string? field)
    {
        if (string.IsNullOrEmpty(field)) return "";

        // If field contains comma, quote, or newline, wrap in quotes and escape internal quotes
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
    }
}
