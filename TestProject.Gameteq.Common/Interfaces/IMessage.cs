using TestProject.Gameteq.Common.Data;

namespace TestProject.Gameteq.Common.Interfaces;

/// <summary>
/// Message from user
/// </summary>
public interface IMessage
{
    /// <summary>
    /// Message text
    /// </summary>
    string Text { get; }
    /// <summary>
    /// Message sending time
    /// </summary>
    DateTime Time { get; }
    /// <summary>
    /// The user who sent the message
    /// </summary>
    User User { get; }
}
