using AutoMapper;
using RetailStoreManagement.Models;

namespace RetailStoreManagement.Mapping
{
    public class PurchaseProfile : Profile
    {
        public PurchaseProfile()
        {
            CreateMap<Purchase, PurchaseDto>();
            CreateMap<PurchaseProduct, PurchaseProductDto>();

            CreateMap<PurchaseCreateDto, Purchase>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.PurchaseDate, opt => opt.Ignore())
                .ForMember(d => d.PurchaseProducts, opt => opt.MapFrom(s => s.PurchaseProducts));

            CreateMap<PurchaseProductCreateDto, PurchaseProduct>()
                .ForMember(d => d.PurchaseId, opt => opt.Ignore())
                .ForMember(d => d.Purchase, opt => opt.Ignore());

            CreateMap<PurchaseDto, Purchase>();
            CreateMap<PurchaseProductDto, PurchaseProduct>();
        }
    }
}