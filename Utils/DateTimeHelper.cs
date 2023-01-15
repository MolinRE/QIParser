namespace QIParser.Utils;

public class DateTimeHelper
{
    public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1);
        return dateTime.AddSeconds(unixTimeStamp);
    }
}
