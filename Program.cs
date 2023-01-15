using QIParser.Models;
using QIParser.Utils;
using System.Text;

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

foreach (var fileName in historyFiles)
    try
    {
        ConvertFile(fileName, outputFolderPath, userName);
    }
    catch (Exception e)
    {
        Console.WriteLine("Ошибка при конвертации файла:");
        Console.WriteLine(fileName);
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

    while (reader.GetNextMessage(msg))
    {
        HistoryWriter.WriteBody(sw.WriteLine, msg, userName, reader.Nick);
    }

    Console.WriteLine(outputFileName);
}

string CleanFileName(string readerNick)
{
    var sb = new StringBuilder();

    foreach (var ch in readerNick) sb.Append(Path.GetInvalidFileNameChars().Contains(ch) ? '_' : ch);

    return sb.ToString();
}
