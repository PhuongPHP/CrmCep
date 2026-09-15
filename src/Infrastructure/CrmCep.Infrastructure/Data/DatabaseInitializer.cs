using CrmCep.Domain.Entities;
using CrmCep.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CrmCep.Infrastructure.Data;

/// <summary>
/// Initializes database schema and seeds administrative user and sample CRM records.
/// </summary>
public static class DatabaseInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        // Automatically apply pending migrations on application startup
        await context.Database.MigrateAsync();

        // Seed default Administrator user for JWT authentication
        if (!await context.Users.AnyAsync())
        {
            var adminUser = new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                FullName = "Quản Trị Viên Hệ Thống",
                Email = "admin@cep.org.vn",
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }

        // Seed realistic microfinance customer sample records
        if (!await context.Customers.IgnoreQueryFilters().AnyAsync())
        {
            var sampleCustomers = new List<Customer>
            {
                new()
                {
                    CustomerCode = "KH-2026-0001",
                    FullName = "Nguyễn Văn Hùng",
                    Email = "hung.nguyen@gmail.com",
                    PhoneNumber = "0908123456",
                    DateOfBirth = new DateOnly(1988, 5, 14),
                    Segment = CustomerSegment.Worker,
                    Status = CustomerStatus.Active,
                    Address = "Khu Công Nghiệp Tân Bình, P. Tây Thạnh, Q. Tân Phú, TP.HCM",
                    Notes = "Công nhân may mặc, có nhu cầu vay vốn sửa chữa nhà vi mô.",
                    CreatedAt = DateTime.UtcNow.AddMonths(-2)
                },
                new()
                {
                    CustomerCode = "KH-2026-0002",
                    FullName = "Trần Thị Mai",
                    Email = "mai.tran@gmail.com",
                    PhoneNumber = "0912345678",
                    DateOfBirth = new DateOnly(1992, 11, 20),
                    Segment = CustomerSegment.MicroMerchant,
                    Status = CustomerStatus.Active,
                    Address = "Chợ Bà Chiểu, P. 1, Q. Bình Thạnh, TP.HCM",
                    Notes = "Tiểu thương bán tạp hóa nhỏ, lịch sử trả nợ tốt qua các kỳ hạn.",
                    CreatedAt = DateTime.UtcNow.AddMonths(-1)
                },
                new()
                {
                    CustomerCode = "KH-2026-0003",
                    FullName = "Lê Hoàng Nam",
                    Email = "nam.le@gmail.com",
                    PhoneNumber = "0987654321",
                    DateOfBirth = new DateOnly(1995, 3, 8),
                    Segment = CustomerSegment.Freelancer,
                    Status = CustomerStatus.Suspended,
                    Address = "120/45 Huỳnh Tấn Phát, H. Nhà Bè, TP.HCM",
                    Notes = "Tài xế giao hàng công nghệ, tạm hoãn kỳ trả góp tháng này do bệnh.",
                    CreatedAt = DateTime.UtcNow.AddDays(-20)
                },
                new()
                {
                    CustomerCode = "KH-2026-0004",
                    FullName = "Phạm Thị Thu Cúc",
                    Email = "cuc.pham@gmail.com",
                    PhoneNumber = "0933998877",
                    DateOfBirth = new DateOnly(1985, 9, 25),
                    Segment = CustomerSegment.Worker,
                    Status = CustomerStatus.Active,
                    Address = "Khu Chế Xuất Tân Thuận, Q. 7, TP.HCM",
                    Notes = "Công nhân điện tử, tham gia gói tiết kiệm tuần hoàn tích lũy CEP.",
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new()
                {
                    CustomerCode = "KH-2026-0005",
                    FullName = "Võ Văn Kiệt",
                    Email = "kiet.vo@gmail.com",
                    PhoneNumber = "0977112233",
                    DateOfBirth = new DateOnly(1990, 7, 19),
                    Segment = CustomerSegment.MicroMerchant,
                    Status = CustomerStatus.Locked,
                    Address = "Chợ Phạm Văn Hai, Q. Tân Bình, TP.HCM",
                    Notes = "Tài khoản bị khóa do quá hạn liên lạc nhiều lần.",
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                }
            };

            await context.Customers.AddRangeAsync(sampleCustomers);
            await context.SaveChangesAsync();
        }
    }
}
