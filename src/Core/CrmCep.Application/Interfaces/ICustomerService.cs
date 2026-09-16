using CrmCep.Application.Common;
using CrmCep.Application.DTOs;

namespace CrmCep.Application.Interfaces;

/// <summary>
/// Business service interface for customer lifecycle management and reporting.
/// </summary>
public interface ICustomerService
{
    Task<ApiResponse<PaginatedResult<CustomerDto>>> GetCustomersAsync(CustomerFilterDto filter);
    Task<ApiResponse<CustomerDto>> GetCustomerByIdAsync(Guid id);
    Task<ApiResponse<CustomerDto>> CreateCustomerAsync(CreateCustomerDto request);
    Task<ApiResponse<CustomerDto>> UpdateCustomerAsync(Guid id, UpdateCustomerDto request);
    Task<ApiResponse<bool>> DeleteCustomerAsync(Guid id);
    Task<ApiResponse<DashboardSummaryDto>> GetDashboardSummaryAsync();
    Task<byte[]> ExportToExcelAsync(CustomerFilterDto filter);
}
