using Intervip.Telnet.Clients;

namespace Intervip.Telnet.Interfaces;

public interface IHuaweiService : IDisposable
{
	Task<HuaweiClient> CreateClientAsync(string address, int port = 23, CancellationToken cancellationToken = default);
}
