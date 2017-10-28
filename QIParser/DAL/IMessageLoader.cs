using System;
using System.Collections.Generic;

namespace QIParser.DAL
{
	public interface IMessageLoader<T1, T2>
	{
		int AddRange(IEnumerable<T1> range);
		IEnumerable<T1> GetRange(T2 account, DateTime from, DateTime to);
		T1 GetEarliest(T2 account);
		T1 GetLatest(T2 account);
	}
}