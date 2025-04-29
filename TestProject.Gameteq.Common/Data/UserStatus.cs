using TestProject.Gameteq.Common.Enums;
using TestProject.Gameteq.Common.Interfaces;

namespace TestProject.Gameteq.Common.Data;

/// <summary>
/// User status update interface
/// </summary>
/// <param name="user">User who changed status</param>
/// <param name="connected">Connection status</param>
public sealed class UserStatus(User user, bool connected) : ReceiveBase, IUserStatus
{
    /// <summary>
    /// User who changed status
    /// </summary>
    public User User { get; } = user;
    /// <summary>
    /// Connection status
    /// </summary>
    public bool Connected { get; } = connected;
    /// <summary>
    /// Packet type
    /// </summary>
    public override ReceiveTypeEnum Type { get; set; } = ReceiveTypeEnum.UserStatus;
}
