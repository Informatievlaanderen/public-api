namespace Public.Api.Infrastructure.Configuration
{
   using PostalRegistry.Api.Legacy.Infrastructure.Options;

   public class PostalOptions : ResponseOptions, IRegistryOptions
   {
       public SyndicationOptions Syndication { get; set; }
   }
}
