using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QIParser.Models;
using System.IO;

namespace QIParser.Utils
{
	public class QipTxtReader
	{
		//public static IcqAccount ReadAccount(string fileName)
		//{

		//}

		private static string separatorFrom = "--------------------------------------<-";
		private static string separatorTo = "-------------------------------------->-";

		public static List<QHFMessage> ReadMessages(string fileName)
		{
			List<QHFMessage> result = new List<QHFMessage>();
			string uin = Path.GetFileNameWithoutExtension(fileName);
			QHFMessage msg = null;

			using (FileStream fs = new FileStream(fileName, FileMode.Open))
			using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding(1251)))
			{
				string nextLine = sr.ReadLine();
				while (nextLine != null)
				{
					if (nextLine == separatorFrom)
					{
						msg = new QHFMessage();
						msg.ID = result.Count + 1;
						msg.IsMy = false;
						msg.Time = ParseDateTime(sr.ReadLine());

						nextLine = sr.ReadLine();
					}
					else if (nextLine == separatorTo)
					{
						msg = new QHFMessage();
						msg.ID = result.Count + 1;
						msg.IsMy = true;
						msg.Time = ParseDateTime(sr.ReadLine());

						nextLine = sr.ReadLine();
					}
					else
					{
						string previousLine = nextLine;
						nextLine = sr.ReadLine();

						if (nextLine == separatorFrom || nextLine == separatorTo || nextLine == null)
						{
							result.Add(msg);
						}
						else
						{
							if (msg.Text != null)
								msg.Text += Environment.NewLine;

							msg.Text += previousLine;
						}
					}
				}
			}

			return result;
		}

		internal static DateTime ParseDateTime(string line)
		{
			string dateTime = line.Substring(line.IndexOf('(') + 1, 19);
			return Convert.ToDateTime(dateTime);
		}
	}
}
