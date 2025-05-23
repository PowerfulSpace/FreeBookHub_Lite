using Mapster;
using PS.FreeBookHub_Lite.CatalogService.Application.DTOs;
using PS.FreeBookHub_Lite.CatalogService.Domain.Entities;

namespace PS.FreeBookHub_Lite.CatalogService.Application.Mapping
{
    public class BookMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Book, BookDto>();
            config.NewConfig<CreateBookRequest, Book>();
            config.NewConfig<UpdateBookRequest, Book>();
        }
    }
}
