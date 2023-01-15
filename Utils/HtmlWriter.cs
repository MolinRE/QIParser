using QIParser.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace QIParser.Utils;

public class LinkInMessage
{
    private int ID { get; init; }

    public string Link { get; set; }

    public bool IsMy { get; init; }

    public DateTime Time { get; init; }

    public LinkInMessage()
    {
        
    }

    public LinkInMessage(QHFMessage message, string link)
    {
        Link = link;
        IsMy = message.IsMy;
        ID = message.ID;
        Time = message.Time;
    }

    public override string ToString()
    {
        return Link;
    }
}
public class HtmlWriter
{
    private static readonly Regex LinkRx = new Regex(@"(https?:\/\/|www).+?(\s|\Z)", RegexOptions.Compiled);
    private List<LinkInMessage> Links { get; set; }

    public StringBuilder _content;

    private DateTime? _from;
    private DateTime? _to;
    private DateTime? _previous;

    private const string dtFormat = "dd.MM.yyyy HH:mm:ss";
    private const string dateFormat = "dd.MM.yyyy";

    private int _count;
    private readonly FileInfo[] _files;
    
    private readonly string[] imageExtensions = {
        ".jpg",
        ".jpeg",
        ".gif",
        ".png",
        ".bmp"
    };

    public HtmlWriter(string filesDirectory)
    {
        Links = new List<LinkInMessage>();
        _content = new StringBuilder(File.ReadAllText(@"C:\Users\Selecty\dev\personal\QIParser\Templates\History.html"));

        if (string.IsNullOrEmpty(filesDirectory))
        {
            _files = Array.Empty<FileInfo>();
        }
        else
        {
            var dir = new DirectoryInfo(filesDirectory);
            _files = dir.GetFiles();
        }
        _count = 0;
    }

    public void WriteAll(QHFReader reader, string userName)
    {
        var sb = new StringBuilder();
        sb.Append($"<p>History size: {HistoryWriter.SizeTextRepresentation(reader.Size)} ({reader.Size} bytes)</p>");
        sb.Append($"<p>Message count: {reader.MsgCount}</p>");
        sb.Append($"<p>UIN: {reader.Uin}</p>");
        sb.Append($"<p>Nickname: {reader.Nick}</p>");

        _content.Replace("@title", $"Переписка с {reader.Nick} ({reader.Uin})");
        _content.Replace("@header", sb.ToString());
        sb.Clear();
        
        var msg = new QHFMessage();
        while (reader.GetNextMessage(msg) /*&& count++ < 100*/)
        {
            WriteBody(s => sb.Append(s), msg, userName, reader.Nick);
            sb.AppendLine();
        }

        _content.Replace("@history", sb.ToString());
        sb.Clear();
        _content.Replace("@dates", $"<p>Time: {_from.Value.ToString(dateFormat)} - {_to.Value.ToString(dateFormat)}</p>");

        foreach (var link in Links)
        {
            var color = link.IsMy ? "blue" : "red";
            var sender = link.IsMy ? userName : reader.Nick;
            
            sb.Append($"<p id=\"{msg.ID}\"><b><font color=\"{color}\">");
            sb.Append($"{sender} [{link.Time.ToString(dtFormat)}]</font></b>");
            sb.Append("<br />");
            sb.Append($"<a href=\"{link.Link}\" target=\"_blank\">{link.Link}</a>");
            sb.Append("");
            sb.Append("</p>");
            sb.AppendLine();
        }

        _content.Replace("@links", sb.ToString());
        sb.Clear();
        
        var media = _files
            .Where(p => p.LastWriteTime >= _from && p.LastWriteTime <= _to)
            .OrderBy(p => p.LastWriteTime)
            .ToArray();
        
        foreach (var image in media.Where(p => IsImage(p.FullName)))
        {
            sb.Append($"<p id=\"{msg.ID}\"><b><font color=\"red\">");
            sb.Append($"{reader.Nick} [{image.LastWriteTime.ToString(dtFormat)}]</font></b>");
            sb.Append("<br />");
            sb.Append($"<span class=\"image-container\">"
                      + $"<a href=\"{image.FullName}\" target=\"_blank\">"
                      + $"<img src=\"{image.FullName}\" alt=\"{Path.GetFileName(image.FullName)}\">"
                      + $"</a>"
                      + $"</span>");
            sb.Append("");
            sb.Append("</p>");
            sb.AppendLine();
        }
        
        _content.Replace("@images", sb.ToString());
        
        sb.Clear();
        
        foreach (var audio in media.Where(p => Path.GetExtension(p.FullName) == ".mp3"))
        {
            sb.Append($"<p id=\"{msg.ID}\"><b><font color=\"red\">");
            sb.Append($"{reader.Nick} [{audio.LastWriteTime.ToString(dtFormat)}]</font></b>");
            sb.Append("<br />");
            sb.Append(Path.GetFileName(audio.FullName));
            sb.Append("<br />");
            sb.Append($"<audio controls><source src=\"{audio.FullName}\" type=\"audio/mpeg\"></audio>");
            sb.Append("");
            sb.Append("</p>");
            sb.AppendLine();
        }

        _content.Replace(" @audio", sb.ToString());
        
        sb.Clear();
    }

