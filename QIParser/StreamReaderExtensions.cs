using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QIParser
{
	public static class StreamReaderExtensions
	{
		public static void SkipLine(this StreamReader sr)
		{
			SkipLine(sr, 1);
		}

		public static void SkipLine(this StreamReader sr, int count)
		{
			for (int i = 0; i < count; i++)
			{
				sr.ReadLine();
			}
		}
	}
}
