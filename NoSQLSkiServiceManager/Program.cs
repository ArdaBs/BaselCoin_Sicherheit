using AutoMapper;
using NoSQLSkiServiceManager.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using NoSQLSkiServiceManager.DTOs.Request;
using NoSQLSkiServiceManager.Profiles;
using NoSQLSkiServiceManager.DTOs.Response;
using NoSQLSkiServiceManager.Middlewares;
using NoSQLSkiServiceManager.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var mongoDbConnectionString = "mongodb://localhost:27017";
var databaseName = "JetStreamAPI";

builder.Services.AddSingleton<IMongoClient>(new MongoClient(builder.Configuration.GetConnectionString("MongoDbConnection")));

builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    return client.GetDatabase("JetStreamAPI");
});

builder.Services.AddSingleton(serviceProvider =>
{
    var database = serviceProvider.GetRequiredService<IMongoDatabase>();
    var mapper = serviceProvider.GetRequiredService<IMapper>();
    return new GenericService<AccountHolder, AccountHolderCreateDto, AccountBalanceUpdateDto, AccountHolderResponseDto>(database, mapper, "accountHolders");
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverter());
    });

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Serilog-Configuration
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

// AutoMapper-Configuration
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddSingleton<TokenService>();

string toolsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MongoTools");
string backupDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MongoDbBackup");

builder.Services.AddHostedService<MongoBackupManager>(serviceProvider =>
{
    return new MongoBackupManager(databaseName, toolsDirectory, backupDirectory);
});



builder.Services.AddSingleton<EmployeeService>();


builder.Services.AddSingleton<MongoDBService>(serviceProvider =>
{
    return new MongoDBService(mongoDbConnectionString, databaseName);
});

// Swagger, MVC and other services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
var allowedOrigins = builder.Configuration["CORS:AllowedOrigins"]?.Split(',') ?? new string[0];
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "POC MongoDB API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        new string[] {}
                    }
                });

});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<TokenValidationMiddleware>();
app.UseStaticFiles();


// HTTP-Request-Pipeline
// Naütrlich ist mir bewusst das in
// einem produktiven umgebung nicht
// eingesetzt wird aber für projekt
// zwecke jetzt gemacht
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var mongoDBService = app.Services.GetRequiredService<MongoDBService>();
await mongoDBService.EnsureDatabaseSetupAsync();

app.Run();