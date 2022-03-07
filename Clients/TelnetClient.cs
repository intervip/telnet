using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Intervip.Telnet.Clients;

public class TelnetClient : TcpClient
{
	public IPAddress Address { get; set; }
	public int Port { get; set; }

	public TelnetClient(IPAddress address, int port) : base()
	{
		Address = address;
		Port = port;
	}

	public async Task ConnectAsync(CancellationToken cancellationToken = default)
	{
		await ConnectAsync(new IPEndPoint(Address, Port), cancellationToken);
	}

	public async Task WriteAsync(byte[] bytes, CancellationToken cancellationToken = default)
	{
		NetworkStream stream = GetStream();

		if (stream.CanWrite)
		{
			await stream.WriteAsync(bytes, cancellationToken);
		}
	}

	public async Task<byte[]> ReadAsync(string text, CancellationToken cancellationToken = default)
	{
		NetworkStream stream = GetStream();

		if (stream.CanRead)
		{
			IEnumerable<byte> bytes = Array.Empty<byte>();

			if (cancellationToken == CancellationToken.None)
			{
				var tokenSource = new CancellationTokenSource();
				tokenSource.CancelAfter(30000);
				cancellationToken = tokenSource.Token;
			}

			while (cancellationToken.IsCancellationRequested is false)
			{
				var data = new byte[ReceiveBufferSize];
				Array.Resize(ref data, await stream.ReadAsync(data, cancellationToken));
				bytes = bytes.Concat(data);

				if (PatternFound(bytes, text, out var index))
				{
					var byteArray = bytes.ToArray();

					if (byteArray.Length > index)
					{
						Array.Resize(ref byteArray, index + text.Length);
					}

					return byteArray;
				}
			}
		}

		return Array.Empty<byte>();
	}

	private static bool PatternFound(IEnumerable<byte> source, string text, out int index)
	{
		index = Encoding.UTF8.GetString(source.ToArray()).IndexOf(text);
		return index != -1;
	}

	public async Task DisconnectAsync()
	{
		await Client.DisconnectAsync(true);
	}
}
