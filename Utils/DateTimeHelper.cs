namespace QIParser.Utils;

public static class DateTimeHelper
{
    private static readonly DateTime UnixStart = new(1970, 1, 1);
    
    public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
    {
        return UnixStart.AddSeconds(unixTimeStamp);
    }
}
