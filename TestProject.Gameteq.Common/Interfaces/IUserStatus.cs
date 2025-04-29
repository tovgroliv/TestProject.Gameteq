using TestProject.Gameteq.Common.Data;

namespace TestProject.Gameteq.Common.Interfaces;

/// <summary>
/// User status update interface
/// </summary>
public interface IUserStatus
{
    /// <summary>
    /// User who changed status
    /// </summary>
    User User { get; }
    /// <summary>
    /// Connection status
    /// </summary>
    bool Connected { get; }
}
