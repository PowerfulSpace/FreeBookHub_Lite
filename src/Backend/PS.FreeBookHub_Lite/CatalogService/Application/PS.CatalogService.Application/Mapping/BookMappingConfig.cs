using Mapster;
using PS.CatalogService.Application.CQRS.Commands.CreateBook;
using PS.CatalogService.Application.CQRS.Commands.UpdateBook;
using PS.CatalogService.Application.DTOs;
using PS.CatalogService.Domain.Entities;

namespace PS.CatalogService.Application.Mapping
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
