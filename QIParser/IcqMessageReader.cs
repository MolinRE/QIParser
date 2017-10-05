using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QIParser
{
	public class IcqMessageReader
	{
		private readonly int me;

		public IcqMessageReader()
		{
			me = 395060975;
		}

		public List<IcqMessage> Read(string htmlFilePath)
		{
			int uin = 0;
			string uinStr = "";
			string nick = "";
			var result = new List<IcqMessage>();

			using (var fs = new FileStream(htmlFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (var sr = new StreamReader(fs, Encoding.GetEncoding(1251)))
				{
					sr.SkipLine(2);
					uinStr = sr.ReadLine().Substring(4);
					uin = Convert.ToInt32(uinStr);
					nick = sr.ReadLine().Substring(4);
					sr.SkipLine();

					IcqMessage msg = new IcqMessage();
					string line = sr.ReadLine();
					while (line != null)
					{
						DateTime sent;
						if (line.Trim() == "")
						{
							result.Add(msg);
							msg = new IcqMessage();
							line = sr.ReadLine();
							continue;
						}

						string sender = line.Substring(0, 1);

						if (sender == "Я" && line.Length > 3 && DateTime.TryParse(line.Substring(3), out sent))
						{
							msg.Sent = sent;
							msg.From = me;
							msg.To = uin;
						}
						else
						{
							var senderSplit = line.Split(new char[] { '(', ')' });
							if (senderSplit.Length == 3 && senderSplit[1] == uinStr)
							{
								msg.Sent = Convert.ToDateTime(senderSplit[2].Substring(1));
								msg.From = uin;
								msg.To = me;
							}
							else
							{
								if (msg.MessageText != null)
								{
									msg.MessageText += Environment.NewLine;
								}
								msg.MessageText += line.TrimStart();
							}
						}

						line = sr.ReadLine();
					}
				}
			}

			return result;
		}
	}
}
