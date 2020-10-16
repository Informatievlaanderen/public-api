namespace Public.Api.Status.Clients
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IStatusClient<TStatus>
    {
        string Registry { get; }
        Task<TStatus> GetStatus(CancellationToken cancellationToken);
    }
}
