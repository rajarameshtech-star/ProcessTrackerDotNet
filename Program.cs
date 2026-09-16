using Microsoft.EntityFrameworkCore;
using ProcessTracker.API.Data;
using ProcessTracker.API.Middlewares;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // Configure Entity Framework Core with SQL Server
    builder.Services.AddDbContext<ProcessTrackerDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Repositories
    builder.Services.AddScoped<ProcessTracker.API.Repositories.IProjectRepository, ProcessTracker.API.Repositories.ProjectRepository>();
    builder.Services.AddScoped<ProcessTracker.API.Repositories.IApplicationRepository, ProcessTracker.API.Repositories.ApplicationRepository>();
    builder.Services.AddScoped<ProcessTracker.API.Repositories.IProcessDefinitionRepository, ProcessTracker.API.Repositories.ProcessDefinitionRepository>();
    builder.Services.AddScoped<ProcessTracker.API.Repositories.IProcessDefinitionProjectMappingRepository, ProcessTracker.API.Repositories.ProcessDefinitionProjectMappingRepository>();
    builder.Services.AddScoped<ProcessTracker.API.Repositories.IProcessFieldRepository, ProcessTracker.API.Repositories.ProcessFieldRepository>();
    builder.Services.AddScoped<ProcessTracker.API.Repositories.IServiceItemRepository, ProcessTracker.API.Repositories.ServiceItemRepository>();
    builder.Services.AddScoped<ProcessTracker.API.Repositories.IProcessRecordRepository, ProcessTracker.API.Repositories.ProcessRecordRepository>();

    // Validators
    builder.Services.AddScoped<ProcessTracker.API.Validators.IDynamicProcessValidator, ProcessTracker.API.Validators.DynamicProcessValidator>();

    // Services
    builder.Services.AddScoped<ProcessTracker.API.Services.IProjectService, ProcessTracker.API.Services.ProjectService>();
    builder.Services.AddScoped<ProcessTracker.API.Services.IApplicationService, ProcessTracker.API.Services.ApplicationService>();
    builder.Services.AddScoped<ProcessTracker.API.Services.IProcessDefinitionService, ProcessTracker.API.Services.ProcessDefinitionService>();
    builder.Services.AddScoped<ProcessTracker.API.Services.IProcessFieldService, ProcessTracker.API.Services.ProcessFieldService>();
    builder.Services.AddScoped<ProcessTracker.API.Services.IProcessDefinitionProjectMappingService, ProcessTracker.API.Services.ProcessDefinitionProjectMappingService>();
    builder.Services.AddScoped<ProcessTracker.API.Services.IServiceItemService, ProcessTracker.API.Services.ServiceItemService>();
    builder.Services.AddScoped<ProcessTracker.API.Services.IProcessRecordService, ProcessTracker.API.Services.ProcessRecordService>();

    var app = builder.Build();

    // Serilog Request Logging (built-in) or custom middleware
    // Serilog Request Logging or custom metrics
    app.UseMiddleware<GlobalLoggingMiddleware>();
    
    // Global exception handling mapped seamlessly inside logging bounds
    app.UseMiddleware<GlobalExceptionMiddleware>();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProcessTrackerDbContext>();
        var validator = scope.ServiceProvider.GetRequiredService<ProcessTracker.API.Validators.IDynamicProcessValidator>();
        await DbSeeder.SeedAsync(context, validator);
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    app.UseAuthorization();
    app.MapControllers();
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
