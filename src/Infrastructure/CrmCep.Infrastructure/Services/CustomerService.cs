using ClosedXML.Excel;
using CrmCep.Application.Common;
using CrmCep.Application.DTOs;
using CrmCep.Application.Interfaces;
using CrmCep.Domain.Entities;
using CrmCep.Domain.Enums;
using CrmCep.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CrmCep.Infrastructure.Services;

/// <summary>
/// Implements business logic for customer lifecycle management, search, soft deletion, and reporting.
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _context;

    public CustomerService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PaginatedResult<CustomerDto>>> GetCustomersAsync(CustomerFilterDto filter)
    {
        var query = _context.Customers.AsNoTracking().AsQueryable();

        // Real-time search by FullName or PhoneNumber (case-insensitive)
        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            var keyword = filter.Keyword.Trim().ToLower();
            query = query.Where(c => c.FullName.ToLower().Contains(keyword) || c.PhoneNumber.Contains(keyword));
        }

        // Filter by operational status
        if (filter.Status.HasValue)
        {
            query = query.Where(c => c.Status == filter.Status.Value);
        }

        // Filter by customer segment
        if (filter.Segment.HasValue)
        {
            query = query.Where(c => c.Segment == filter.Segment.Value);
        }

        var totalCount = await query.CountAsync();

        var pageIndex = filter.PageIndex < 1 ? 1 : filter.PageIndex;
        var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(c => MapToDto(c))
            .ToListAsync();

        var paginatedResult = new PaginatedResult<CustomerDto>(items, totalCount, pageIndex, pageSize);
        return ApiResponse<PaginatedResult<CustomerDto>>.Ok(paginatedResult);
    }

    public async Task<ApiResponse<CustomerDto>> GetCustomerByIdAsync(Guid id)
    {
        var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null)
        {
            return ApiResponse<CustomerDto>.Fail("Không tìm thấy thông tin khách hàng.");
        }

        return ApiResponse<CustomerDto>.Ok(MapToDto(customer));
    }

    public async Task<ApiResponse<CustomerDto>> CreateCustomerAsync(CreateCustomerDto request)
    {
        // Enforce unique customer code
        var codeExists = await _context.Customers.IgnoreQueryFilters()
            .AnyAsync(c => c.CustomerCode.ToLower() == request.CustomerCode.Trim().ToLower());

        if (codeExists)
        {
            return ApiResponse<CustomerDto>.Fail($"Mã khách hàng '{request.CustomerCode}' đã tồn tại trong hệ thống.");
        }

        // Check if phone number is already registered for an active customer
        var phoneExists = await _context.Customers
            .AnyAsync(c => c.PhoneNumber == request.PhoneNumber.Trim());

        if (phoneExists)
        {
            return ApiResponse<CustomerDto>.Fail($"Số điện thoại '{request.PhoneNumber}' đã được đăng ký bởi khách hàng khác.");
        }

        var customer = new Customer
        {
            CustomerCode = request.CustomerCode.Trim().ToUpper(),
            FullName = request.FullName.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            DateOfBirth = request.DateOfBirth,
            Segment = request.Segment,
            Status = request.Status,
            Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();

        return ApiResponse<CustomerDto>.Ok(MapToDto(customer), "Thêm mới khách hàng thành công.");
    }

    public async Task<ApiResponse<CustomerDto>> UpdateCustomerAsync(Guid id, UpdateCustomerDto request)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null)
        {
            return ApiResponse<CustomerDto>.Fail("Không tìm thấy khách hàng cần cập nhật.");
        }

        // Check phone duplicate with another customer
        var phoneExists = await _context.Customers
            .AnyAsync(c => c.PhoneNumber == request.PhoneNumber.Trim() && c.Id != id);

        if (phoneExists)
        {
            return ApiResponse<CustomerDto>.Fail($"Số điện thoại '{request.PhoneNumber}' đã được sử dụng bởi khách hàng khác.");
        }

        customer.FullName = request.FullName.Trim();
        customer.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        customer.PhoneNumber = request.PhoneNumber.Trim();
        customer.DateOfBirth = request.DateOfBirth;
        customer.Segment = request.Segment;
        customer.Status = request.Status;
        customer.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
        customer.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        customer.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ApiResponse<CustomerDto>.Ok(MapToDto(customer), "Cập nhật thông tin khách hàng thành công.");
    }

    public async Task<ApiResponse<bool>> DeleteCustomerAsync(Guid id)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy khách hàng cần xóa.");
        }

        // Apply soft delete to maintain historical audit logs
        customer.IsDeleted = true;
        customer.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Xóa khách hàng thành công.");
    }

    public async Task<ApiResponse<DashboardSummaryDto>> GetDashboardSummaryAsync()
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var total = await _context.Customers.CountAsync();
        var active = await _context.Customers.CountAsync(c => c.Status == CustomerStatus.Active);
        var suspended = await _context.Customers.CountAsync(c => c.Status == CustomerStatus.Suspended);
        var locked = await _context.Customers.CountAsync(c => c.Status == CustomerStatus.Locked);
        var newThisMonth = await _context.Customers.CountAsync(c => c.CreatedAt >= startOfMonth);

        var summary = new DashboardSummaryDto
        {
            TotalCustomers = total,
            ActiveCustomers = active,
            SuspendedCustomers = suspended,
            LockedCustomers = locked,
            NewCustomersThisMonth = newThisMonth
        };

        return ApiResponse<DashboardSummaryDto>.Ok(summary);
    }

    public async Task<byte[]> ExportToExcelAsync(CustomerFilterDto filter)
    {
        var query = _context.Customers.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            var keyword = filter.Keyword.Trim().ToLower();
            query = query.Where(c => c.FullName.ToLower().Contains(keyword) || c.PhoneNumber.Contains(keyword));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(c => c.Status == filter.Status.Value);
        }

        if (filter.Segment.HasValue)
        {
            query = query.Where(c => c.Segment == filter.Segment.Value);
        }

        var customers = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Danh Sach Khach Hang");
        var headers = new[] { "STT", "Mã KH", "Họ và Tên", "Số Điện Thoại", "Email", "Ngày Sinh", "Phân Loại", "Trạng Thái", "Địa Chỉ", "Ghi Chú", "Ngày Tạo" };

        // Set title header merged across table columns
        var titleRange = worksheet.Range(1, 1, 1, headers.Length);
        titleRange.Merge();
        titleRange.Value = "TỔ CHỨC TÀI CHÍNH VI MÔ CEP - PHÒNG CNTT";
        titleRange.Style.Font.Bold = true;
        titleRange.Style.Font.FontSize = 14;
        titleRange.Style.Font.FontColor = XLColor.FromHtml("#00695C");
        titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        worksheet.Row(1).Height = 25;

        var subTitleRange = worksheet.Range(2, 1, 2, headers.Length);
        subTitleRange.Merge();
        subTitleRange.Value = $"DANH SÁCH KHÁCH HÀNG CRM (Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm})";
        subTitleRange.Style.Font.Italic = true;
        subTitleRange.Style.Font.FontSize = 10;
        subTitleRange.Style.Font.FontColor = XLColor.FromHtml("#546E7A");
        subTitleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        worksheet.Row(2).Height = 20;

        // Table headers (Row 4)
        worksheet.Row(4).Height = 26;
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cell(4, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontSize = 11;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#00695C");
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        // Populate data
        int row = 5;
        int stt = 1;
        foreach (var c in customers)
        {
            worksheet.Row(row).Height = 22;

            var sttCell = worksheet.Cell(row, 1);
            sttCell.Value = stt++;
            sttCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var codeCell = worksheet.Cell(row, 2);
            codeCell.Value = c.CustomerCode;
            codeCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            codeCell.Style.Font.Bold = true;

            var nameCell = worksheet.Cell(row, 3);
            nameCell.Value = c.FullName;

            var phoneCell = worksheet.Cell(row, 4);
            phoneCell.Value = c.PhoneNumber;
            phoneCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var emailCell = worksheet.Cell(row, 5);
            emailCell.Value = c.Email ?? "";

            var dobCell = worksheet.Cell(row, 6);
            dobCell.Value = c.DateOfBirth.ToString("dd/MM/yyyy");
            dobCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var segCell = worksheet.Cell(row, 7);
            segCell.Value = GetSegmentName(c.Segment);
            segCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var statusCell = worksheet.Cell(row, 8);
            statusCell.Value = GetStatusName(c.Status);
            statusCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var addrCell = worksheet.Cell(row, 9);
            addrCell.Value = c.Address ?? "";

            var noteCell = worksheet.Cell(row, 10);
            noteCell.Value = c.Notes ?? "";

            var createdCell = worksheet.Cell(row, 11);
            createdCell.Value = c.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            createdCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Zebra striping for even rows
            if (stt % 2 == 1)
            {
                worksheet.Range(row, 1, row, headers.Length).Style.Fill.BackgroundColor = XLColor.FromHtml("#F5F9F8");
            }

            row++;
        }

        // Apply grid styling
        var tableRange = worksheet.Range(4, 1, row - 1, headers.Length);
        tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        tableRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#B2DFDB");
        tableRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#E0E0E0");
        tableRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        // Adjust column widths based on table contents (avoiding unmerged title blowup)
        worksheet.Columns(1, headers.Length).AdjustToContents(4, row - 1);
        worksheet.Column(1).Width = 8; // Compact STT column

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static CustomerDto MapToDto(Customer c) => new()
    {
        Id = c.Id,
        CustomerCode = c.CustomerCode,
        FullName = c.FullName,
        Email = c.Email,
        PhoneNumber = c.PhoneNumber,
        DateOfBirth = c.DateOfBirth,
        Segment = c.Segment,
        Status = c.Status,
        Address = c.Address,
        Notes = c.Notes,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };

    private static string GetStatusName(CustomerStatus status) => status switch
    {
        CustomerStatus.Active => "Đang hoạt động",
        CustomerStatus.Suspended => "Tạm ngưng",
        CustomerStatus.Locked => "Đã khóa",
        _ => "Không xác định"
    };

    private static string GetSegmentName(CustomerSegment segment) => segment switch
    {
        CustomerSegment.Worker => "Công nhân lao động",
        CustomerSegment.MicroMerchant => "Tiểu thương buôn bán",
        CustomerSegment.Freelancer => "Lao động tự do",
        _ => "Khác"
    };
}
