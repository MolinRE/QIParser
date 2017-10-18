using System;

namespace QIParser.Models
{
	public class QHFMessage
	{
		public Int32 ID { get; set; }
		public bool IsMy { get; set; }
		public string Text { get; set; }
		public DateTime Time { get; set; }
		public Int16 MsgBlockType { get; set; }

		public override string ToString()
		{
			return Text;
		}
	}
}
