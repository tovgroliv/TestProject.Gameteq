using TestProject.Gameteq.Common.Interfaces;

namespace TestProject.Gameteq.Common.Data;

/// <summary>
/// Application user
/// </summary>
/// <param name="name">User nickname</param>
/// <param name="color">User nickname color</param>
public class User(string name, ConsoleColor color) : IUser
{
    /// <summary>
    /// User nickname
    /// </summary>
    public string Name { get; } = name;
    /// <summary>
    /// User nickname color
    /// </summary>
    public ConsoleColor Color { get; } = color;
}
