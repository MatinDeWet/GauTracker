namespace Messaging.Core;
public class MessagingOptions
{
    public const string SectionName = "MessageBroker";

    public string Host { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }
}
