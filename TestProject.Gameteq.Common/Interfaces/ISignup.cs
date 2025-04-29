using TestProject.Gameteq.Common.Data;

namespace TestProject.Gameteq.Common.Interfaces;

/// <summary>
/// User authorization interface
/// </summary>
public interface ISignup
{
    /// <summary>
    /// User who authorize
    /// </summary>
    User User { get; }
}
