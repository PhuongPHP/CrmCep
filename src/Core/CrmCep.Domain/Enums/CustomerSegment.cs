namespace CrmCep.Domain.Enums;

/// <summary>
/// Defines the specific microfinance customer target segments served by CEP.
/// </summary>
public enum CustomerSegment
{
    /// <summary>
    /// Factory and industrial zone workers (Cong nhan lao dong).
    /// </summary>
    Worker = 1,

    /// <summary>
    /// Small retail merchants and street vendors (Tieu thuong buon ban nho).
    /// </summary>
    MicroMerchant = 2,

    /// <summary>
    /// Low-income freelance and self-employed laborers (Lao dong tu do).
    /// </summary>
    Freelancer = 3
}
