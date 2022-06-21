namespace QIParser.Models;

public enum QHFMessageType : short
{
    MessageOnline = 1,
    MessageDateTime = 2,
    MessageIsMy = 3,
    MessageAuthRequest = 5,
    MessageAddRequest = 6,
    MessageOffline = 13,
    MessageAuthRequestOk = 14
}
