using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SmartCampus.Attendance.Controllers;

[ApiController]
[Route("api/attendance")]
public class AttendanceController : ControllerBase
{
    private const string JiraIssue = "CAMPUS-103";

    private readonly AttendanceService _service;

    public AttendanceController(AttendanceService service)
    {
        _service = service;
    }

    // Отметить студента на паре
    [HttpPost]
    public async Task<IActionResult> Mark([FromBody] MarkAttendanceRequest request)
    {
        // Activity.Current создаёт OpenTelemetry (ASP.NET Core instrumentation) для каждого запроса.
        var activity = Activity.Current;

        // Тег 1: номер задачи в Jira - по нему трассу можно связать с задачей
        activity?.SetTag("jira.issue", JiraIssue);

        var record = await _service.MarkAsync(request);

        // Тег 2: доменный идентификатор сущности
        activity?.SetTag("campus.attendance_id", record.Id);

        return CreatedAtAction(nameof(GetById), new { id = record.Id }, record);
    }

    // Получить запись по id
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var activity = Activity.Current;
        activity?.SetTag("jira.issue", JiraIssue);
        activity?.SetTag("campus.attendance_id", id);

        var record = await _service.GetAsync(id);
        return record is null ? NotFound() : Ok(record);
    }

    // Удалить запись (нужно для очистки после sanity-теста)
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var activity = Activity.Current;
        activity?.SetTag("jira.issue", JiraIssue);
        activity?.SetTag("campus.attendance_id", id);

        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
