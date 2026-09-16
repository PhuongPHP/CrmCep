namespace CrmCep.Application.DTOs;

/// <summary>
/// Aggregated metric statistics for the CRM dashboard overview cards.
/// </summary>
public class DashboardSummaryDto
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int SuspendedCustomers { get; set; }
    public int LockedCustomers { get; set; }
    public int NewCustomersThisMonth { get; set; }
}
