using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using Acme.BookStore.Authors;
using Acme.BookStore.Books;
namespace Acme.BookStore.Web;
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class BookStoreWebMappers : MapperBase<BookDto, CreateUpdateBookDto>
{
    public override partial CreateUpdateBookDto Map(BookDto source);
    public override partial void Map(BookDto source, CreateUpdateBookDto destination);
}
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class BookStoreAuthorDtoToCreateUpdateAuthorDtoMapper : MapperBase<AuthorDto, CreateUpdateAuthorDto>
{
    public override partial CreateUpdateAuthorDto Map(AuthorDto source);
    public override partial void Map(AuthorDto source, CreateUpdateAuthorDto destination);
}
