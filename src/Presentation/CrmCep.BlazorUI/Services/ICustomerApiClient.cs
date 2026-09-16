using CrmCep.Application.Common;
using CrmCep.Application.DTOs;

namespace CrmCep.BlazorUI.Services;

/// <summary>
/// Client interface for consuming CEP CRM Web API endpoints.
/// </summary>
public interface ICustomerApiClient
{
    Task<ApiResponse<TokenDto>> LoginAsync(LoginDto dto);
    Task LogoutAsync();
    Task<ApiResponse<PaginatedResult<CustomerDto>>> GetCustomersAsync(CustomerFilterDto filter);
    Task<ApiResponse<CustomerDto>> GetCustomerByIdAsync(Guid id);
    Task<ApiResponse<CustomerDto>> CreateCustomerAsync(CreateCustomerDto dto);
    Task<ApiResponse<CustomerDto>> UpdateCustomerAsync(Guid id, UpdateCustomerDto dto);
    Task<ApiResponse<bool>> DeleteCustomerAsync(Guid id);
    Task<ApiResponse<DashboardSummaryDto>> GetDashboardStatsAsync();
    Task<byte[]> ExportExcelAsync(CustomerFilterDto filter);
}
