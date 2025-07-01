using AutoMapper;
using FreelanceFlow.Application.DTOs.Invoice;
using FreelanceFlow.Application.Services.Interfaces;
using FreelanceFlow.Core.Common;
using FreelanceFlow.Domain.Entities;
using FreelanceFlow.Domain.Enums;
using FreelanceFlow.Domain.Interfaces.Repositories;

namespace FreelanceFlow.Application.Services.Implementations;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IPdfService _pdfService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        IClientRepository clientRepository,
        IProjectRepository projectRepository,
        IPdfService pdfService,
        IEmailService emailService,
        IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _clientRepository = clientRepository;
        _projectRepository = projectRepository;
        _pdfService = pdfService;
        _emailService = emailService;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<InvoiceDto>>> GetAllAsync()
    {
        var invoices = await _invoiceRepository.GetAllWithDetailsAsync();
        var invoiceDtos = _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
        return Result<IEnumerable<InvoiceDto>>.Success(invoiceDtos);
    }

    public async Task<Result<InvoiceDto>> GetByIdAsync(Guid id)
    {
        var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(id);
        
        if (invoice == null)
        {
            return Result<InvoiceDto>.Failure("Fatura bulunamadı.");
        }

        var invoiceDto = _mapper.Map<InvoiceDto>(invoice);
        return Result<InvoiceDto>.Success(invoiceDto);
    }

    public async Task<Result<Guid>> CreateAsync(CreateInvoiceDto dto)
    {
        // Check if client exists and is active
        var client = await _clientRepository.GetByIdAsync(dto.ClientId);
        if (client == null)
        {
            return Result<Guid>.Failure("Belirtilen müşteri bulunamadı.");
        }

        if (client.Status != FreelanceFlow.Domain.Enums.ClientStatus.Active)
        {
            return Result<Guid>.Failure("Pasif durumda olan müşteri için fatura oluşturulamaz.");
        }

        // Check if project exists and is active (if provided)
        if (dto.ProjectId.HasValue)
        {
            var project = await _projectRepository.GetByIdAsync(dto.ProjectId.Value);
            if (project == null)
            {
                return Result<Guid>.Failure("Belirtilen proje bulunamadı.");
            }

            if (!project.IsActive)
            {
                return Result<Guid>.Failure("Pasif durumda olan proje için fatura oluşturulamaz.");
            }
        }

        // Generate invoice number
        var invoiceNumber = await _invoiceRepository.GenerateInvoiceNumberAsync();

        // Calculate totals
        var subTotal = dto.Items.Sum(i => i.Quantity * i.UnitPrice);
        var taxAmount = subTotal * (dto.TaxRate / 100);
        var totalAmount = subTotal + taxAmount;

        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            ClientId = dto.ClientId,
            ProjectId = dto.ProjectId,
            IssueDate = dto.IssueDate,
            DueDate = dto.DueDate,
            SubTotal = subTotal,
            TaxAmount = taxAmount,
            TotalAmount = totalAmount,
            Status = InvoiceStatus.Draft,
            PaymentStatus = PaymentStatus.Pending,
            Notes = dto.Notes,
            Items = dto.Items.Select(i => new InvoiceItem
            {
                Description = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.Quantity * i.UnitPrice
            }).ToList()
        };

        await _invoiceRepository.AddAsync(invoice);
        return Result<Guid>.Success(invoice.Id);
    }

    public async Task<Result<InvoiceDto>> UpdateAsync(Guid id, UpdateInvoiceDto dto)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice == null)
        {
            return Result<InvoiceDto>.Failure("Fatura bulunamadı.");
        }

        // Calculate totals
        var subTotal = dto.Items.Sum(i => i.Quantity * i.UnitPrice);
        var taxAmount = subTotal * (dto.TaxRate / 100);
        var totalAmount = subTotal + taxAmount;

        // Update invoice properties
        invoice.IssueDate = dto.IssueDate;
        invoice.DueDate = dto.DueDate;
        invoice.SubTotal = subTotal;
        invoice.TaxAmount = taxAmount;
        invoice.TotalAmount = totalAmount;
        invoice.Notes = dto.Notes;

        // Update items
        invoice.Items.Clear();
        invoice.Items = dto.Items.Select(i => new InvoiceItem
        {
            InvoiceId = invoice.Id,
            Description = i.Description,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            TotalPrice = i.Quantity * i.UnitPrice
        }).ToList();

        await _invoiceRepository.UpdateAsync(invoice);
        
        var updatedInvoiceDto = _mapper.Map<InvoiceDto>(invoice);
        return Result<InvoiceDto>.Success(updatedInvoiceDto);
    }

    public async Task<Result<string>> DeleteAsync(Guid id)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice == null)
        {
            return Result<string>.Failure("Fatura bulunamadı.");
        }

        // Check if invoice can be deleted (not paid)
        if (invoice.PaymentStatus == PaymentStatus.Paid)
        {
            return Result<string>.Failure("Ödenmiş faturalar silinemez.");
        }

        await _invoiceRepository.DeleteAsync(invoice);
        return Result<string>.Success("Fatura başarıyla silindi.");
    }

    public async Task<Result<string>> UpdateStatusAsync(Guid id, InvoiceStatus status)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice == null)
        {
            return Result<string>.Failure("Fatura bulunamadı.");
        }

        invoice.Status = status;
        await _invoiceRepository.UpdateAsync(invoice);
        
        return Result<string>.Success("Fatura durumu başarıyla güncellendi.");
    }

    public async Task<Result<string>> UpdatePaymentStatusAsync(Guid id, PaymentStatus paymentStatus, DateTime? paidAt)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice == null)
        {
            return Result<string>.Failure("Fatura bulunamadı.");
        }

        invoice.PaymentStatus = paymentStatus;
        if (paymentStatus == PaymentStatus.Paid && paidAt.HasValue)
        {
            invoice.PaidAt = paidAt.Value;
        }
        
        await _invoiceRepository.UpdateAsync(invoice);
        
        return Result<string>.Success("Ödeme durumu başarıyla güncellendi.");
    }

    public async Task<Result<string>> SendInvoiceByEmailAsync(Guid id)
    {
        var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(id);
        if (invoice == null)
        {
            return Result<string>.Failure("Fatura bulunamadı.");
        }

        try
        {
            // Generate PDF - Invoice entity'yi direkt geç
            var pdfBytes = await _pdfService.GenerateInvoicePdfAsync(invoice);
            
            // Send email
            var emailSent = await _emailService.SendInvoiceEmailAsync(
                invoice.Client.Email,
                invoice.Client.Name,
                invoice.InvoiceNumber,
                pdfBytes);

            if (!emailSent)
            {
                return Result<string>.Failure("Fatura gönderilirken hata oluştu.");
            }

            // Update invoice status
            invoice.Status = InvoiceStatus.Sent;
            await _invoiceRepository.UpdateAsync(invoice);

            return Result<string>.Success("Fatura başarıyla gönderildi.");
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Fatura gönderilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<InvoiceDto>>> GetOverdueInvoicesAsync()
    {
        var overdueInvoices = await _invoiceRepository.GetOverdueInvoicesAsync();
        var invoiceDtos = _mapper.Map<IEnumerable<InvoiceDto>>(overdueInvoices);
        return Result<IEnumerable<InvoiceDto>>.Success(invoiceDtos);
    }

    public async Task<Result<bool>> MarkAsPaidAsync(Guid id, DateTime? paidDate = null)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice == null)
        {
            return Result<bool>.Failure("Fatura bulunamadı.");
        }

        invoice.PaymentStatus = PaymentStatus.Paid;
        invoice.PaidAt = paidDate ?? DateTime.UtcNow;
        
        await _invoiceRepository.UpdateAsync(invoice);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> MarkAsOverdueAsync(Guid id)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice == null)
        {
            return Result<bool>.Failure("Fatura bulunamadı.");
        }

        invoice.PaymentStatus = PaymentStatus.Overdue;
        
        await _invoiceRepository.UpdateAsync(invoice);
        return Result<bool>.Success(true);
    }

    public async Task<Result<byte[]>> GeneratePdfAsync(Guid id)
    {
        var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(id);
        if (invoice == null)
        {
            return Result<byte[]>.Failure("Fatura bulunamadı.");
        }

        var pdfBytes = await _pdfService.GenerateInvoicePdfAsync(invoice);
        return Result<byte[]>.Success(pdfBytes);
    }

    public async Task<Result<IEnumerable<InvoiceDto>>> GetByClientIdAsync(Guid clientId)
    {
        var invoices = await _invoiceRepository.GetByClientIdAsync(clientId);
        var invoiceDtos = _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
        return Result<IEnumerable<InvoiceDto>>.Success(invoiceDtos);
    }

    public async Task<Result<IEnumerable<InvoiceDto>>> GetByProjectIdAsync(Guid projectId)
    {
        var invoices = await _invoiceRepository.GetByProjectIdAsync(projectId);
        var invoiceDtos = _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
        return Result<IEnumerable<InvoiceDto>>.Success(invoiceDtos);
    }

    public async Task<Result<InvoiceStatsDto>> GetInvoiceStatsAsync()
    {
        var allInvoices = await _invoiceRepository.GetAllAsync();
        
        var stats = new InvoiceStatsDto
        {
            TotalInvoices = allInvoices.Count(),
            PaidInvoices = allInvoices.Count(i => i.PaymentStatus == PaymentStatus.Paid),
            PendingInvoices = allInvoices.Count(i => i.PaymentStatus == PaymentStatus.Pending),
            OverdueInvoices = allInvoices.Count(i => i.PaymentStatus == PaymentStatus.Overdue),
            TotalAmount = allInvoices.Sum(i => i.TotalAmount),
            PaidAmount = allInvoices.Where(i => i.PaymentStatus == PaymentStatus.Paid).Sum(i => i.TotalAmount),
            PendingAmount = allInvoices.Where(i => i.PaymentStatus == PaymentStatus.Pending).Sum(i => i.TotalAmount),
            OverdueAmount = allInvoices.Where(i => i.PaymentStatus == PaymentStatus.Overdue).Sum(i => i.TotalAmount)
        };

        return Result<InvoiceStatsDto>.Success(stats);
    }
}