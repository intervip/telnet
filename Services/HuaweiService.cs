using Intervip.Telnet.Clients;
using Intervip.Telnet.Interfaces;

using Microsoft.Extensions.Logging;

using System.Net;

namespace Intervip.Telnet.Services;

public class HuaweiService : IHuaweiService, IDisposable
{
	private readonly ILogger<HuaweiService> logger;
	private readonly List<HuaweiClient> clients;
	private bool disposedValue;

	public HuaweiService(ILogger<HuaweiService> logger)
	{
		this.logger = logger;
		clients = new List<HuaweiClient>();
	}

	public async Task<HuaweiClient> CreateClientAsync(string address, int port = 23, CancellationToken cancellationToken = default)
	{
		var iPAddress = IPAddress.Parse(address);

		if (clients.FirstOrDefault(client => client.Address.Equals(iPAddress)) is HuaweiClient client)
		{
			logger.LogDebug("Skipping client configuration for already configured host {address}", address);
			return client;
		}

		client = await new HuaweiClient(iPAddress, port).ConnectAsync(cancellationToken);
		logger.LogInformation("Client configuration successful for host {address}", address);
		clients.Add(client);
		return client;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				clients.ForEach(client => client.Dispose());
			}

			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}
