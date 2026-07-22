using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StudentManagement.API.Middleware;
using StudentManagement.Application.Interfaces;
using StudentManagement.Application.Services;
using StudentManagement.Domain.Interfaces;
using StudentManagement.Infrastructure.Data;
using StudentManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Serilog Logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/app_.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting Student Management Web API...");

    // 2. Add Controllers & API Explorer
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // 3. Configure Database (SQL Server with automatic fallback support for seamless running)
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var useInMemoryDb = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        if (useInMemoryDb)
        {
            options.UseInMemoryDatabase("StudentManagementInMemoryDb");
        }
        else
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            });
        }
    });

    // 4. Register Dependency Injection (Repositories & Services)
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    builder.Services.AddScoped<IStudentService, StudentService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

    // 5. Configure JWT Authentication
    var secretKey = builder.Configuration["Jwt:SecretKey"] ?? "ZestIndiaAssessmentSuperSecretSecurityKey2026!";
    var issuer = builder.Configuration["Jwt:Issuer"] ?? "StudentManagementAPI";
    var audience = builder.Configuration["Jwt:Audience"] ?? "StudentManagementApp";

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization();

    // 6. Configure Swagger with JWT Bearer Support
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Student Management System API",
            Version = "v1",
            Description = "Clean Architecture ASP.NET Core Web API with Code-First EF Core, Generic Repository Pattern, JWT Authentication, and Serilog Logging.",
            Contact = new OpenApiContact
            {
                Name = "Technical Candidate",
                Email = "candidate@zestindia.com"
            }
        });

        // Add JWT Bearer Security Scheme
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter JWT Bearer Token to access secured Student API endpoints.\n\nExample: `Bearer eyJhbGciOiJIUzI1Ni...`"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // 7. Enable CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // 8. Auto Database Initialization / Seeding
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            // If using real SQL Server, migrate. If in-memory, ensure created.
            if (dbContext.Database.IsSqlServer())
            {
                dbContext.Database.Migrate();
            }
            else
            {
                dbContext.Database.EnsureCreated();
            }
            Log.Information("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Database migration on startup skipped or failed. Falling back to ensure created.");
            try
            {
                var dbContext = services.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.EnsureCreated();
            }
            catch (Exception innerEx)
            {
                Log.Error(innerEx, "Failed to initialize database.");
            }
        }
    }

    // 9. Middleware Pipeline
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseSerilogRequestLogging();

    // Configure HTTP Request Pipeline
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Management API V1");
        c.DocumentTitle = "Student Management API Documentation";
        c.RoutePrefix = "swagger";
    });

    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.UseRouting();
    app.UseCors("AllowAll");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
