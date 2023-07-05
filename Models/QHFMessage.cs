namespace QIParser.Models;

public class QHFMessage
{
    public int ID { get; set; }

    public short Signature { get; set; }

    public bool IsMy { get; set; }

    public string Text { get; set; }

    public DateTime Time { get; set; }

    // public short MsgBlockType { get; set; }
    //
    // public bool SignatureMismatch => Signature != 1;

    public QHFMessage()
    {
        
    }

    public QHFMessage(QHFMessage msg)
    {
        ID = msg.ID;
        Signature = msg.Signature;
        IsMy = msg.IsMy;
        Text = msg.Text;
        Time = msg.Time;
    }

    public bool Same(QHFMessage msg) => msg.IsMy == IsMy && DateTimeEquals(msg.Time, Time);

    /// <summary>
    /// Сранивает две даты вплоть до секунды, поскольку большая точность не имеет значения в контексте ICQ сообщения.
    /// </summary>
    /// <param name="dt1">Первый из сравниваемых объектов.</param>
    /// <param name="dt2">Второй из сравниваемых объектов.</param>
    /// <returns></returns>
    private static bool DateTimeEquals(DateTime dt1, DateTime dt2)
    {
        if (dt1.Year != dt2.Year) return false;
        if (dt1.Month != dt2.Month) return false;
        if (dt1.Date != dt2.Date) return false;
        if (dt1.Hour != dt2.Hour) return false;
        if (dt1.Minute != dt2.Minute) return false;
        return dt1.Second == dt2.Second;
    }

    public override string ToString()
    {
        return Text;
    }
}
