
using DigitalSignService.Business.IServices;
using DigitalSignService.Business.IServices.Sign;
using DigitalSignService.Business.Service3th;
using DigitalSignService.Business.Services;
using DigitalSignService.Business.Services.Sign;
using DigitalSignService.DAL;
using DigitalSignService.DAL.IRepository;
using DigitalSignService.DAL.Models;
using DigitalSignService.DAL.Repository;
using DPURedisService;
using DPUStorageService.APIs;
using DPUStorageService.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using System.Text;
using WebGisBE.Business.Services;

string policyName = "DigitalSignService";
var builder = WebApplication.CreateBuilder(args);
var secretKey = Encoding.ASCII
    .GetBytes(builder.Configuration["AuthSetting:SecretKey"] ?? "DlhXHPrSJgIzqZzhK0nRrVPuOo4nhzVF");
var authority = builder.Configuration["AuthSetting:Authority"] ?? "https://localhost:7153";
var secretKeySign = Encoding.ASCII
    .GetBytes(builder.Configuration["AuthSetting:SecretKeySign"] ?? "DlhXHPrSJgIzqZzhK0nRrVPuOo4nhzVF");
var appSettings = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSetting>(appSettings);
var authSetting = builder.Configuration.GetSection("AuthSetting");
builder.Services.Configure<AuthSetting>(authSetting);
var configAPI = builder.Configuration.GetSection("ConfigAPI");
builder.Services.Configure<ConfigAPI>(configAPI);
var redisSettings = builder.Configuration.GetSection("Redis");
builder.Services.Configure<RedisSetting>(redisSettings);
var config = new Config();
builder.Configuration.GetSection("Config").Bind(config);
if (config is not null)
{
    builder.Services.AddSingleton(config);
}
// Add connection database
builder.Services.AddDbContext<DataBaseContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgres"));
});

builder.Services.AddControllers().AddNewtonsoftJson();
// JWT authentication
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer("DigitalSignAuth", options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(secretKeySign),
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false
    };
}).AddJwtBearer("AuthGateway", options =>
{
    options.Authority = authority;
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                        Enter 'Bearer' [space] and then your token in the text input below.
                        \r\n\r\nExample: '12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
          {
            {
              new OpenApiSecurityScheme
              {
                Reference = new OpenApiReference
                  {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                  },
                  Scheme = "oauth2",
                  Name = "Bearer",
                  In = ParameterLocation.Header,
              },
            new List<string>()
        }
            });
    options.CustomSchemaIds(type => type.FullName?.ToString().Replace("+", "."));
    options.EnableAnnotations();
});
// Add CORS policy
string[]? origins = builder.Configuration.GetSection("Origins").Get<string[]>();
builder.Services.AddCors(options => options.AddPolicy(policyName,
    p => p.WithOrigins(origins ?? [])
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
          ));

builder.Services.AddHttpContextAccessor();

// Configure Digital Sign settings
var digitalSignSettings = builder.Configuration.GetSection("DigitalSign");
builder.Services.Configure<DigitalSignSettings>(digitalSignSettings);

// Register services
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
builder.Services.AddSingleton<CachingService>();
builder.Services.AddTransient<ViettelSignApi>();
builder.Services.AddTransient<WebhookApi>();
builder.Services.AddTransient<FileApi>();
builder.Services.AddTransient<ITemplateService, TemplateService>();
builder.Services.AddTransient<ISigningProvider, ViettelSigningProvider>();
builder.Services.AddTransient<ISigningProvider, VnptSigningProvider>();
builder.Services.AddTransient<IPaperSizeService, PaperSizeService>();
builder.Services.AddTransient<IApiStorage, ApiStorage>();
builder.Services.AddTransient<IJWTContext, JWTContext>();
builder.Services.AddSingleton<RedisService>();
builder.Services.AddHostedService<WebhookService>();

// Register provider factory
builder.Services.AddSingleton<ISigningProviderFactory, SigningProviderFactory>();

// Register repositories
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddTransient<ITemplateRepository, TemplateRepository>();
builder.Services.AddTransient<IHistorySignRepository, HistorySignRepository>();
builder.Services.AddTransient<IPaperSizeRepository, PaperSizeRepository>();


builder.Services.AddHealthChecks();
// Configure logging
var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
builder.Logging.ClearProviders();
builder.Host.UseNLog();
logger.Info($"Application Starting Up At {DateTime.UtcNow}");


var app = builder.Build();
// seeder data - commented for testing
/*using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
    db.Database.Migrate();
    await SeedPaperSize.SeedData(db);
}*/
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseCors(policyName);

app.MapHealthChecks("/healthz");

app.Run();