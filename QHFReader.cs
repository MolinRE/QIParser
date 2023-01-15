using QIParser.Models;
using QIParser.Utils;
using System.Diagnostics;
using System.Text;

public class QHFReader : IDisposable
{
    private readonly BinaryReaderBE br;
    private readonly Encoding encoding = Encoding.UTF8;

    private readonly FileStream fs;
    public string Nick { get; set; }

    public string Uin { get; set; }

    public int MsgCount { get; set; }

    public int Size { get; set; }

    public QHFVersion Version { get; init; }

    public QHFReader(string path)
    {
        fs = new FileStream(path, FileMode.Open);
        br = new BinaryReaderBE(fs, encoding);
        
        // Заголовок
        Debug.WriteLine("Чтение заголовка");

        var sign = string.Concat(br.ReadChars(3));
        if (sign != "QHF")
        {
            Console.WriteLine($"Подпись не совпадает! Ожидаемая QHF, реальная {sign}");
        }
        Version = (QHFVersion) br.ReadByte();
        Debug.WriteLine($"Версия файла истории: {Version.ToString()}");
        
        //fs.Position = 4;
        Size = br.ReadInt32();
        // Неизвестно
        br.ReadBytes(10);
        // Неизвестно
        br.ReadBytes(16);
        // fs.Position = 34;
        // Количество сообщений
        MsgCount = br.ReadInt32();
        // Количество сообщений -- сверка
        var msgCountToVerify = br.ReadInt32();
        if (MsgCount != msgCountToVerify)
        {
            Console.WriteLine($"Количество сообщений не совпадает!\nОжидаемое {msgCountToVerify}, реальное {MsgCount}.");
        }

        // Зарезервировано
        br.ReadInt16();
        
        Debug.WriteLine("Заголовок ОК");
        
        // fs.Position = 44;
        // Блок данных UIN и Nickname
        var uinLength = br.ReadInt16();
        Uin = encoding.GetString(br.ReadBytes(uinLength));
        var nickLength = br.ReadInt16();
        //var bytes = br.ReadBytes(nickLength);
        Nick = encoding.GetString(br.ReadBytes(nickLength));
    }
    
    private void ReadMessage(QHFMessage msg, QHFVersion version)
    {
        var start = fs.Position;

        msg.Signature = br.ReadInt16();
        var blockSize = br.ReadInt32();

        // Тип поля с id сообщения - Всегда 1
        var fieldType = br.ReadInt16();
        // Размер поля - всегда 4
        var fieldSize = br.ReadInt16();
        // Номер сообщения
        msg.ID = br.ReadInt32();
        // Тип поля с датой сообщения - всегда 2
        fieldType = br.ReadInt16();
        // Размер поля -- всегда 4
        fieldSize = br.ReadInt16();
        // Дата и время отправки в Unix time
        var time = br.ReadInt32();
        msg.Time = DateTimeHelper.UnixTimeStampToDateTime(time);

        // Тип поля с ?? - всегда 3
        fieldType = br.ReadInt16();
        // ??? -- всегда 3
        fieldSize = br.ReadInt16();
        // Входящее или исходящее
        msg.IsMy = br.ReadBoolean();
        // Тип поля = 0x0f
        fieldType = br.ReadInt16();
        // Размер поля = 0x04  
        fieldSize = br.ReadInt16();
        // Размер самого сообщения

        int msgSize = version == QHFVersion.QipInfiumOrHigher ? br.ReadInt32() : br.ReadInt16();
        var end = fs.Position;
        var text = br.ReadBytes(msgSize);

        DecodeBytes(text);

        if (version == QHFVersion.Unknown)
        {
            DecodeBytes(text);
        }

        msg.Text = encoding.GetString(text);

        var diff = end - start;

        //Console.WriteLine($"Размер блока сообщения ({blockSize}) = Размер самого сообщения ({msgSize}) + размер QHFRecord (33, {diff}) - 6 ");

        var check = msgSize + diff - 6;

        if (check != blockSize)
        {
            Console.WriteLine($"Размер блока не совпадает (ожидал {blockSize}, получил {check})");
        }
    }

    public bool GetNextMessage(QHFMessage msg)
    {
        if (fs.Position == fs.Length)
        {
            return false;
        }

        ReadMessage(msg, Version);
        return true;
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

    private void DecodeBytes(byte[] array)
    {
        for (var i = 0; i < array.Length; i++)
        {
            array[i] = DecodeByte(array[i], i);
        }
    }

    private byte DecodeByte(byte b, int index) => (byte) (byte.MaxValue - b - index - 1);

    public void Dispose()
    {
        br.Close();
        fs.Close();
    }
}
