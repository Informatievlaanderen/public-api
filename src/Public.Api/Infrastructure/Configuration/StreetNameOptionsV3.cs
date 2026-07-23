namespace Public.Api.Infrastructure.Configuration
{
   using StreetNameRegistry.Api.Oslo.Infrastructure.Options;

   public class StreetNameOptionsV3 : ResponseOptionsV3, IRegistryOptions
   {
       public SyndicationOptions? Syndication { get; set; }
   }
}
