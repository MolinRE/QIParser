using QIParser.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace QIParser.Utils;

public class HtmlWriter
{
    private static readonly Regex LinkRx = new Regex(@"(https?:\/\/|www).+?(\s|\Z)", RegexOptions.Compiled);
    private List<string> Links { get; set; }

    public StringBuilder _content;

    public HtmlWriter()
    {
        Links = new List<string>();
        _content = new StringBuilder(File.ReadAllText(@"C:\Users\Selecty\dev\personal\QIParser\Templates\History.html"));
    }

    public void WriteAll(QHFReader reader, string userName)
    {
        var sb = new StringBuilder();
        sb.Append($"<p>History size: {HistoryWriter.SizeTextRepresentation(reader.Size)} ({reader.Size} bytes)</p>");
        sb.Append($"<p>Message count: {reader.MsgCount}></p>");
        sb.Append($"<p>UIN: {reader.Uin}</p>");
        sb.Append($"<p>Nickname: {reader.Nick}</p>");

        _content.Replace("@title", $"Переписка с {reader.Nick} ({reader.Uin})");
        _content.Replace("@header", sb.ToString());
        sb.Clear();
        
        var msg = new QHFMessage();
        int count = 0;
        while (reader.GetNextMessage(msg) /*&& count++ < 100*/)
        {
            WriteBody(s => sb.Append(s), msg, userName, reader.Nick);
            sb.AppendLine();
        }

        _content.Replace("@history", sb.ToString());
        sb.Clear();

        foreach (var link in Links)
        {
            sb.AppendLine($"<p><a href=\"{link}\" target=\"_blank\">{link}</a></p>");
        }

        _content.Replace("@links", sb.ToString());
        sb.Clear();
    }

    private void WriteBody(Action<string> writer, QHFMessage msg, string myNick, string contactNick)
    {
        var color = msg.IsMy ? "blue" : "red";
        var sender = msg.IsMy ? myNick : contactNick;
        writer($"<p><b><font color=\"{color}\">");
        writer($"{sender} [{msg.Time:dd.MM.yyyy HH:mm:ss}]</font></b>");
        writer("<br />");
        writer(msg.Text);
        writer("");
        writer("</p>");

        if (msg.Text.Contains("last"))
        {
            //Console.WriteLine(msg.Text);
        }

        var links = LinkRx.Matches(msg.Text);

        foreach (Match m in links)
        {
            Links.Add(m.Value.Trim());
        }
    }

    static string FormatMessage(string text)
    {
        return LinkRx.Replace(text, link => $"<p><a href=\"{link}\" target=\"_blank\">{link}</a></p>");
    }
}
