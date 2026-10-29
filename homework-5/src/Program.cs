using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SmartCampus.Attendance;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AttendanceDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<AttendanceService>();

// OpenTelemetry: именно он создаёт Activity.Current для каждого HTTP-запроса
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("attendance-service"))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter());

var app = builder.Build();
app.MapControllers();
app.Run();
