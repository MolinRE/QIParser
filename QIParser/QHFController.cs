﻿using QIParser.DAL;
using QIParser.Models;
using QIParser.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QIParser
{
	public class QHFController
	{
		private readonly string _cnString;

		public QHFController(string cnString)
		{
			_cnString = cnString;
		}

		public int LoadToDb(string qhfFilePath, out int totalMsgs)
		{
			var messages = new List<IcqMessage>();
			var account = new IcqAccount();

			using (QHFReader reader = new QHFReader(qhfFilePath))
			{
				account.Nickname = reader.Nick;
				account.Uin = Convert.ToInt32(reader.Uin);

				var message = new QHFMessage();
				while (reader.GetNextMessage(message))
				{
					messages.Add(new IcqMessage(account.Uin, message.IsMy, message.Time, message.Text));
				}
			}

			messages = messages.OrderBy(ks => ks.DateTime).ToList();

			totalMsgs = messages.Count;

			var loader = IcqMessageLoader.GetInstance(_cnString);
			IEnumerable<IcqMessage> uploadedMessages = null;

			var accountLoader = IcqAccountLoader.GetInstance(_cnString);
			if (accountLoader.Get(account.Uin) == null)
			{
				accountLoader.Add(account);
			}
			else
			{
				if (messages.First().DateTime < loader.GetLatest(account).DateTime)
				{
					if (messages.Last().DateTime > loader.GetEarliest(account).DateTime)
					{
						uploadedMessages = loader.GetRange(account, messages.First().DateTime, messages.Last().DateTime);
					}
				}
			}

			int uploadCount = 0;

			if (uploadedMessages == null)
			{
				uploadCount = loader.AddRange(messages);
			}
			else
			{
				var selectedMessages = messages.Where(m => !uploadedMessages.Any(um => um.Equals(m))).ToList();
				uploadCount = loader.AddRange(selectedMessages);
			}

			return uploadCount;
		}

		public int LoadTxtToDb(string txtFilePath)
		{
			if (Path.GetExtension(txtFilePath) != ".txt")
				throw new IOException($"Файл \"{Path.GetFileName(txtFilePath)}\" не является текстовым.");

			var account = new IcqAccount();
			var qipMessages = QipTxtReader.ReadMessages(txtFilePath);
			account.Uin = Convert.ToInt32(Path.GetFileNameWithoutExtension(txtFilePath));
			account.Nickname = "";

			List<IcqMessage> messages = qipMessages
				.Select(message => new IcqMessage(account.Uin, message.IsMy, message.Time, message.Text))
				.ToList();

			var loader = IcqMessageLoader.GetInstance(_cnString);
			IEnumerable<IcqMessage> uploadedMessages = null;

			var accountLoader = IcqAccountLoader.GetInstance(_cnString);
			if (accountLoader.Get(account.Uin) == null)
			{
				accountLoader.Add(account);
			}
			else
			{
				if (messages.First().DateTime < loader.GetLatest(account).DateTime)
				{
					if (messages.Last().DateTime > loader.GetEarliest(account).DateTime)
					{
						uploadedMessages = loader.GetRange(account, messages.First().DateTime, messages.Last().DateTime);
					}
				}
			}

			int uploadCount = 0;

			if (uploadedMessages == null)
			{
				uploadCount = loader.AddRange(messages);
			}
			else
			{
				var selectedMessages = messages.Where(m => !uploadedMessages.Any(um => um.Equals(m)));
				uploadCount = loader.AddRange(selectedMessages);
			}

			return uploadCount;
		}
	}
}
