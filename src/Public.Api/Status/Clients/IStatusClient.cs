namespace Public.Api.Status.Clients
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IStatusClient<TStatus>
    {
        Task<TStatus> GetStatus(CancellationToken cancellationToken);
    }
}
