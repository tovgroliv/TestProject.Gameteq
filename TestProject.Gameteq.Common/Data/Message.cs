using TestProject.Gameteq.Common.Enums;
using TestProject.Gameteq.Common.Interfaces;

namespace TestProject.Gameteq.Common.Data;

/// <summary>
/// The class that describes chat messages.
/// </summary>
/// <param name="text">Message content</param>
/// <param name="time">Date of message sending</param>
/// <param name="user">The user who sent the message</param>
public sealed class Message(string text, DateTime time, User user) : ReceiveBase, IMessage
{
    /// <summary>
    /// Message content
    /// </summary>
    public string Text { get; } = text;
    /// <summary>
    /// Date of message sending
    /// </summary>
    public DateTime Time { get; } = time;
    /// <summary>
    /// The user who sent the message
    /// </summary>
    public User User { get; } = user;
    /// <summary>
    /// Type of packet received
    /// </summary>
    public override ReceiveTypeEnum Type { get; set; } = ReceiveTypeEnum.Message;
}
