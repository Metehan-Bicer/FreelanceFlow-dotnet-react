using AutoMapper;
using FreelanceFlow.Application.DTOs.Client;
using FreelanceFlow.Application.DTOs.Project;
using FreelanceFlow.Application.DTOs.Invoice;
using FreelanceFlow.Domain.Entities;

namespace FreelanceFlow.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Client mappings
        CreateMap<Client, ClientDto>()
            .ForMember(dest => dest.ProjectCount, opt => opt.MapFrom(src => src.Projects.Count))
            .ForMember(dest => dest.TotalRevenue, opt => opt.MapFrom(src => src.Invoices.Where(i => i.PaymentStatus == Domain.Enums.PaymentStatus.Paid).Sum(i => i.TotalAmount)));
            
        CreateMap<CreateClientDto, Client>();
        CreateMap<UpdateClientDto, Client>();

        // Project mappings
        CreateMap<Project, ProjectDto>()
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client != null ? src.Client.Name : string.Empty));
            
        CreateMap<CreateProjectDto, Project>();
        CreateMap<UpdateProjectDto, Project>();

        // Invoice mappings
        CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client != null ? src.Client.Name : string.Empty))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : null))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            
        CreateMap<InvoiceItem, InvoiceItemDto>();
        CreateMap<CreateInvoiceDto, Invoice>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            
        CreateMap<CreateInvoiceItemDto, InvoiceItem>();
        CreateMap<UpdateInvoiceDto, Invoice>()
            .ForMember(dest => dest.Items, opt => opt.Ignore()); // Handle separately in service

        // Task mappings
        CreateMap<ProjectTask, object>(); // Placeholder for task DTOs
        CreateMap<TimeEntry, object>(); // Placeholder for time entry DTOs
    }
}