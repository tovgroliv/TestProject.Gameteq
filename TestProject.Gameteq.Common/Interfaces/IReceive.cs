using TestProject.Gameteq.Common.Enums;

namespace TestProject.Gameteq.Common.Interfaces;

/// <summary>
/// Base class for all chat packets
/// </summary>
public interface IReceive
{
    /// <summary>
    /// Type of packet received
    /// </summary>
    ReceiveTypeEnum Type { get; }
}
