using E_ToolTiktok.Core.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "E_ToolTiktok API",
        Version = "v1",
        Description = "API for TikTok Account Registration"
    });
});

// Register HttpClient
builder.Services.AddHttpClient();

// Register services
builder.Services.AddScoped<IInboxesEmailService, InboxesEmailService>();
builder.Services.AddScoped<INameGeneratorService, NameGeneratorService>();
builder.Services.AddScoped<IProxyService, ProxyService>();
builder.Services.AddScoped<ITiktokApiService, TiktokApiService>();
builder.Services.AddScoped<ITiktokRegistrationService, TiktokRegistrationService>();

// Add CORS for mobile/web clients
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

// Configure the HTTP request pipeline.
// Enable Swagger for all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "E_ToolTiktok API V1");
    c.RoutePrefix = "swagger";
});

// Enable static files
app.UseStaticFiles();

// Default route to index.html
app.MapGet("/", () => Results.Redirect("/index.html"));

// CORS phải được đặt trước UseAuthorization
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

