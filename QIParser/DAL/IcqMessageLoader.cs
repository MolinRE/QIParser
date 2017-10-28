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

		public int AddRange(IEnumerable<IcqMessage> range)
		{
			int rowCount = 0;

			try
			{
				using (var conn = new SqlConnection(_cnString))
				{
					using (var cmd = new SqlCommand("[dbo].[ICQ_HISTORY_ADD]", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						conn.Open();

						foreach (var item in range)
						{
							cmd.Parameters.AddWithValue("@uin", item.ContactUin);
							cmd.Parameters.AddWithValue("@is_my", item.IsMy);
							cmd.Parameters.AddWithValue("@date", item.DateTime);
							cmd.Parameters.AddWithValue("@text", item.Text);

							using (var dr = cmd.ExecuteReader())
							{
								if (dr.Read())
								{
									item.Id = dr.GetInt32(0);
									rowCount++;
								}
							}
							cmd.Parameters.Clear();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			return rowCount;
		}

		//public int Add(IcqMessage item)
		//{
		//	int id = 0;

		//	try
		//	{
		//		using (var conn = new SqlConnection(_cnString))
		//		{
		//			using (var cmd = new SqlCommand("[dbo].[ICQ_HISTORY_ADD]", conn))
		//			{
		//				cmd.CommandType = CommandType.StoredProcedure;

		//				cmd.Parameters.AddWithValue("@uin", item.ContactUin);
		//				cmd.Parameters.AddWithValue("@is_my", item.IsMy);
		//				cmd.Parameters.AddWithValue("@date", item.DateTime);
		//				cmd.Parameters.AddWithValue("@text", item.Text);

		//				conn.Open();

		//				using (var dr = cmd.ExecuteReader())
		//				{
		//					if (dr.Read())
		//					{
		//						id = dr.GetInt32(0);
		//					}
		//				}
		//			}
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine(ex);
		//	}

		//	return id;
		//}

		public IcqMessage GetEarliest(IcqAccount account)
		{
			IcqMessage result = null;

			try
			{
				using (var conn = new SqlConnection(_cnString))
				{
					using (var cmd = new SqlCommand("[dbo].[ICQ_HISTORY_GET_EARLIEST]", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@uin", account.Uin);

						conn.Open();

						using (var dr = cmd.ExecuteReader())
						{
							if (dr.Read())
							{
								result = new IcqMessage();
								result.Id = dr.GetInt32(dr.GetOrdinal("MESSAGE_ID"));
								result.ContactUin = dr.GetInt32(dr.GetOrdinal("CONTACT_UIN"));
								result.IsMy = dr.GetBoolean(dr.GetOrdinal("IS_MY"));
								result.DateTime = dr.GetDateTime(dr.GetOrdinal("DATE"));
								result.Text = dr.GetString(dr.GetOrdinal("TEXT"));
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

		public IcqMessage GetLatest(IcqAccount account)
		{
			IcqMessage result = null;

			try
			{
				using (var conn = new SqlConnection(_cnString))
				{
					using (var cmd = new SqlCommand("[dbo].[ICQ_HISTORY_GET_LATEST]", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@uin", account.Uin);

						conn.Open();

						using (var dr = cmd.ExecuteReader())
						{
							if (dr.Read())
							{
								result = new IcqMessage();
								result.Id = dr.GetInt32(dr.GetOrdinal("MESSAGE_ID"));
								result.ContactUin = dr.GetInt32(dr.GetOrdinal("CONTACT_UIN"));
								result.IsMy = dr.GetBoolean(dr.GetOrdinal("IS_MY"));
								result.DateTime = dr.GetDateTime(dr.GetOrdinal("DATE"));
								result.Text = dr.GetString(dr.GetOrdinal("TEXT"));
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

		public IEnumerable<IcqMessage> GetRange(IcqAccount account, DateTime from, DateTime to)
		{
			var result = new List<IcqMessage>();

			try
			{
				using (var conn = new SqlConnection(_cnString))
				{
					using (var cmd = new SqlCommand("[dbo].[ICQ_HISTORY_GET_RANGE]", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@uin", account.Uin);
						cmd.Parameters.AddWithValue("@from", from);
						cmd.Parameters.AddWithValue("@to", to);

						conn.Open();

						using (var dr = cmd.ExecuteReader())
						{
							while (dr.Read())
							{
								var msg = new IcqMessage();
								msg.Id = dr.GetInt32(dr.GetOrdinal("MESSAGE_ID"));
								msg.ContactUin = dr.GetInt32(dr.GetOrdinal("CONTACT_UIN"));
								msg.IsMy = dr.GetBoolean(dr.GetOrdinal("IS_MY"));
								msg.DateTime = dr.GetDateTime(dr.GetOrdinal("DATE"));
								msg.Text = dr.GetString(dr.GetOrdinal("TEXT"));

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
