using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using FreelanceFlow.Application.DTOs.Invoice;
using FreelanceFlow.Application.Services.Interfaces;
using FreelanceFlow.Core.Common;

namespace FreelanceFlow.Infrastructure.Services;

public class PdfService : IPdfService
{
    public async Task<Result<byte[]>> GenerateInvoicePdfAsync(InvoiceDto invoice)
    {
        try
        {
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Header
            var header = new Paragraph("FATURA / INVOICE")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20)
                .SetMarginBottom(20);
            document.Add(header);

            // Company Info
            var companyInfo = new Paragraph("FreelanceFlow")
                .SetFontSize(16)
                .SetMarginBottom(10);
            document.Add(companyInfo);

            // Invoice Details Table
            var detailsTable = new Table(2).UseAllAvailableWidth();
            
            detailsTable.AddCell(new Cell().Add(new Paragraph("Fatura No:")));
            detailsTable.AddCell(new Cell().Add(new Paragraph(invoice.InvoiceNumber)));
            
            detailsTable.AddCell(new Cell().Add(new Paragraph("Tarih:")));
            detailsTable.AddCell(new Cell().Add(new Paragraph(invoice.IssueDate.ToString("dd.MM.yyyy"))));
            
            detailsTable.AddCell(new Cell().Add(new Paragraph("Vade Tarihi:")));
            detailsTable.AddCell(new Cell().Add(new Paragraph(invoice.DueDate.ToString("dd.MM.yyyy"))));
            
            detailsTable.AddCell(new Cell().Add(new Paragraph("Müşteri:")));
            detailsTable.AddCell(new Cell().Add(new Paragraph(invoice.ClientName)));

            if (!string.IsNullOrEmpty(invoice.ProjectName))
            {
                detailsTable.AddCell(new Cell().Add(new Paragraph("Proje:")));
                detailsTable.AddCell(new Cell().Add(new Paragraph(invoice.ProjectName)));
            }

            document.Add(detailsTable.SetMarginBottom(20));

            // Items Table
            var itemsTable = new Table(4).UseAllAvailableWidth();
            
            // Header
            itemsTable.AddHeaderCell(new Cell().Add(new Paragraph("Açıklama")).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
            itemsTable.AddHeaderCell(new Cell().Add(new Paragraph("Miktar")).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
            itemsTable.AddHeaderCell(new Cell().Add(new Paragraph("Birim Fiyat")).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
            itemsTable.AddHeaderCell(new Cell().Add(new Paragraph("Toplam")).SetBackgroundColor(ColorConstants.LIGHT_GRAY));

            // Items
            foreach (var item in invoice.Items)
            {
                itemsTable.AddCell(new Cell().Add(new Paragraph(item.Description)));
                itemsTable.AddCell(new Cell().Add(new Paragraph(item.Quantity.ToString())));
                itemsTable.AddCell(new Cell().Add(new Paragraph(item.UnitPrice.ToString("C", new System.Globalization.CultureInfo("tr-TR")))));
                itemsTable.AddCell(new Cell().Add(new Paragraph(item.TotalPrice.ToString("C", new System.Globalization.CultureInfo("tr-TR")))));
            }

            document.Add(itemsTable.SetMarginBottom(20));

            // Totals Table
            var totalsTable = new Table(2).UseAllAvailableWidth();
            totalsTable.SetWidth(300).SetHorizontalAlignment(HorizontalAlignment.RIGHT);

            totalsTable.AddCell(new Cell().Add(new Paragraph("Ara Toplam:")).SetBorder(Border.NO_BORDER));
            totalsTable.AddCell(new Cell().Add(new Paragraph(invoice.SubTotal.ToString("C", new System.Globalization.CultureInfo("tr-TR")))).SetBorder(Border.NO_BORDER));

            totalsTable.AddCell(new Cell().Add(new Paragraph("KDV:")).SetBorder(Border.NO_BORDER));
            totalsTable.AddCell(new Cell().Add(new Paragraph(invoice.TaxAmount.ToString("C", new System.Globalization.CultureInfo("tr-TR")))).SetBorder(Border.NO_BORDER));

            totalsTable.AddCell(new Cell().Add(new Paragraph("TOPLAM:").SetFontSize(14)).SetBorder(Border.NO_BORDER));
            totalsTable.AddCell(new Cell().Add(new Paragraph(invoice.TotalAmount.ToString("C", new System.Globalization.CultureInfo("tr-TR"))).SetFontSize(14)).SetBorder(Border.NO_BORDER));

            document.Add(totalsTable);

            // Notes
            if (!string.IsNullOrEmpty(invoice.Notes))
            {
                document.Add(new Paragraph("Notlar:")
                    .SetMarginTop(20)
                    .SetMarginBottom(10));
                document.Add(new Paragraph(invoice.Notes));
            }

            // Footer
            document.Add(new Paragraph("Bu fatura FreelanceFlow sistemi tarafından otomatik olarak oluşturulmuştur.")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(10)
                .SetMarginTop(30)
                .SetFontColor(ColorConstants.GRAY));

            document.Close();

            return await Task.FromResult(Result<byte[]>.Success(stream.ToArray()));
        }
        catch (Exception ex)
        {
            return Result<byte[]>.Failure($"PDF oluşturulurken hata oluştu: {ex.Message}");
        }
    }
}