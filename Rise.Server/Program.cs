using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Persistence.Triggers;
using Rise.Services.Reservations;
using Rise.Services.TimeSlots;
using Rise.Shared.Reservations;
using Rise.Shared.TimeSlots;
using FluentValidation;
using FluentValidation.AspNetCore;
using Rise.Server.Middleware;
using Serilog.Events;
using Serilog;
using Rise.Shared.Notifications;
using Rise.Services.Notifications;
using Rise.Shared.Users;
using Rise.Services.Users;
using Rise.Services.Boats;
using Rise.Server.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Auth0Net.DependencyInjection;
using Rise.Server.Auth;
using Rise.Services.Auth;
using Microsoft.OpenApi.Models;
using Rise.Shared.Boats;
using Rise.Shared;

try
{
    Log.Information("Starting up Server");
    var builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .CreateLogger();

    builder.Services.AddSerilog();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(options =>
    {
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
        options.EnableAnnotations();
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri($"{builder.Configuration["Auth0:Authority"]}/oauth/token"),
                    AuthorizationUrl = new Uri($"{builder.Configuration["Auth0:Authority"]}/authorize?audience={builder.Configuration["Auth0:Audience"]}"),
                }
            }
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "oauth2"
                    }
                },
                new string[] { "openid" }
            }
        });
    });

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth0:Authority"];
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "buutUserId"
        };
    });

    builder.Services.AddAuth0AuthenticationClient(config =>
    {
        config.Domain = builder.Configuration["Auth0:Authority"]!;
        config.ClientId = builder.Configuration["Auth0:M2MClientId"];
        config.ClientSecret = builder.Configuration["Auth0:M2MClientSecret"];
    });
    builder.Services.AddAuth0ManagementClient().ConfigureHttpClient((httpClient) =>
    {
        httpClient.Timeout = TimeSpan.FromSeconds(200);
    }).AddManagementAccessToken();

    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
        options.UseTriggers(options => options.AddTrigger<EntityBeforeSaveTrigger>());
    });

    builder.Services.AddScoped<INotificationService, NotificationService>();
    builder.Services.AddScoped<IInternalNotificationService, InternalNotificationService>();
    builder.Services.AddScoped<ICruisePeriodService, CruisePeriodService>();
    builder.Services.AddScoped<ITimeSlotService, TimeSlotService>();
    builder.Services.AddScoped<IReservationService, ReservationService>();
    builder.Services.AddScoped<IUserAdminService, UserService>();
    builder.Services.AddScoped<IUserRegisterService, UserService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IBatteryService, BatteryService>();
    builder.Services.AddScoped<IBoatService, BoatService>();

    builder.Services.AddHttpContextAccessor()
                .AddScoped<IAuthContextProvider, HttpContextAuthProvider>();
    builder.Services.AddScoped<BatteryAssignmentService>();

    //validation using fluent validation
    builder.Services.AddValidatorsFromAssemblyContaining<CreateReservationDto.Validator>();
    builder.Services.AddFluentValidationAutoValidation();

    builder.Services.AddLocalization();

    builder.Services.AddHostedService<BatteryAssignmentWorker>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1.0");
        options.OAuthClientId(builder.Configuration["Auth0:BlazorClientId"]);
        options.OAuthClientSecret(builder.Configuration["Auth0:BlazorClientSecret"]);
        options.InjectJavascript("/swagger-custom.js");
    });
    }

    if (!(app.Environment.IsProduction() || app.Environment.IsStaging()))
        app.UseHttpsRedirection();

    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();

    app.UseSerilogIngestion();

    app.UseMiddleware<ExceptionMiddleware>();

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers().RequireAuthorization();
    app.MapFallbackToFile("index.html");

    if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
    {
        using var scope = app.Services.CreateScope();
        // Require a DbContext from the service provider and seed the database.
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        new Seeder(dbContext).Seed();
    }

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
public partial class Program { }
