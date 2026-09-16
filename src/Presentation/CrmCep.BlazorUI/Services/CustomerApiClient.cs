using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using CrmCep.Application.Common;
using CrmCep.Application.DTOs;
using Microsoft.AspNetCore.Components.Authorization;

namespace CrmCep.BlazorUI.Services;

/// <summary>
/// Handles HTTP communication with the Web API, managing JWT headers and serialization.
/// </summary>
public class CustomerApiClient : ICustomerApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public CustomerApiClient(
        HttpClient httpClient,
        ILocalStorageService localStorage,
        AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    private async Task AttachBearerTokenAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }
        catch
        {
            // Gracefully handle scenario where JS Interop is not yet initialized
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public async Task<ApiResponse<TokenDto>> LoginAsync(LoginDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<TokenDto>>();
            if (result != null && result.Success && result.Data != null)
            {
                await _localStorage.SetItemAsync("authToken", result.Data.AccessToken);
                await _localStorage.SetItemAsync("userName", result.Data.Username);
                await _localStorage.SetItemAsync("userFullName", result.Data.FullName);
                await _localStorage.SetItemAsync("userRole", result.Data.Role);

                if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
                {
                    customProvider.NotifyUserAuthentication(result.Data.AccessToken);
                }
            }
            return result ?? ApiResponse<TokenDto>.Fail("Phản hồi không hợp lệ từ máy chủ.");
        }
        catch (Exception ex)
        {
            return ApiResponse<TokenDto>.Fail($"Lỗi kết nối máy chủ: {ex.Message}");
        }
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("userName");
        await _localStorage.RemoveItemAsync("userFullName");
        await _localStorage.RemoveItemAsync("userRole");

        if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
        {
            customProvider.NotifyUserLogout();
        }
    }

    public async Task<ApiResponse<PaginatedResult<CustomerDto>>> GetCustomersAsync(CustomerFilterDto filter)
    {
        await AttachBearerTokenAsync();
        var query = $"api/customers?keyword={Uri.EscapeDataString(filter.Keyword ?? "")}&pageIndex={filter.PageIndex}&pageSize={filter.PageSize}";
        if (filter.Status.HasValue)
        {
            query += $"&status={(int)filter.Status.Value}";
        }
        if (filter.Segment.HasValue)
        {
            query += $"&segment={(int)filter.Segment.Value}";
        }

        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedResult<CustomerDto>>>(query);
            return response ?? ApiResponse<PaginatedResult<CustomerDto>>.Fail("Không nhận được dữ liệu.");
        }
        catch (Exception ex)
        {
            return ApiResponse<PaginatedResult<CustomerDto>>.Fail($"Lỗi khi tải danh sách: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CustomerDto>> GetCustomerByIdAsync(Guid id)
    {
        await AttachBearerTokenAsync();
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<CustomerDto>>($"api/customers/{id}");
            return response ?? ApiResponse<CustomerDto>.Fail("Không tìm thấy khách hàng.");
        }
        catch (Exception ex)
        {
            return ApiResponse<CustomerDto>.Fail($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CustomerDto>> CreateCustomerAsync(CreateCustomerDto dto)
    {
        await AttachBearerTokenAsync();
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/customers", dto);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<CustomerDto>>();
            return result ?? ApiResponse<CustomerDto>.Fail("Lỗi khi thêm mới khách hàng.");
        }
        catch (Exception ex)
        {
            return ApiResponse<CustomerDto>.Fail($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CustomerDto>> UpdateCustomerAsync(Guid id, UpdateCustomerDto dto)
    {
        await AttachBearerTokenAsync();
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/customers/{id}", dto);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<CustomerDto>>();
            return result ?? ApiResponse<CustomerDto>.Fail("Lỗi khi cập nhật thông tin.");
        }
        catch (Exception ex)
        {
            return ApiResponse<CustomerDto>.Fail($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteCustomerAsync(Guid id)
    {
        await AttachBearerTokenAsync();
        try
        {
            var response = await _httpClient.DeleteAsync($"api/customers/{id}");
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
            return result ?? ApiResponse<bool>.Fail("Lỗi khi xóa khách hàng.");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Fail($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<DashboardSummaryDto>> GetDashboardStatsAsync()
    {
        await AttachBearerTokenAsync();
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<DashboardSummaryDto>>("api/customers/dashboard-stats");
            return response ?? ApiResponse<DashboardSummaryDto>.Fail("Không thể lấy dữ liệu thống kê.");
        }
        catch (Exception ex)
        {
            return ApiResponse<DashboardSummaryDto>.Fail($"Lỗi: {ex.Message}");
        }
    }

    public async Task<byte[]> ExportExcelAsync(CustomerFilterDto filter)
    {
        await AttachBearerTokenAsync();
        var query = $"api/customers/export-excel?keyword={Uri.EscapeDataString(filter.Keyword ?? "")}";
        if (filter.Status.HasValue) query += $"&status={(int)filter.Status.Value}";
        if (filter.Segment.HasValue) query += $"&segment={(int)filter.Segment.Value}";

        var response = await _httpClient.GetAsync(query);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }
}
