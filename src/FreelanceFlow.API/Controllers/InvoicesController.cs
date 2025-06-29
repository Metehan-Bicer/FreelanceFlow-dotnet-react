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
}

public class UpdateInvoiceStatusDto
{
    public InvoiceStatus Status { get; set; }
}

public class UpdatePaymentStatusDto
{
    public PaymentStatus PaymentStatus { get; set; }
    public DateTime? PaidAt { get; set; }
}