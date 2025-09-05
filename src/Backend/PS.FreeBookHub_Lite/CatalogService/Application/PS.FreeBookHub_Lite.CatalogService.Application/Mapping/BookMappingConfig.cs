using Mapster;
using PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Commands.CreateBook;
using PS.FreeBookHub_Lite.CatalogService.Application.CQRS.Commands.UpdateBook;
using PS.FreeBookHub_Lite.CatalogService.Application.DTOs;
using PS.FreeBookHub_Lite.CatalogService.Domain.Entities;

namespace PS.FreeBookHub_Lite.CatalogService.Application.Mapping
{
    public class BookMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Book, BookResponse>();

            config.NewConfig<CreateBookRequest, Book>();
            config.NewConfig<CreateBookRequest, CreateBookCommand>();

            config.NewConfig<UpdateBookRequest, Book>();
            config.NewConfig<UpdateBookRequest, UpdateBookCommand>();
        }
    }
}
