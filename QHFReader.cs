using QIParser.Models;
using QIParser.Utils;
using System.Diagnostics;
using System.Text;

public class QHFReader : IDisposable
{
    private readonly BinaryReaderBE br;
    private readonly Encoding encoding = Encoding.UTF8;

    private readonly FileStream fs;

    public QHFReader(string path)
    {
        fs = new FileStream(path, FileMode.Open);
        br = new BinaryReaderBE(fs, encoding);

        fs.Position = 4;
        Size = br.ReadInt32();
        fs.Position = 34;
        MsgCount = br.ReadInt32();
        fs.Position = 44;
        var uinLength = br.ReadInt16();
        Uin = encoding.GetString(br.ReadBytes(uinLength));
        var nickLength = br.ReadInt16();
        Nick = encoding.GetString(br.ReadBytes(nickLength));
    }

    public string Nick { get; set; }

    public string Uin { get; set; }

    public int MsgCount { get; set; }

    public int Size { get; set; }

    public void Dispose()
    {
        br.Close();
        fs.Close();
    }

    public bool GetNextMessage(QHFMessage msg)
    {
        if (fs.Position >= fs.Length - 24)
        {
            return false;
        }

        byte[] msgBytes = null;
        //Trace.WriteLine("Block start: " + fs.Position);
        msg.Signature = br.ReadInt16();

        if (msg.SignatureMismatch)
        {
            if (br.ReadByte() == 1)
            {
                // В предыдущем сообщении были ошибки и структура съехала.
                msg.Signature = 1;
            }
            else
            {
                // Сдвигаем позицию на исходную для достоверности отладки.
                fs.Position -= 1;
                Trace.WriteLine($"{Uin} ({Nick}). Signature mismatch. (sign={msg.Signature})");
                return true;
            }
        }

        var size = br.ReadInt32();
        var endOfBlock = size + fs.Position;

        try
        {
            msg.ID = GetNextBlock();

            if (msg.ID == 28)
            {
                Console.WriteLine();
            }
            
            msg.Time = UnixTimeStampToDateTime(GetNextBlock());
            msg.IsMy = GetNextBlock() > 0;
            var msgLength = GetNextBlock();

            if (msgLength < 0)
            {
                fs.Position -= 4;
                msgBytes = ReadBytesUntilNextMessage(msg.ID + 1);
                msgLength = msgBytes.Length;
            }
            else
            {
                if (msgLength > size)
                {
                    fs.Position -= 4;
                    msgLength = br.ReadInt16();

                    if (msgLength > size)
                    {
                        fs.Position -= 2;
                        msgLength = br.ReadByte();
                    }
                }

                msgBytes = br.ReadBytes(msgLength);
            }

            msg.Text = encoding.GetString(DecodeBytes(msgBytes));

            return true;
        }
        catch (ArgumentException ex)
        {
            Trace.WriteLine(ex.Message);

            if (fs.Position == fs.Length)
            {
                msg.Text = "Сообщение потеряно.";
            }
            return true;
        }
    }

    public int GetSize(short size)
    {
        switch (size)
        {
            case 1:
                return br.ReadByte();
            case 2:
                return br.ReadInt16();
            case 4:
                return br.ReadInt32();
            default:
                return 0;
        }
    }

    public int GetNextBlock()
    {
        var result = 0;
        var blockType = (QHFMessageType) br.ReadInt16();

        //Trace.WriteLine(blockType);
        switch (blockType)
        {
            case QHFMessageType.MessageOnline:
            case QHFMessageType.MessageDateTime:
            case QHFMessageType.MessageAuthRequest:
            case QHFMessageType.MessageAuthRequestOk:
            case QHFMessageType.MessageAddRequest:
            case QHFMessageType.MessageOffline:
                result = GetSize(br.ReadInt16());
                break;
            case QHFMessageType.MessageIsMy:
                if (br.ReadInt16() == 3)
                    result = br.ReadByte();
                else
                    result = GetSize(br.ReadInt16());

                break;
            default:
                Trace.WriteLine("Unknown message type: " + blockType);
                result = GetSize(br.ReadInt16());
                break;
        }

        return result;
    }

    public byte[] DecodeBytes(byte[] array)
    {
        for (var i = 0; i < array.Length; i++) array[i] = DecodeByte(array[i], i);

        return array;
    }

    public byte DecodeByte(byte b, int index)
    {
        return (byte) (byte.MaxValue - b - index - 1);
    }

    public byte[] ReadBytesUntilNextMessage(int nextMessageId)
    {
        var result = new List<byte>();

        if (fs.Length - fs.Position > 14)
        {
            var sign = br.ReadInt16();
            fs.Position += 8;
            var msgId = br.ReadInt32();

            while (sign != 1 || msgId != nextMessageId)
            {
                fs.Position -= 14;
                var nextByte = br.ReadByte();
                result.Add(nextByte);

                sign = br.ReadInt16();
                fs.Position += 8;
                msgId = br.ReadInt32();
            }

            fs.Position -= 14;
        }

        return result.ToArray();
    }

    public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime;
    }
}
