﻿using System.Text;

namespace QIParser.Utils;

/// <summary>
///     Считывает примитивные типы данных как двоичные значения в заданной кодировке.
///     Численные значения обрабатываются как Big Endian.
/// </summary>
public class BinaryReaderBE : BinaryReader
{
    /// <summary>
    ///     Инициализирует новый экземпляр класса BinaryReaderBE на основании указанного
    ///     потока с использованием кодировки UTF-8.
    /// </summary>
    /// <param name="input">Входной поток.</param>
    /// <exception cref="ArgumentException">
    ///     Поток не поддерживает чтение, имеет значение null или был закрыт до начала
    ///     операции.
    /// </exception>
    public BinaryReaderBE(Stream input)
        : base(input)
    {
    }

    /// <summary>
    ///     Инициализирует новый экземпляр класса BinaryReaderBE на основе указанного
    ///     потока и кодировки символов.
    /// </summary>
    /// <param name="input">Входной поток.</param>
    /// <param name="encoding">Кодировка символов, которую нужно использовать.</param>
    /// <exception cref="ArgumentException">
    ///     Поток не поддерживает чтение, имеет значение null или был закрыт до начала
    ///     операции.
    /// </exception>
    /// <exception cref="ArgumentNullException">Свойство encoding имеет значение null.</exception>
    public BinaryReaderBE(Stream input, Encoding encoding)
        : base(input, encoding)
    {
    }

    public override int ReadInt32()
    {
        var data = base.ReadBytes(4);
        Array.Reverse(data);
        return BitConverter.ToInt32(data, 0);
    }

    public override uint ReadUInt32()
    {
        var data = base.ReadBytes(4);
        Array.Reverse(data);
        return BitConverter.ToUInt32(data, 0);
    }

    public override short ReadInt16()
    {
        var data = base.ReadBytes(2);
        Array.Reverse(data);
        return BitConverter.ToInt16(data, 0);
    }

    public override ushort ReadUInt16()
    {
        var data = base.ReadBytes(2);
        Array.Reverse(data);
        return BitConverter.ToUInt16(data, 0);
    }

    public override long ReadInt64()
    {
        var data = base.ReadBytes(8);
        Array.Reverse(data);
        return BitConverter.ToInt64(data, 0);
    }

    public override ulong ReadUInt64()
    {
        var data = base.ReadBytes(8);
        Array.Reverse(data);
        return BitConverter.ToUInt64(data, 0);
    }

    // алиас
    public int ReadDWORD() => ReadInt32();
    
    // алиас
    public int ReadWord() => ReadInt16();
}
