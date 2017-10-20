using System.Collections.Generic;
using System.Xml.Serialization;
using QIParser.Models;

namespace QIParser.Models
{
	[XmlRoot(ElementName = "smses")]
	public class SmsBackup
	{
		[XmlElement(ElementName = "sms")]
		public List<ShortMessage> Messages { get; set; }
		[XmlAttribute(AttributeName = "count")]
		public string Count { get; set; }
		[XmlAttribute(AttributeName = "backup_set")]
		public string Backup_set { get; set; }
		[XmlAttribute(AttributeName = "backup_date")]
		public string Backup_date { get; set; }
	}
}
