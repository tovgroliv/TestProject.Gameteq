using TestProject.Gameteq.Common.Enums;
using TestProject.Gameteq.Common.Interfaces;

namespace TestProject.Gameteq.Common.Data;

/// <summary>
/// Base class for all chat packets
/// </summary>
public class ReceiveBase : IReceive
{
    /// <summary>
    /// Type of packet received
    /// </summary>
    public virtual ReceiveTypeEnum Type { get; set; }
}
