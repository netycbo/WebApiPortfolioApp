using Microsoft.EntityFrameworkCore;
using WebApiPortfolioApp.API.Handlers;
using WebApiPortfolioApp.API;
using WebApiPortfolioApp.Data;
using WebApiPortfolioApp.ExeptionsHandling;
using WebApiPortfolioApp.Extensions;
using RestSharp;
using WebApiPortfolioApp.API.Handlers.Services;
using Microsoft.Extensions.Options;
using WebApiPortfolioApp.Services.SendEmail;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices;
using WebApiPortfolioApp.Providers.ViewRender;
using WebApiPortfolioApp.Validation;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

// Singleton
builder.Services.AddSingleton<ShopNameList>();

// Scoped
builder.Services.AddScoped<IUserNameClaimService, UserNameClaimService>();
builder.Services.AddScoped<IShopNameValidator, ShopNameValidator>();
builder.Services.AddScoped<IViewRender, ViewRender>();
builder.Services.AddScoped<ViewRender>();
builder.Services.AddScoped<NoSpecialCharactersAttribute>();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication
builder.Services.AddAppAuthentication(builder.Configuration);

// View Engine Configuration
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Clear();
    options.ViewLocationFormats.Add("~/Providers/{1}/{0}/{0}.cshtml");
    options.ViewLocationFormats.Add("~/Providers/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("~/Providers/{0}.cshtml");
});

builder.Services.AddHttpContextAccessor();

// App Services
builder.Services.AddAppServices();
builder.Services.AddQuartzJobs();
builder.Services.AddAppHealthChecks();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RegisteringHandler>());
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// API Call Service
builder.Services.AddScoped<IApiCall, ApiCall>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var apiKey = configuration.GetValue<string>("KassalappenApi:ApiKey");
    var client = provider.GetRequiredService<IRestClient>();
    return new ApiCall(client, apiKey);
});

// Email Configuration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<EmailSettings>>().Value);
builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();