using AutoMapper;
using CustomerService.Application.Features.Customers.DTOs.Requests;
using CustomerService.Application.Features.Customers.DTOs.Responses;
using CustomerService.Domain.Entities;

namespace CustomerService.Application.Features.Customers.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile() 
        {
            CreateMap<CreateCustomerRequest, Customer>();
            CreateMap<UpdateCustomerRequest, Customer>();
            CreateMap<Customer, CustomerResponse>()
           .ForMember(
               d => d.FullName,
               o => o.MapFrom(
                   s => $"{s.FirstName} {s.LastName}"
               ));
        }
    }
}