    private void WriteBody(Action<string> writer, QHFMessage msg, string myNick, string contactNick)
    {
        var color = msg.IsMy ? "blue" : "red";
        var sender = msg.IsMy ? myNick : contactNick;

        if (_previous.HasValue)
        {
            var media = _files.Where(p => p.LastWriteTime >= _previous && p.LastWriteTime <= msg.Time).ToArray();

            foreach (var image in media.Where(p => IsImage(p.FullName)))
            {
                writer($"<p id=\"{msg.ID}\"><b><font color=\"red\">");
                writer($"{contactNick} [{image.LastWriteTime.ToString(dtFormat)}]</font></b>");
                writer("<br />");
                writer($"<img src=\"{image.FullName}\" alt=\"{Path.GetFileName(image.FullName)}\">");
                writer("");
                writer("</p>");
            }
            
            foreach (var audio in media.Where(p => Path.GetExtension(p.FullName) == ".mp3"))
            {
                writer($"<p id=\"{msg.ID}\"><b><font color=\"red\">");
                writer($"{contactNick} [{audio.LastWriteTime.ToString(dtFormat)}]</font></b>");
                writer("<br />");
                writer(Path.GetFileName(audio.FullName));
                writer("<br />");
                writer($"<audio controls><source src=\"{audio.FullName}\" type=\"audio/mpeg\"></audio>");
                writer("");
                writer("</p>");
            }
        }
        
        writer($"<p id=\"{msg.ID}\"><b><font color=\"{color}\">");
        writer($"{sender} [{msg.Time.ToString(dtFormat)}]</font></b>");
        writer("<br />");
        writer(FormatMessage(msg.Text));
        writer("");
        writer("</p>");

        if (msg.Text.Contains("last"))
        {
            //Console.WriteLine(msg.Text);
        }

        var links = LinkRx.Matches(msg.Text);

        foreach (Match m in links)
        {
            Links.Add(new LinkInMessage(msg, m.Value.Trim()));
        }

        SetDates(msg.Time);
        _previous = msg.Time;
    }

    bool IsImage(string fileName) => imageExtensions.Contains(Path.GetExtension(fileName));


    private void SetDates(DateTime time)
    {
        if (!_to.HasValue)
        {
            _to = time;
        }
        else
        {
            if (time > _to.Value)
            {
                _to = time;
            }
        }

        _from ??= time;
    }

    static string FormatMessage(string text)
    {
        return LinkRx.Replace(text, link => $"<p><a href=\"{link}\" target=\"_blank\">{link}</a></p>");
    }
}
