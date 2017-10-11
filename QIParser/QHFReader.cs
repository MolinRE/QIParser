using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QIParser.Models;
using QIParser.Utils;

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

		public QHFMessage GetNextMessage()
		{
			var sign = br.ReadInt16();
			var size = br.ReadInt32();

			var msg = new QHFMessage();
			fs.Position += 4;
			msg.ID = br.ReadInt32();
			fs.Position += 4;
			msg.Time = UnixTimeStampToDateTime(br.ReadInt32());
			fs.Position += 4;
			msg.IsMy = br.ReadBoolean();
			fs.Position += 4;
			var textLength = br.ReadInt32();
			var msgBytes = br.ReadBytes(textLength);
			for (int i = 0; i < textLength; i++)
			{
				msgBytes[i] = (byte)((255 - msgBytes[i] - i) - 1);
			}

			msg.Text = encoding.GetString(msgBytes);

			return msg;
		}

		public bool GetNextMessage(QHFMessage msg)
		{
			if (fs.Position >= fs.Length - 24)
				return false;

			var sign = br.ReadInt16();
			var size = br.ReadInt32();
			
			fs.Position += 4;
			msg.ID = br.ReadInt32();
			fs.Position += 4;
			msg.Time = UnixTimeStampToDateTime(br.ReadInt32());
			fs.Position += 4;
			msg.IsMy = br.ReadBoolean();
			fs.Position += 4;
			var textLength = br.ReadInt32();
			var msgBytes = br.ReadBytes(textLength);
			for (int i = 0; i < textLength; i++)
			{
				msgBytes[i] = (byte)((255 - msgBytes[i] - i) - 1);
			}

			msg.Text = encoding.GetString(msgBytes);

			return true;
		}

		public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
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
