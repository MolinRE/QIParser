using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QIParser
{
    public class IcqMessage
    {
		public int From { get; set; }
		public int To { get; set; }
		public string MessageText { get; set; }
		public DateTime Sent { get; set; }

		public override string ToString()
		{
			return $"{From} ({Sent.ToString("dd.MM.yy hh:mm:ss")}";
		}
	}
}
