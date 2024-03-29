﻿using QIParser.Models;
using System.Text;

namespace QIParser.Utils;

public class QipTxtReader
{
    //public static IcqAccount ReadAccount(string fileName)
    //{

    //}

    private static readonly string separatorFrom = "--------------------------------------<-";
    private static readonly string separatorTo = "-------------------------------------->-";

    public static List<QHFMessage> ReadMessages(string fileName)
    {
        var result = new List<QHFMessage>();
        var uin = Path.GetFileNameWithoutExtension(fileName);
        QHFMessage msg = null;

        using (var fs = new FileStream(fileName, FileMode.Open))
        using (var sr = new StreamReader(fs, Encoding.GetEncoding(1251)))
        {
            var nextLine = sr.ReadLine();

            while (nextLine != null)
                if (nextLine == separatorFrom)
                {
                    msg = new QHFMessage();
                    msg.ID = result.Count + 1;
                    msg.IsMy = false;
                    msg.Time = ParseDateTime(sr.ReadLine());

                    nextLine = sr.ReadLine();
                }
                else if (nextLine == separatorTo)
                {
                    msg = new QHFMessage();
                    msg.ID = result.Count + 1;
                    msg.IsMy = true;
                    msg.Time = ParseDateTime(sr.ReadLine());

                    nextLine = sr.ReadLine();
                }
                else
                {
                    var previousLine = nextLine;
                    nextLine = sr.ReadLine();

                    if (nextLine == separatorFrom || nextLine == separatorTo || nextLine == null)
                    {
                        result.Add(msg);
                    }
                    else
                    {
                        if (msg?.Text != null)
                        {
                            msg.Text += Environment.NewLine;
                            msg.Text += previousLine;
                        }
                    }
                }
        }

        return result;
    }

    internal static DateTime ParseDateTime(string line)
    {
        var dateTime = line.Substring(line.IndexOf('(') + 1, 19);
        return Convert.ToDateTime(dateTime);
    }
}
