using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FreelanceFlow.Application.DTOs.Invoice;
using FreelanceFlow.Application.Services.Interfaces;
using FreelanceFlow.Domain.Enums;

namespace FreelanceFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly IPdfService _pdfService;
    private readonly IEmailService _emailService;

    public InvoicesController(IInvoiceService invoiceService, IPdfService pdfService, IEmailService emailService)
    {
        _invoiceService = invoiceService;
        _pdfService = pdfService;
        _emailService = emailService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _invoiceService.GetAllAsync();
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { 
            success = true, 
            data = result.Value 
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _invoiceService.GetByIdAsync(id);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { 
            success = true, 
            data = result.Value 
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto)
    {
        var result = await _invoiceService.CreateAsync(dto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { 
            success = true, 
            data = new { id = result.Value }, 
            message = "Fatura başarıyla oluşturuldu" 
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInvoiceDto dto)
    {
        var result = await _invoiceService.UpdateAsync(id, dto);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { 
            success = true, 
            data = result.Value,
            message = "Fatura başarıyla güncellendi" 
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _invoiceService.DeleteAsync(id);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { 
            success = true, 
            message = result.Value 
        });
    }

    [HttpPost("{id}/send-email")]
    public async Task<IActionResult> SendInvoiceByEmail(Guid id)
    {
        var result = await _invoiceService.SendInvoiceByEmailAsync(id);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { 
            success = true, 
            message = result.Value 
        });
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateInvoiceStatusDto dto)
    {
        var result = await _invoiceService.UpdateStatusAsync(id, dto.Status);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { 
            success = true, 
            message = result.Value 
        });
    }

    [HttpPut("{id}/payment-status")]
    public async Task<IActionResult> UpdatePaymentStatus(Guid id, [FromBody] UpdatePaymentStatusDto dto)
    {
        var result = await _invoiceService.UpdatePaymentStatusAsync(id, dto.PaymentStatus, dto.PaidAt);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { 
            success = true, 
            message = result.Value 
        });
    }

    [HttpGet("client/{clientId}")]
    public async Task<IActionResult> GetByClientId(Guid clientId)
    {
        var result = await _invoiceService.GetByClientIdAsync(clientId);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { 
            success = true, 
            data = result.Value 
        });
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProjectId(Guid projectId)
    {
        var result = await _invoiceService.GetByProjectIdAsync(projectId);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { 
            success = true, 
            data = result.Value 
        });
    }

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdueInvoices()
    {
        var result = await _invoiceService.GetOverdueInvoicesAsync();
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { 
            success = true, 
            data = result.Value 
        });
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingInvoices()
    {
        var result = await _invoiceService.GetAllAsync();
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        var pendingInvoices = result.Value.Where(i => i.PaymentStatus == PaymentStatus.Pending);

        return Ok(new { 
            success = true, 
            data = pendingInvoices 
        });
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> DownloadPdf(Guid id)
    {
        try
        {
            var result = await _invoiceService.GeneratePdfAsync(id);
            
            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.Error });
            }

            var invoice = await _invoiceService.GetByIdAsync(id);
            if (!invoice.IsSuccess)
            {
                return BadRequest(new { error = "Fatura bulunamadı" });
            }

            return File(result.Value, "application/pdf", $"Fatura_{invoice.Value.InvoiceNumber}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"PDF oluşturulurken hata oluştu: {ex.Message}" });
        }
    }

    [HttpPost("{id}/mark-paid")]
    public async Task<IActionResult> MarkAsPaid(Guid id)
    {
        var result = await _invoiceService.UpdatePaymentStatusAsync(id, PaymentStatus.Paid, DateTime.UtcNow);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { 
            success = true, 
            message = result.Value 
        });
    }
}