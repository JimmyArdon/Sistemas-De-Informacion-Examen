using AutoMapper;
using WebApiAutores.Dtos.Autores;
using WebApiAutores.Dtos.Books;
using WebApiAutores.Dtos.Comentarios;
using WebApiAutores.Dtos.Reviews;
using WebApiAutores.Entities;

namespace WebApiAutores.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            MapsForBooks();    
            MapsForAutores();
            MapsReviews();
            MapsComentario();
        }

        private void MapsForAutores() 
        {
            CreateMap<Autor, AutorDto>();
            CreateMap<Autor, AutorGetByIdDto>();
            CreateMap<AutorCreateDto, Autor>();
        }

        private void MapsForBooks()
        {
            //CreateMap<BookDto, Book>().ReverseMap();

            CreateMap<Book, BookDto>()
                .ForPath(dest => dest.AutorNombre, opt => opt.MapFrom(src => src.Autor.Name));
            
            CreateMap<BookCreateDto, Book>();
        }

        private void MapsReviews() {

            CreateMap<ReviewsCreationDto, Reviews>();
            CreateMap<ReviewsUpdateDto, Reviews>();
        }

        private void MapsComentario()
        {
            CreateMap<ComentarioCreationDto, Comentarios>();
            CreateMap<ComentariosUpdateDtos, ComentarioDto>();
        }
    }

       
}
