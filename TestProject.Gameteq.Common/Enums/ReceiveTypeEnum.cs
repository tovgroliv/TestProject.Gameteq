namespace TestProject.Gameteq.Common.Enums;

/// <summary>
/// Packet type
/// </summary>
public enum ReceiveTypeEnum
{
    /// <summary>
    /// Authorization packet
    /// </summary>
    SignUp,
    /// <summary>
    /// User message packet
    /// </summary>
    Message,
    /// <summary>
    /// Packet of user status change
    /// </summary>
    UserStatus,  
}
