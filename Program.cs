using QIParser.Models;
using QIParser.Utils;
using System.Text;

List<QhfInfo> meta;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

Console.WriteLine(@"
QIParser, версия 1.1.0. 
Автор: Константин Комаров.
Описание: Конвертер файлов истории QIP и QIP Infium. Преобразует файл с историей в TXT-файл в кодировке UFT-8.
");

Console.WriteLine("Введите адрес папки, которая содержит файлы истории (*.AHF, *.BHD, *.QHF):");
var historyFolderPath = Console.ReadLine();

if (string.IsNullOrEmpty(historyFolderPath) || !Directory.Exists(historyFolderPath))
{
    Console.WriteLine("Указанная папка не существует.");
    return;
}

Console.Write("Искать файлы во вложенных папках? [Y/N]: ");
var searchOption = SearchOption.TopDirectoryOnly;
Console.WriteLine();

if (Console.ReadLine()?.ToLower() == "y")
{
    searchOption = SearchOption.AllDirectories;
}

var historyFiles = Directory.GetFiles(historyFolderPath, "*.?hf", searchOption);

if (historyFiles.Length == 0)
{
    Console.WriteLine("Файлов истории не найдено");
    return;
}

meta = new List<QhfInfo>(historyFiles.Length);

Console.WriteLine($"Найдено файлов: {historyFiles.Length}");
var userName = string.Empty;

do
{
    Console.Write("Введите ваш ник: ");
    userName = Console.ReadLine();
} while (string.IsNullOrEmpty(userName));

Console.WriteLine("Введите адрес папки, куда будут сохранены сконвертированные файлы истории:");
var outputFolderPath = Console.ReadLine();

if (string.IsNullOrEmpty(outputFolderPath) || !Directory.Exists(outputFolderPath))
{
    Console.WriteLine("Указанная папка не существует.");
    return;
}

outputFolderPath = Path.Combine(outputFolderPath.TrimEnd(Path.DirectorySeparatorChar), $"Конвертация {DateTime.Now:ddMMyyyy_HHmmss}");
Directory.CreateDirectory(outputFolderPath);

// TODO: У людей менялся номер аськи + объединять квип и аську
foreach (var contact in historyFiles
             .Select(fileName => QHFReader.GetHeader(fileName))
             .GroupBy(s => s.Uin))
{
    var nick = contact.First().Nick;
    string allNicks = nick;
    if (contact.Count() > 1)
    {
        allNicks = $"{string.Join(", ", contact.Select(s => s.Nick).Distinct())}";
    }
    
    Console.WriteLine(allNicks + $": {contact.Count()} файлов");
    
    var files = new List<QhfHeader>();
    var messages = new List<QHFMessage>();

    var counter = 1;
    foreach (var fileName in contact.Select(s => s.FileName))
    {
        Console.Write($"\r{counter++}/{contact.Count()}                                                                                                            ");
        
        try
        {
            GetMessages();
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при чтении файла:");
            Console.WriteLine(fileName);
        }

        void GetMessages()
        {
            var file = QHFReader.GetHeader(fileName);
            if (files.Any(p => p.MsgCount == file.MsgCount && p.Size == file.Size))
            {
                return;
            }

            files.Add(file);

            using var reader = new QHFReader(fileName);

            var msg = new QHFMessage();
            while (reader.GetNextMessage(msg))
            {
                if (!messages.Any(p => p.Same(msg)))
                {
                    messages.Add(new QHFMessage(msg));
                }
            }
        }
    }

    Console.WriteLine();
    
    var fileNameWithoutExtension = CleanFileName(nick);
    var outputFileName = Path.Combine(outputFolderPath, $"{fileNameWithoutExtension}.txt");
    
    if (File.Exists(outputFileName))
    {
        var count = Directory.GetFiles(outputFolderPath, $"{fileNameWithoutExtension}_*.txt").Length;
        fileNameWithoutExtension = fileNameWithoutExtension + "_" + (count + 1);
        outputFileName = Path.Combine(outputFolderPath, $"{fileNameWithoutExtension}.txt");
    }
    
    using var fs = new FileStream(outputFileName, FileMode.Create);
    using var sw = new StreamWriter(fs, Encoding.UTF8);

    //HistoryWriter.WriteHeader(sw.WriteLine, reader);

    foreach (var msg in messages.OrderBy(p => p.Time))
    {
        HistoryWriter.WriteBody(sw.WriteLine, msg, userName, nick);
    }
    
    // try
    // {
    //     ConvertFile(fileName, outputFolderPath, userName);
    // }
    // catch (Exception e)
    // {
    //     Console.WriteLine("Ошибка при конвертации файла:");
    //     Console.WriteLine(fileName);
    // }
}

foreach (var info in meta.Where(p => p.MsgCount > 1).OrderBy(p => p.Nick).ThenBy(p => p.FirstMessage))
{
    Console.WriteLine($"{info.Nick} - с {info.FirstMessage:dd.MM.yyyy} по {info.LastMessage:dd.MM.yyyy}");
}

void ConvertFile(string fileName, string outputFolderPath, string userName)
{
    using var reader = new QHFReader(fileName);

    var fileNameWithoutExtension = CleanFileName(reader.Nick);
    var outputFileName = Path.Combine(outputFolderPath, $"{fileNameWithoutExtension}.txt");

    if (File.Exists(outputFileName))
    {
        var count = Directory.GetFiles(outputFolderPath, $"{fileNameWithoutExtension}_*.txt").Length;
        fileNameWithoutExtension = fileNameWithoutExtension + "_" + (count + 1);
        outputFileName = Path.Combine(outputFolderPath, $"{fileNameWithoutExtension}.txt");
    }

    using var fs = new FileStream(outputFileName, FileMode.Create);
    using var sw = new StreamWriter(fs, Encoding.UTF8);

    HistoryWriter.WriteHeader(sw.WriteLine, reader);

    var msg = new QHFMessage();

    DateTime? firstMessage = null;
    DateTime? lastMessage = null;

    while (reader.GetNextMessage(msg))
    {
        firstMessage ??= msg.Time;
        lastMessage = msg.Time;
        HistoryWriter.WriteBody(sw.WriteLine, msg, userName, reader.Nick);
    }

    meta.Add(new QhfInfo()
    {
        Nick = reader.Nick,
        MsgCount = reader.MsgCount,
        LastMessage = lastMessage.Value,
        FirstMessage = firstMessage.Value
    });

    //Console.WriteLine(outputFileName);
}

string CleanFileName(string readerNick)
{
    var sb = new StringBuilder();

    foreach (var ch in readerNick) sb.Append(Path.GetInvalidFileNameChars().Contains(ch) ? '_' : ch);

    return sb.ToString();
}

class QhfInfo
{
    public DateTime FirstMessage { get; set; }

    public DateTime LastMessage { get; set; }

    public string Nick { get; set; }

    public int MsgCount { get; set; }
}