using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;

namespace Acme.BookStore;

[DependsOn(
    typeof(BookStoreDomainModule),
    typeof(BookStoreTestBaseModule),
    typeof(AbpOpenIddictDomainModule)  
)]
public class BookStoreDomainTestModule : AbpModule
{

}