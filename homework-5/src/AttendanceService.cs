using Microsoft.EntityFrameworkCore;

namespace SmartCampus.Attendance;

// ---------- Модель (фаза Expand: есть И старое поле, И новые) ----------
public class AttendanceRecord
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public string Status { get; set; } = "present";
    public DateTime MarkedAt { get; set; }

    // СТАРОЕ поле: "Фамилия Имя". Удалим в фазе Contract (03_contract.sql)
    public string StudentName { get; set; } = "";

    // НОВЫЕ поля (появились в 01_expand.sql)
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

// Запрос от клиента. Старые клиенты шлют StudentName, новые - FirstName + LastName
public class MarkAttendanceRequest
{
    public Guid LessonId { get; set; }
    public string Status { get; set; } = "present";
    public string? StudentName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public class AttendanceDbContext : DbContext
{
    public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options) : base(options) { }

    public DbSet<AttendanceRecord> Records => Set<AttendanceRecord>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<AttendanceRecord>(e =>
        {
            e.ToTable("attendance_records");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.LessonId).HasColumnName("lesson_id");
            e.Property(x => x.Status).HasColumnName("status");
            e.Property(x => x.MarkedAt).HasColumnName("marked_at");
            e.Property(x => x.StudentName).HasColumnName("student_name");
            e.Property(x => x.FirstName).HasColumnName("first_name");
            e.Property(x => x.LastName).HasColumnName("last_name");
        });
    }
}

public class AttendanceService
{
    private readonly AttendanceDbContext _db;

    public AttendanceService(AttendanceDbContext db)
    {
        _db = db;
    }

    // ---------- ДВОЙНАЯ ЗАПИСЬ: пишем и в старое, и в новые поля ----------
    public async Task<AttendanceRecord> MarkAsync(MarkAttendanceRequest request)
    {
        string firstName;
        string lastName;

        if (!string.IsNullOrWhiteSpace(request.LastName))
        {
            // Новый клиент прислал уже разделённые поля
            lastName = request.LastName.Trim();
            firstName = (request.FirstName ?? "").Trim();
        }
        else
        {
            // Старый клиент прислал только StudentName ("Иванов Иван") - делим сами
            (lastName, firstName) = SplitName(request.StudentName ?? "");
        }

        var record = new AttendanceRecord
        {
            Id = Guid.NewGuid(),
            LessonId = request.LessonId,
            Status = request.Status,
            MarkedAt = DateTime.UtcNow,

            // старая структура (её ещё читают старые версии сервисов)
            StudentName = $"{lastName} {firstName}".Trim(),

            // новая структура
            FirstName = firstName,
            LastName = lastName
        };

        _db.Records.Add(record);
        await _db.SaveChangesAsync();
        return record;
    }

    public Task<AttendanceRecord?> GetAsync(Guid id) =>
        _db.Records.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

    public async Task<bool> DeleteAsync(Guid id)
    {
        var rows = await _db.Records.Where(r => r.Id == id).ExecuteDeleteAsync();
        return rows > 0;
    }

    // "Иванов Иван Петрович" -> LastName = "Иванов", FirstName = "Иван Петрович"
    public static (string LastName, string FirstName) SplitName(string fullName)
    {
        var parts = fullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var last = parts.Length > 0 ? parts[0] : "";
        var first = parts.Length > 1 ? parts[1] : "";
        return (last, first);
    }
}
