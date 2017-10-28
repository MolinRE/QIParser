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
	public class IcqAccountLoader
	{
		private static IcqAccountLoader instance;
		private static object _syncLock = new object();

		private readonly string _cnString;

		protected IcqAccountLoader(string cnString)
		{
			_cnString = cnString;
		}

		public static IcqAccountLoader GetInstance(string cnString)
		{
			if (instance == null)
			{
				lock (_syncLock)
				{
					if (instance == null)
						instance = new IcqAccountLoader(cnString);
				}
			}

			return instance;
		}

		public void Add(IcqAccount account)
		{
			int rowCount = 0;

			try
			{
				using (var conn = new SqlConnection(_cnString))
				{
					using (var cmd = new SqlCommand("[dbo].[ICQ_ACCOUNT_ADD]", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						cmd.Parameters.AddWithValue("@uin", account.Uin);
						cmd.Parameters.AddWithValue("@nick", account.Nickname);

						conn.Open();

						using (var dr = cmd.ExecuteReader())
						{
							if (dr.Read())
							{
								rowCount = dr.GetInt32(dr.GetOrdinal("ROWCOUNT"));
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public IcqAccount Get(int uin)
		{
			IcqAccount result = null;

			try
			{
				using (var conn = new SqlConnection(_cnString))
				{
					using (var cmd = new SqlCommand("[dbo].[ICQ_ACCOUNT_GET_BY_UIN]", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						cmd.Parameters.AddWithValue("@uin", uin);

						conn.Open();

						using (var dr = cmd.ExecuteReader())
						{
							if (dr.Read())
							{
								result = new IcqAccount();
								result.Uin = dr.GetInt32(dr.GetOrdinal("UIN"));
								result.Nickname = dr.GetString(dr.GetOrdinal("NICKNAME"));
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
