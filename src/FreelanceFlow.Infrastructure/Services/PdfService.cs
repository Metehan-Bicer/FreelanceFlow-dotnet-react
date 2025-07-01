using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using FreelanceFlow.Application.Services.Interfaces;
using FreelanceFlow.Domain.Entities;

namespace FreelanceFlow.Infrastructure.Services;

public class PdfService : IPdfService
{
    public async Task<byte[]> GenerateInvoicePdfAsync(Invoice invoice)
    {
        // QuestPDF license (for development)
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Text($"FATURA #{invoice.InvoiceNumber}")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        // Firma Bilgileri
                        x.Item().Text("FreelanceFlow").FontSize(24).SemiBold();
                        x.Item().Text("Freelance Proje Yönetim Sistemi").FontSize(12);
                        x.Item().PaddingTop(0.5f, Unit.Centimetre);

                        // Müşteri Bilgileri
                        x.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Fatura Bilgileri:").SemiBold();
                                col.Item().Text($"Fatura No: {invoice.InvoiceNumber}");
                                col.Item().Text($"Tarih: {invoice.IssueDate:dd.MM.yyyy}");
                                col.Item().Text($"Vade: {invoice.DueDate:dd.MM.yyyy}");
                            });

                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Müşteri:").SemiBold();
                                col.Item().Text(invoice.Client?.Name ?? "Müşteri Adı");
                                if (!string.IsNullOrEmpty(invoice.Client?.Email))
                                    col.Item().Text($"E-posta: {invoice.Client.Email}");
                                if (!string.IsNullOrEmpty(invoice.Client?.Phone))
                                    col.Item().Text($"Telefon: {invoice.Client.Phone}");
                            });
                        });

                        x.Item().PaddingTop(1, Unit.Centimetre);

                        // Proje Bilgisi (varsa)
                        if (invoice.Project != null)
                        {
                            x.Item().Text($"Proje: {invoice.Project.Name}").SemiBold();
                            x.Item().PaddingBottom(0.5f, Unit.Centimetre);
                        }

                        // Fatura Kalemleri Tablosu
                        x.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Açıklama
                                columns.RelativeColumn(1); // Miktar
                                columns.RelativeColumn(1); // Birim Fiyat
                                columns.RelativeColumn(1); // Toplam
                            });

                            // Tablo Başlığı
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Açıklama").SemiBold();
                                header.Cell().Element(CellStyle).Text("Miktar").SemiBold();
                                header.Cell().Element(CellStyle).Text("Birim Fiyat").SemiBold();
                                header.Cell().Element(CellStyle).Text("Toplam").SemiBold();

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            // Fatura Kalemleri
                            foreach (var item in invoice.Items)
                            {
                                table.Cell().Element(CellStyle).Text(item.Description);
                                table.Cell().Element(CellStyle).Text($"{item.Quantity:N2}");
                                table.Cell().Element(CellStyle).Text($"{item.UnitPrice:N2} ₺");
                                table.Cell().Element(CellStyle).Text($"{item.TotalPrice:N2} ₺");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                }
                            }
                        });

                        x.Item().PaddingTop(1, Unit.Centimetre);

                        // Toplam Hesaplamalar
                        x.Item().AlignRight().Column(totals =>
                        {
                            totals.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Ara Toplam:");
                                row.ConstantItem(100).Text($"{invoice.SubTotal:N2} ₺").AlignRight();
                            });

                            totals.Item().Row(row =>
                            {
                                row.RelativeItem().Text("KDV:");
                                row.ConstantItem(100).Text($"{invoice.TaxAmount:N2} ₺").AlignRight();
                            });

                            totals.Item().BorderTop(1).BorderColor(Colors.Black).PaddingTop(5).Row(row =>
                            {
                                row.RelativeItem().Text("GENEL TOPLAM:").SemiBold().FontSize(14);
                                row.ConstantItem(100).Text($"{invoice.TotalAmount:N2} ₺").SemiBold().FontSize(14).AlignRight();
                            });
                        });

                        // Notlar
                        if (!string.IsNullOrEmpty(invoice.Notes))
                        {
                            x.Item().PaddingTop(1, Unit.Centimetre).Column(notes =>
                            {
                                notes.Item().Text("Notlar:").SemiBold();
                                notes.Item().Text(invoice.Notes);
                            });
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Sayfa ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
            });
        });

        return await Task.FromResult(document.GeneratePdf());
    }
}