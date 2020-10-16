namespace Public.Api.Status.Clients
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public static class ClientsExtensions
    {
        public static async Task<IEnumerable<T>> GetStatuses<T>(
            this IEnumerable<IStatusClient<T>> clients,
            CancellationToken cancellationToken)
            => await Task.WhenAll(
                clients
                    .AsParallel()
                    .Select(async client => await client.GetStatus(cancellationToken)));
    }
}
