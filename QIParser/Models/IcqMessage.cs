using System;

namespace QIParser.Models
{
    public class IcqMessage
    {
		public int Id { get; set; }
		public int ContactUin { get; set; }
		public bool IsMy { get; set; }
		public string Text { get; set; }
		public DateTime DateTime { get; set; }

		public IcqMessage()
		{

		}

		public IcqMessage(int uin, bool isMy, DateTime dateTime, string text)
		{
			ContactUin = uin;
			IsMy = isMy;
			DateTime = dateTime;
			Text = text;
		}

		public override string ToString()
		{
			return (IsMy ? "To: " : "From: ") +  $"{ContactUin} ({DateTime:dd.MM.yyhh:mm:ss}";
		}
	}
}
