namespace QIParser.Models;

public class QhfHeader
{
    public string Nick { get; set; }

    public string Uin { get; set; }

    public int MsgCount { get; set; }

    public int Size { get; set; }

    public QHFVersion Version { get; set; }

    public string FileName { get; set; }
}