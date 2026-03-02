using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using TaskFlow.API.Middleware;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Services;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Repositories;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, config) =>
        config.ReadFrom.Configuration(context.Configuration)
              .WriteTo.Console());

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddDbContext<TaskFlowDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

    var jwtSecret = builder.Configuration["Jwt:Secret"]
        ?? throw new InvalidOperationException("Jwt:Secret is not configured.");

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
            };
        });

    builder.Services.AddAuthorization();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins("http://localhost")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
    builder.Services.AddScoped<ITaskRepository, TaskRepository>();
    builder.Services.AddScoped<ITaskCommentRepository, TaskCommentRepository>();

    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IProjectService, ProjectService>();
    builder.Services.AddScoped<ITaskService, TaskService>();
    builder.Services.AddScoped<ITaskCommentService, TaskCommentService>();

    builder.Services.AddValidatorsFromAssemblyContaining<Program>();

    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
            options.SuppressModelStateInvalidFilter = true);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskFlow API", Version = "v1" });
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token."
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                []
            }
        });
    });

    var app = builder.Build();

    app.UseMiddleware<GlobalExceptionMiddleware>();

    app.UseCors("AllowFrontend");

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapGet("/api/health", async (TaskFlowDbContext db) =>
    {
        var canConnect = await db.Database.CanConnectAsync();
        return Results.Ok(new
        {
            success = true,
            data = new { database = canConnect },
            message = canConnect ? "All systems operational" : "Database unreachable",
            errors = Array.Empty<string>()
        });
    });

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
