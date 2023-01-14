using QIParser.Models;

namespace QIParser.Utils;

public class HistoryWriter
{
    public static void WriteHeader(Action<string> writer, QHFReader reader)
    {
        writer($"History size: {SizeTextRepresentation(reader.Size)} ({reader.Size} bytes)");
        writer($"Message count: {reader.MsgCount}");
        writer($"UIN: {reader.Uin}");
        writer($"Nickname: {reader.Nick}");
        writer("---------------------------------------------------");
    }

    public static void WriteBody(Action<string> writer, QHFMessage msg, string myNick, string contactNick)
    {
        var sender = msg.IsMy ? myNick : contactNick;
        writer($"{sender} [{msg.Time:dd.MM.yyyy HH:mm:ss}]");
        writer(msg.Text);
        writer("");
    }

    public static string SizeTextRepresentation(long size)
    {
        var bytes = "bytes";
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

                    if (fullSize < 1024) bytes = "gb";
                }
            }
        }

        return $"{fullSize:00.##} {bytes}";
    }
}
