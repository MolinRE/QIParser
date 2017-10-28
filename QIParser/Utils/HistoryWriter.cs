using System;
using QIParser.Models;

namespace QIParser.Utils
{
	public class HistoryWriter
	{
		public static void WriteHeader(Action<string> writer, QHFReader reader)
		{
			writer.Invoke($"History size: {SizeTextRepresentation(reader.Size)} ({reader.Size} bytes)");
			writer.Invoke($"Message count: {reader.MsgCount}");
			writer.Invoke($"UIN: {reader.Uin}");
			writer.Invoke($"Nickname: {reader.Nick}");
			writer.Invoke("---------------------------------------------------");
		}

		public static void WriteBody(Action<string> writer, QHFMessage msg, string myNick, string contactNick)
		{
			string sender = msg.IsMy ? myNick : contactNick;
			writer.Invoke($"{sender} [{msg.Time:dd.MM.yyyy HH:mm:ss}]");
			writer.Invoke(msg.Text);
			writer.Invoke("");
		}

		public static string SizeTextRepresentation(long size)
		{
			string bytes = "bytes";
			double fullSize = size;
			if (fullSize > 1024)
			{
				fullSize = fullSize / 1024;
				if (fullSize < 1024)
				{
					bytes = "kb";
				}
				else
				{
					fullSize = fullSize / 1024;
					if (fullSize < 1024)
					{
						bytes = "mb";
					}
					else
					{
						fullSize = fullSize / 1024;
						if (fullSize < 1024)
						{
							bytes = "gb";
						}
					}
				}
			}

			return string.Format("{0:00.##} {1}", fullSize, bytes);
		}
	}
}
