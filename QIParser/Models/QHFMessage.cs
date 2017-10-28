using System;

namespace QIParser.Models
{
	public class QHFMessage
	{
		public Int32 ID { get; set; }
		public Int16 Signature { get; set; }
		public bool IsMy { get; set; }
		public string Text { get; set; }
		public DateTime Time { get; set; }
		public Int16 MsgBlockType { get; set; }

		public bool SignatureMismatch
		{
			get { return Signature != 1; }
		}

		public bool Equals(IcqMessage msg)
		{
			if (msg.IsMy == this.IsMy)
			{
				if (DateTimeEquals(msg.DateTime, this.Time))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Сранивает две даты вплоть до секунды, поскольку большая точность не имеет значения в контексте ICQ сообщения.
		/// </summary>
		/// <param name="dt1">Первый из сравниваемых объектов.</param>
		/// <param name="dt2">Второй из сравниваемых объектов.</param>
		/// <returns></returns>
		public static bool DateTimeEquals(DateTime dt1, DateTime dt2)
		{
			if (dt1.Year == dt2.Year)
			{
				if (dt1.Month == dt2.Month)
				{
					if (dt1.Date == dt2.Date)
					{
						if (dt1.Hour == dt2.Hour)
						{
							if (dt1.Minute == dt2.Minute)
							{
								if (dt1.Second == dt2.Second)
								{
									return true;
								}
							}
						}
					}
				}
			}

			return false;
		}

		public override string ToString()
		{
			return Text;
		}
	}
}
