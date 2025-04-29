namespace TestProject.Gameteq.Common.Interfaces;

/// <summary>
/// Application user
/// </summary>
public interface IUser
{
    /// <summary>
    /// User nickname
    /// </summary>
    string Name { get; }
    /// <summary>
    /// User nickname color
    /// </summary>
    ConsoleColor Color { get; }
}
