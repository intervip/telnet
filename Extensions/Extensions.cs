using System.Text;

namespace Intervip.Telnet.Extensions;

public static class Extensions
{
	public static async Task<string> ToString(this Task<byte[]> bytes, Encoding? encoding)
	{
		return encoding is null
			? Encoding.UTF8.GetString(await bytes)
			: encoding.GetString(await bytes);
	}
}
