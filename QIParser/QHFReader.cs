using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QIParser.Models;
using QIParser.Utils;
using System.Diagnostics;

namespace QIParser
{
	public class QHFReader : IDisposable
	{
		public string Nick { get; set; }
		public string UIN { get; set; }
		public Int32 MsgCount { get; set; }
		public Int32 Size { get; set; }

		private FileStream fs;
		private BinaryReaderBE br;
		private Encoding encoding = Encoding.UTF8;

		public QHFReader(string path)
		{
			fs = new FileStream(path, FileMode.Open);
			br = new BinaryReaderBE(fs, encoding);
			
			fs.Position = 4;
			Size = br.ReadInt32();
			fs.Position = 34;
			MsgCount = br.ReadInt32();
			fs.Position = 44;
			Int16 uinLength = br.ReadInt16();
			UIN = encoding.GetString(br.ReadBytes(uinLength));
			Int16 nickLength = br.ReadInt16();
			Nick = encoding.GetString(br.ReadBytes(nickLength));
				
		}

		public bool GetNextMessage(QHFMessage msg)
		{
			if (fs.Position >= fs.Length - 24)
				return false;

			byte[] msgBytes = null;
			Trace.WriteLine("Block start: " + fs.Position);
			var sign = br.ReadInt16();
			if (sign != 1)
			{
				throw new Exception($"Signature mismatch. (sign={sign})");
			}
			var size = br.ReadInt32();
			var endOfBlock = size + fs.Position;

			msg.ID = GetNextBlock();
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

		public Int32 GetSize(Int16 size)
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

		public Int32 GetNextBlock()
		{
			Int32 result = 0;
			var blockType = (QHFMessageType)br.ReadInt16();
			Trace.WriteLine(blockType);
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
					{
						result = br.ReadByte();
					}
					else
					{
						result = GetSize(br.ReadInt16());
					}
					break;
			}

			return result;
		}

		public byte[] DecodeBytes(byte[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = DecodeByte(array[i], i);
			}

			return array;
		}

		public Byte DecodeByte(Byte b, int index)
		{
			return (byte)(Byte.MaxValue - b - index - 1);
		}

		public byte[] ReadBytesUntilNextMessage(int nextMessageId)
		{
			List<byte> result = new List<byte>();
			if (fs.Length - fs.Position > 14)
			{
				Int16 sign = br.ReadInt16();
				fs.Position += 8;
				Int32 msgId = br.ReadInt32();
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

		public static DateTime UnixTimeStampToDateTime(Int32 unixTimeStamp)
		{
			var dateTime = new DateTime(1970, 1, 1);
			dateTime = dateTime.AddSeconds(unixTimeStamp);
			return dateTime;
		}

		public void Dispose()
		{
			br.Close();
			fs.Close();
		}
	}
}
