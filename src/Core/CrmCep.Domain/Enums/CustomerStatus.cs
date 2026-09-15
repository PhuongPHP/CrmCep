namespace CrmCep.Domain.Enums;

/// <summary>
/// Represents the lifecycle and operational status of a customer in CEP Microfinance.
/// </summary>
public enum CustomerStatus
{
    /// <summary>
    /// Active customer eligible for financial transactions and credit products.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Temporarily suspended customer account.
    /// </summary>
    Suspended = 2,

    /// <summary>
    /// Locked customer account due to compliance or severe delinquency.
    /// </summary>
    Locked = 3
}
