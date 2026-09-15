using Microsoft.EntityFrameworkCore;
using ProcessTracker.API.Data;

var builder = WebApplication.CreateBuilder(args);

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ProcessTracker.API.Data.ProcessTrackerDbContext>();
    var validator = scope.ServiceProvider.GetRequiredService<ProcessTracker.API.Validators.IDynamicProcessValidator>();
    await ProcessTracker.API.Data.DbSeeder.SeedAsync(context, validator);
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
