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

		public readonly string _cnString;

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
	}
}
