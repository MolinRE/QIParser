using QIParser.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QIParser.DAL
{
	public class IcqMessageLoader
	{
		private static IcqMessageLoader instance;
		private static object _syncLock = new object();

		private readonly string _cnString;

		protected IcqMessageLoader(string cnString)
		{
			_cnString = cnString;
		}

		public static IcqMessageLoader GetInstance(string cnString)
		{
			if (instance == null)
			{
				lock (_syncLock)
				{
					if (instance == null)
						instance = new IcqMessageLoader(cnString);
				}
			}

			return instance;
		}

		public long Add(IcqMessage value)
		{
			long id = 0;

			try
			{
				using (var conn = new SqlConnection(_cnString))
				{
					using (var cmd = new SqlCommand("[dbo].[ICQ_HISTORY_ADD]", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						cmd.Parameters.AddWithValue("@from", value.From);
						cmd.Parameters.AddWithValue("@to", value.To);
						cmd.Parameters.AddWithValue("@date", value.Sent);
						cmd.Parameters.AddWithValue("@text", value.MessageText);

						conn.Open();

						using (var dr = cmd.ExecuteReader())
						{
							if (dr.Read())
							{
								id = dr.GetInt32(0);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			return id;
		}

		public List<IcqMessage> GetAll(int uin)
		{
			var result = new List<IcqMessage>();

			try
			{
				using (var conn = new SqlConnection(_cnString))
				{
					using (var cmd = new SqlCommand("[dbo].[ICQ_HISTORY_GET_ALL]", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@uin", uin);

						conn.Open();

						using (var dr = cmd.ExecuteReader())
						{
							while (dr.Read())
							{
								var msg = new IcqMessage();
								msg.From = dr.GetInt32(dr.GetOrdinal("FROM"));
								msg.To = dr.GetInt32(dr.GetOrdinal("TO"));
								msg.Sent = dr.GetDateTime(dr.GetOrdinal("DATE"));
								msg.MessageText = dr.GetString(dr.GetOrdinal("MESSAGE"));

								result.Add(msg);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			return result;
		}

		public List<IcqMessage> GetRange(int uin, DateTime from, DateTime to)
		{
			var result = new List<IcqMessage>();

			try
			{
				using (var conn = new SqlConnection(_cnString))
				{
					using (var cmd = new SqlCommand("[dbo].[ICQ_HISTORY_GET_RANGE]", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						cmd.Parameters.AddWithValue("@date_from", from);
						cmd.Parameters.AddWithValue("@date_to", to);
						cmd.Parameters.AddWithValue("@uin", uin);

						conn.Open();

						using (var dr = cmd.ExecuteReader())
						{
							while (dr.Read())
							{
								var msg = new IcqMessage();
								msg.From = dr.GetInt32(dr.GetOrdinal("FROM"));
								msg.To = dr.GetInt32(dr.GetOrdinal("TO"));
								msg.Sent = dr.GetDateTime(dr.GetOrdinal("DATE"));
								msg.MessageText = dr.GetString(dr.GetOrdinal("MESSAGE"));

								result.Add(msg);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="uin"></param>
		/// <param name="year">Год.</param>
		/// <param name="month">Месяц (число в диапазоне от 1 до 12).</param>
		/// <returns></returns>
		public List<IcqMessage> GetRangeByMonth(int uin, int year, int month)
		{
			var result = new List<IcqMessage>();
			DateTime from = new DateTime(year, month, 1);
			DateTime to = new DateTime(year, month, DateTime.DaysInMonth(year, month));

			try
			{
				using (var conn = new SqlConnection(_cnString))
				{
					using (var cmd = new SqlCommand("[dbo].[ICQ_HISTORY_GET_RANGE]", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						cmd.Parameters.AddWithValue("@date_from", from);
						cmd.Parameters.AddWithValue("@date_to", to);
						cmd.Parameters.AddWithValue("@uin", uin);

						conn.Open();

						using (var dr = cmd.ExecuteReader())
						{
							while (dr.Read())
							{
								var msg = new IcqMessage();
								msg.From = dr.GetInt32(dr.GetOrdinal("FROM"));
								msg.To = dr.GetInt32(dr.GetOrdinal("TO"));
								msg.Sent = dr.GetDateTime(dr.GetOrdinal("DATE"));
								msg.MessageText = dr.GetString(dr.GetOrdinal("MESSAGE"));

								result.Add(msg);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			return result;
		}
	}
}
