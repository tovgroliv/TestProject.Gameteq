using TestProject.Gameteq.Common.Enums;
using TestProject.Gameteq.Common.Interfaces;

namespace TestProject.Gameteq.Common.Data;

/// <summary>
/// User authorization interface
/// </summary>
/// <param name="user">User who authorize</param>
public class Signup(User user) : ReceiveBase, ISignup
{
    /// <summary>
    /// User who authorize
    /// </summary>
    public User User { get; } = user;
    /// <summary>
    /// Packet type
    /// </summary>
    public override ReceiveTypeEnum Type { get; set; } = ReceiveTypeEnum.SignUp;
}
