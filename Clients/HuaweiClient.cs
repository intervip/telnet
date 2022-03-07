using System.Net;
using System.Text;

using Intervip.Telnet.Extensions;

namespace Intervip.Telnet.Clients;

public class HuaweiClient : TelnetClient
{
	private const string DefaultDelimiter = ">";
	private const string EnabledDelimiter = "#";

	public bool LoggedIn { get; set; }

	public HuaweiClient(IPAddress address, int port) : base(address, port)
	{
		LoggedIn = false;
	}

	public new async Task<HuaweiClient> ConnectAsync(CancellationToken cancellationToken = default)
	{
		await base.ConnectAsync(cancellationToken);
		return this;
	}

	public async Task<string> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
	{
		if (LoggedIn is false)
		{
			await ReadAsync("User name:", cancellationToken);
			await WriteAsync(username, cancellationToken);
			await ReadAsync("User password:", cancellationToken);
			await WriteAsync(password, cancellationToken);
			LoggedIn = true;

			return await ReadAsync(DefaultDelimiter, cancellationToken).ToString(default);
		}

		throw new InvalidOperationException("Invalid authentication attempt on an already logged in client");
	}

	public async Task<string> EnableAsync(CancellationToken cancellationToken = default)
	{
		await WriteAsync("enable", cancellationToken);
		return await ReadAsync(EnabledDelimiter, cancellationToken).ToString(default);
	}

	private Task WriteAsync(string text, CancellationToken cancellationToken = default)
	{
		return WriteAsync(Encoding.UTF8.GetBytes($"{text}\n"), cancellationToken);
	}
}
