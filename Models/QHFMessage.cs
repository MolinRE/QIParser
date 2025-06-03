namespace QIParser.Models;

public class QHFMessage
{
    public int ID { get; set; }

    public short Signature { get; set; }

    public bool IsMy { get; set; }

    public string Text { get; set; } = null!;

    public DateTime Time { get; set; }

    public override string ToString()
    {
        return Text;
    }
}
