namespace Public.Api.Infrastructure.Configuration
{
   using StreetNameRegistry.Api.Oslo.Abstractions.Infrastructure.Options;

   public class StreetNameOptionsV2 : ResponseOptions, IRegistryOptions
   {
       public SyndicationOptions Syndication { get; set; }
   }
}
