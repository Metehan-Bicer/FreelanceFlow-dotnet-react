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
        // Check if client exists
        var client = await _clientRepository.GetByIdAsync(dto.ClientId);
        if (client == null)
        {
            return Result<Guid>.Failure("Belirtilen müşteri bulunamadı.");
        }

        // Check if project exists (if provided)
        if (dto.ProjectId.HasValue)
        {
            var project = await _projectRepository.GetByIdAsync(dto.ProjectId.Value);
            if (project == null)
            {
                return Result<Guid>.Failure("Belirtilen proje bulunamadı.");
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
        var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(id);
        
        if (invoice == null)
        {
            return Result<InvoiceDto>.Failure("Fatura bulunamadı.");
        }

        if (invoice.Status == InvoiceStatus.Paid)
        {
            return Result<InvoiceDto>.Failure("Ödenmiş faturalar güncellenemez.");
        }

        // Calculate new totals
        var subTotal = dto.Items.Sum(i => i.Quantity * i.UnitPrice);
        var taxAmount = subTotal * (dto.TaxRate / 100);
        var totalAmount = subTotal + taxAmount;

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

        var updatedInvoice = await _invoiceRepository.GetByIdWithDetailsAsync(id);
        var invoiceDto = _mapper.Map<InvoiceDto>(updatedInvoice);
        
        return Result<InvoiceDto>.Success(invoiceDto);
    }

    public async Task<Result<string>> DeleteAsync(Guid id)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        
        if (invoice == null)
        {
            return Result<string>.Failure("Fatura bulunamadı.");
        }

        if (invoice.Status == InvoiceStatus.Paid)
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
        
        return Result<string>.Success("Fatura durumu güncellendi.");
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
            invoice.Status = InvoiceStatus.Paid;
        }

        await _invoiceRepository.UpdateAsync(invoice);
        
        return Result<string>.Success("Ödeme durumu güncellendi.");
    }

    public async Task<Result<string>> SendInvoiceByEmailAsync(Guid id)
    {
        var invoiceResult = await GetByIdAsync(id);
        if (!invoiceResult.IsSuccess)
        {
            return Result<string>.Failure(invoiceResult.Error ?? "Fatura bulunamadı.");
        }

        var invoice = invoiceResult.Value;
        if (invoice == null)
        {
            return Result<string>.Failure("Fatura bulunamadı.");
        }
        
        var client = await _clientRepository.GetByIdAsync(invoice.ClientId);
        
        if (client == null || string.IsNullOrEmpty(client.Email))
        {
            return Result<string>.Failure("Müşteri email adresi bulunamadı.");
        }

        // Generate PDF
        var pdfResult = await _pdfService.GenerateInvoicePdfAsync(invoice);
        if (!pdfResult.IsSuccess)
        {
            return Result<string>.Failure(pdfResult.Error ?? "PDF oluşturulamadı.");
        }

        // Send email
        var emailResult = await _emailService.SendInvoiceEmailAsync(client.Email, invoice, pdfResult.Value!);
        if (!emailResult.IsSuccess)
        {
            return Result<string>.Failure(emailResult.Error ?? "Email gönderilemedi.");
        }

        // Update invoice status
        var invoiceEntity = await _invoiceRepository.GetByIdAsync(id);
        if (invoiceEntity != null && invoiceEntity.Status == InvoiceStatus.Draft)
        {
            invoiceEntity.Status = InvoiceStatus.Sent;
            await _invoiceRepository.UpdateAsync(invoiceEntity);
        }

        return Result<string>.Success("Fatura email ile gönderildi.");
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

    public async Task<Result<IEnumerable<InvoiceDto>>> GetOverdueInvoicesAsync()
    {
        var invoices = await _invoiceRepository.GetOverdueInvoicesAsync();
        var invoiceDtos = _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
        return Result<IEnumerable<InvoiceDto>>.Success(invoiceDtos);
    }
}