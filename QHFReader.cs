using QIParser.Models;
using QIParser.Utils;
using System.Diagnostics;
using System.Text;

public class QHFReader : IDisposable
{
    private const string Sign = "QHF";
    private static readonly Encoding Encoding = Encoding.UTF8;
    
    private readonly BinaryReaderBE br;
    private readonly FileStream fs;
    public string Nick { get; }

    public string Uin { get; }

    public int MsgCount { get; }

    public int Size { get; }

    public QHFVersion Version { get; }

    public QHFReader(string path)
    {
        fs = new FileStream(path, FileMode.Open);
        br = new BinaryReaderBE(fs, Encoding);
        
        // Заголовок
        Debug.WriteLine("Чтение заголовка");

        var sign = string.Concat(br.ReadChars(3));
        if (sign != Sign)
        {
            Console.WriteLine($"Подпись не совпадает! Ожидаемая {Sign}, реальная {sign}");
        }
        Version = (QHFVersion) br.ReadByte();
        Debug.WriteLine($"Версия файла истории: {Version.ToString()}");
        
        Size = br.ReadInt32();
        // Неизвестно
        br.ReadBytes(10);
        // Неизвестно
        br.ReadBytes(16);
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
        
        // Блок данных UIN и Nickname
        var uinLength = br.ReadInt16();
        Uin = Encoding.GetString(br.ReadBytes(uinLength));
        var nickLength = br.ReadInt16();
        Nick = Encoding.GetString(br.ReadBytes(nickLength));
    }
    
    private void ReadMessage(QHFMessage msg, QHFVersion version)
    {
        var start = fs.Position;

        msg.Signature = br.ReadInt16();
        var blockSize = br.ReadInt32();

        // Тип поля с id сообщения - Всегда 1
        br.ReadInt16();
        // Размер поля - всегда 4
        br.ReadInt16();
        // Номер сообщения
        msg.ID = br.ReadInt32();
        // Тип поля с датой сообщения - всегда 2
        br.ReadInt16();
        // Размер поля -- всегда 4
        br.ReadInt16();
        // Дата и время отправки в Unix time
        var time = br.ReadInt32();
        msg.Time = DateTimeHelper.UnixTimeStampToDateTime(time);

        // Тип поля с ?? - всегда 3
        br.ReadInt16();
        // Само ?? -- всегда 3
        br.ReadInt16();
        // Входящее или исходящее
        msg.IsMy = br.ReadBoolean();
        // Тип поля = 0x0f
        br.ReadInt16();
        // Размер поля = 0x04  
        br.ReadInt16();
        // Размер самого сообщения

        var msgSize = version == QHFVersion.QipInfiumOrHigher ? br.ReadInt32() : br.ReadInt16();
        var end = fs.Position;
        var text = br.ReadBytes(msgSize);

        DecodeBytes(text);

        // Почему-то требуется дополнительная кодировка
        if (version == QHFVersion.Unknown)
        {
            DecodeBytes(text);
        }

        msg.Text = Encoding.GetString(text);

        var diff = end - start;

        // Почему 6 -- неизвестно
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

    private void DecodeBytes(byte[] array)
    {
        for (var i = 0; i < array.Length; i++)
        {
            array[i] = (byte) (byte.MaxValue - array[i] - i - 1);
        }
    }

    public void Dispose()
    {
        br.Close();
        fs.Close();
    }
}
