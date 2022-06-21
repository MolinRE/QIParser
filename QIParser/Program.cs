using QIParser.Models;
using QIParser.Utils;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace QIParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
            //string fileName = @"C:\Users\Selecty\Documents\QIP Infium\Profiles\newomeg\History\InfICQ_405844575.qhf";

            var fileNames = Directory.GetFiles(@"C:\Users\Selecty\Documents\QIP Infium\Profiles\newomeg\History");

            foreach (var fileName in fileNames)
            {
                using var reader = new QHFReader(fileName);

                string outputFileName = @"C:\Users\Selecty\dev\qip\" + CleanFileName(reader.Nick) + ".txt";
                
                using var fs = new FileStream(outputFileName, FileMode.Create);
                using var sw = new StreamWriter(fs, Encoding.UTF8);

                HistoryWriter.WriteHeader(sw.WriteLine, reader);
            
                var msg = new QHFMessage();

                while (reader.GetNextMessage(msg))
                {
                    HistoryWriter.WriteBody(sw.WriteLine, msg, "Омегыч", reader.Nick);
                }
                
                Console.WriteLine(outputFileName);
            }
        }

        private static object CleanFileName(string readerNick)
        {
            var sb = new StringBuilder();

            foreach (var ch in readerNick)
            {
                sb.Append(Path.GetInvalidFileNameChars().Contains(ch) ? '_' : ch);
            }

            return sb.ToString();
        }
    }
}
