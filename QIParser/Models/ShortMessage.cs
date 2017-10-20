using System;
using System.Xml.Serialization;

namespace QIParser.Models
{
	public enum MessageType
	{
		Received = 1,
		Sent = 2
	}

	[XmlRoot(ElementName = "sms")]
	public class ShortMessage
	{
		[XmlAttribute(AttributeName = "protocol")]
		public string Protocol { get; set; }
		[XmlAttribute(AttributeName = "address")]
		public string Address { get; set; }
		[XmlAttribute(AttributeName = "date")]
		public string Date { get; set; }
		[XmlAttribute(AttributeName = "type")]
		public int Type { get; set; }
		[XmlAttribute(AttributeName = "subject")]
		public string Subject { get; set; }
		[XmlAttribute(AttributeName = "body")]
		public string Body { get; set; }
		[XmlAttribute(AttributeName = "toa")]
		public string Toa { get; set; }
		[XmlAttribute(AttributeName = "sc_toa")]
		public string Sc_toa { get; set; }
		[XmlAttribute(AttributeName = "service_center")]
		public string ServiceCenter { get; set; }
		[XmlAttribute(AttributeName = "read")]
		public string Read { get; set; }
		[XmlAttribute(AttributeName = "status")]
		public int Status { get; set; }
		[XmlAttribute(AttributeName = "locked")]
		public int Locked { get; set; }
		[XmlAttribute(AttributeName = "date_sent")]
		public long DateSent { get; set; }
		[XmlAttribute(AttributeName = "readable_date")]
		public string ReadableDate { get; set; }
		[XmlAttribute(AttributeName = "contact_name")]
		public string ContactName { get; set; }

		public DateTime GetDateTime => Convert.ToDateTime(ReadableDate);

		public override string ToString()
		{
			return $"{Address} ({ContactName}) [{ReadableDate}]";
		}
	}
}
