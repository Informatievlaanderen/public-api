namespace Public.Api.Notifications
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.FeatureToggles;
    using Common.Infrastructure.Extensions;
    using Infrastructure;
    using Infrastructure.Swagger;
    using Marvin.Cache.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using NotificationService.Api.Abstractions;
    using RestSharp;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
    using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

    public partial class NotificationsController
    {
        /// <summary>
        /// Maak een notificatie aan.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="actionContextAccessor"></param>
        /// <param name="problemDetailsHelper"></param>
        /// <param name="featureToggle"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de aanmaak van de notificatie gelukt is.</response>
        /// <response code="400">Als uw verzoek foutieve data bevat.</response>
        /// <response code="401">Als u niet geauthenticeerd bent om deze actie uit te voeren.</response>
        /// <response code="403">Als u niet beschikt over de correcte rechten om deze actie uit te voeren.</response>
        /// <response code="406">Als het gevraagde formaat niet beschikbaar is.</response>
        /// <response code="429">Als het aantal requests per seconde de limiet overschreven heeft.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpPost("notificaties", Name = nameof(CreateNotification))]
        [ApiOrder(ApiOrder.Notifications + 1)]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(NotificatieAangemaakt), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateNotification(
            [FromBody] MaakNotificatieRequest request,
            [FromServices] IActionContextAccessor actionContextAccessor,
            [FromServices] ProblemDetailsHelper problemDetailsHelper,
            [FromServices] CreateNotificationToggle featureToggle,
            CancellationToken cancellationToken = default)
        {
            if (!featureToggle.FeatureEnabled)
            {
                return NotFound();
            }

            var contentFormat = DetermineFormat(actionContextAccessor.ActionContext);

            RestRequest BackendRequest() =>
                new RestRequest($"{BackOfficeVersion}/notificaties", Method.Post)
                    .AddBody(request)
                    .AddHeaderAuthorization(actionContextAccessor);

            var value = await GetFromBackendWithBadRequestAsync(
                contentFormat.ContentType,
                BackendRequest,
                CreateDefaultHandleBadRequest(),
                problemDetailsHelper,
                cancellationToken: cancellationToken);

            return new BackendResponseResult(value, BackendResponseResultOptions.ForBackOffice());
        }
    }
}

//TODO-pr remove and use nuget package Be.Vlaanderen.Basisregisters.NotificationService.Abstractions
namespace NotificationService.Api.Abstractions
{
    using System;
    using System.Collections.Generic;
    using Basisregisters.IntegrationDb.SuspiciousCases.Api.Abstractions.List;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Filters;

    public sealed class MaakNotificatieRequest
    {
        [JsonProperty("geldigVanaf")] public DateTimeOffset? GeldigVanaf { get; set; }

        [JsonProperty("geldigTot")] public DateTimeOffset? GeldigTot { get; set; }

        [JsonProperty("ernst")] public NotificatieErnst Ernst { get; set; }

        [JsonProperty("titel")] public required string Titel { get; set; }

        [JsonProperty("inhoud")] public required string Inhoud { get; set; }

        [JsonProperty("platformen")] public required ICollection<Platform> Platformen { get; set; }

        [JsonProperty("rollen")] public required ICollection<Rol> Rollen { get; set; }

        [JsonProperty("kanSluiten")] public bool KanSluiten { get; set; }

        [JsonProperty("links")] public ICollection<MaakNotificatieLink> Links { get; set; } = [];
    }

    public record MaakNotificatieLink(string Label, string Url);

    public enum Platform
    {
        Lara,
        Geoit
    }

    public enum Rol
    {
        NietIngelogd,
        StandaardGebruiker,
        InterneBeheerder
    }

    public record NotificatieAangemaakt(int NotificatieId);

    public class NotificationsFilter
    {
        public NotificatieStatus? Status { get; init; }
        public DateTimeOffset? Vanaf { get; init; }
        public DateTimeOffset? Tot { get; init; }
    }

    public enum NotificatieStatus
    {
        Concept,
        Gepubliceerd,
        Ingetrokken
    }
    public enum NotificatieErnst
    {
        Informatie,
        Waarschuwing,
        Fout
    }
    public sealed class Notificatie
    {
        [JsonProperty("notificatieId")]
        public required int NotificatieId { get; init; }

        [JsonProperty("status")]
        public required NotificatieStatus Status { get; init; }

        [JsonProperty("geldigVanaf")]
        public required DateTimeOffset GeldigVanaf { get; init; }

        [JsonProperty("geldigTot")]
        public required DateTimeOffset GeldigTot { get; init; }

        [JsonProperty("ernst")]
        public required NotificatieErnst Ernst { get; init; }

        [JsonProperty("titel")]
        public required string Titel { get; init; }

        [JsonProperty("inhoud")]
        public required string Inhoud { get; init; }

        [JsonProperty("platformen")]
        public required ICollection<Platform> Platformen { get; init; }

        [JsonProperty("rollen")]
        public required ICollection<Rol> Rollen { get; init; }

        [JsonProperty("kanSluiten")]
        public required bool KanSluiten { get; init; }

        [JsonProperty("links")]
        public required ICollection<NotificatieLink> Links { get; init; }
    }

    public record NotificatieLink(string Label, string Url);

    public class GetNotificatiesResponseExample : IExamplesProvider<Notificatie[]>
    {
        public Notificatie[] GetExamples()
        {
            return
            [
                new Notificatie
                {
                    NotificatieId = 1,
                    Status = NotificatieStatus.Gepubliceerd,
                    GeldigVanaf = DateTimeOffset.Now.Date.AddDays(-2),
                    GeldigTot = DateTimeOffset.Now.AddDays(7),
                    Ernst = NotificatieErnst.Informatie,
                    Titel = "Systeemonderhoud",
                    Inhoud = "Er is gepland systeemonderhoud op 15 juni.",
                    Platformen = [ Platform.Lara, Platform.Geoit ],
                    Rollen = [ Rol.NietIngelogd, Rol.StandaardGebruiker, Rol.InterneBeheerder ],
                    KanSluiten = true,
                    Links = [ new NotificatieLink("Meer info", "https://example.com/onderhoud") ]
                }
            ];
        }
    }

}
