using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace SmartCampus.Attendance.Tests;

// Production Sanity-тест: запускается СРАЗУ после выкладки v1.9.0 на боевой стенд.
// Адрес берётся из переменной окружения, например: SANITY_BASE_URL=https://campus.example.ru
public class ProductionSanityTests
{
    private static readonly HttpClient Http = new()
    {
        BaseAddress = new Uri(
            Environment.GetEnvironmentVariable("SANITY_BASE_URL") ?? "http://localhost:8080"),
        Timeout = TimeSpan.FromSeconds(10)
    };

    // id тестовой пары, которой нет в реальном расписании
    private static readonly Guid TestLessonId = Guid.Parse("00000000-0000-0000-0000-00000000c103");

    private record AttendanceDto(Guid Id, string StudentName, string? FirstName, string? LastName);

    [Fact]
    public async Task MarkAttendance_AfterRelease_SavesNewAndOldFields()
    {
        Guid? createdId = null;
        try
        {
            // 1. Создаём тестовую отметку (боевой сценарий)
            var response = await Http.PostAsJsonAsync("/api/attendance", new
            {
                lessonId = TestLessonId,
                status = "present",
                firstName = "Тест",
                lastName = "Санитарный"
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<AttendanceDto>();
            Assert.NotNull(created);
            createdId = created!.Id;

            // 2. Читаем запись обратно и проверяем оба формата (двойная запись работает)
            var loaded = await Http.GetFromJsonAsync<AttendanceDto>($"/api/attendance/{createdId}");
            Assert.NotNull(loaded);
            Assert.Equal("Санитарный", loaded!.LastName);
            Assert.Equal("Тест", loaded.FirstName);
            Assert.Equal("Санитарный Тест", loaded.StudentName);
        }
        finally
        {
            // 3. CLEANUP: мгновенно удаляем тестовую запись, даже если проверки упали
            if (createdId is not null)
            {
                var del = await Http.DeleteAsync($"/api/attendance/{createdId}");
                Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);
            }
        }
    }
}
