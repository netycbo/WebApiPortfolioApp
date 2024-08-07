﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using RestSharp;
using System.Reflection;
using WebApiPortfolioApp.API;
using WebApiPortfolioApp.API.Handlers;
using WebApiPortfolioApp.API.Handlers.Services;
using WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices;
using WebApiPortfolioApp.API.Handlers.Services.ChcekBeerPriceDailyServices.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.DeserializeService;
using WebApiPortfolioApp.API.Handlers.Services.Interfaces;
using WebApiPortfolioApp.API.Handlers.Services.NewsLetterProductsServices;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices;
using WebApiPortfolioApp.API.Handlers.Services.ProductSearchServices.Interfaces;
using WebApiPortfolioApp.API.Mappings;
using WebApiPortfolioApp.Data;
using WebApiPortfolioApp.Data.Entinities.Identity;
using WebApiPortfolioApp.ExeptionsHandling;
using WebApiPortfolioApp.HealthChecks;
using WebApiPortfolioApp.Providers.ViewRender;
using WebApiPortfolioApp.Services.SendEmail;
using WebApiPortfolioApp.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UsersOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("AdminOrUser", policy => policy.RequireClaim("Admin", "User"));
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RegisteringHandler>());
builder.Services.AddAutoMapper(typeof(Profiles).GetTypeInfo().Assembly, typeof(Profiles).Assembly);
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<EmailSettings>>().Value);
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddMvc();
builder.Services.AddScoped<NoSpecialCharactersAttribute>();
builder.Services.AddScoped<ViewRender>();
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Clear();
    options.ViewLocationFormats.Add("~/Providers/{1}/{0}/{0}.cshtml");
    options.ViewLocationFormats.Add("~/Providers/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("~/Providers/{0}.cshtml");
});
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("KassalappenApi"));
builder.Services.AddSingleton<IRestClient>(sp =>
{
    var apiSettings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
    return new RestClient(apiSettings.BaseUrl);
});

builder.Services.AddScoped<IProductFilterService, ProductFilterService>();
builder.Services.AddScoped<ISaveProductService, SaveProductService>();
builder.Services.AddScoped<IUserIdService, UserIdService>();
builder.Services.AddScoped<ApplicationUser>();
builder.Services.AddScoped<DatabaseHealthCheck>();
builder.Services.AddScoped<ApiHealthCheck>();
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("Database")
    .AddCheck<ApiHealthCheck>("Api");
builder.Services.AddScoped<IComparePrices, ComparePrices>();
builder.Services.AddScoped<IFetchProductDetails, ProductDetailsFetcher>();
builder.Services.AddScoped<IAveragePriceComparator, AveragePriceComperator>();
builder.Services.AddScoped<IShopNameValidator, ShopNameValidator>();
builder.Services.AddScoped<IDeserializeService, DeserializeService>();
builder.Services.AddScoped<IUserNameClaimService, UserNameClaimService>();
builder.Services.AddScoped<IGetEmailService, GetEmailService>();
builder.Services.AddScoped<ISaveToProductSubscriptionService, SaveToProductSubscriptionService>();
builder.Services.AddSingleton<ShopNameList>();
builder.Services.AddSingleton(provider =>
{
    var shopNameList = provider.GetRequiredService<ShopNameList>();
    return shopNameList.Names;
});
builder.Services.AddScoped<IApiCall, ApiCall>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var apiKey = configuration.GetValue<string>("KassalappenApi:ApiKey");
    return new ApiCall(provider.GetRequiredService<IRestClient>(), apiKey);
});
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    q.AddJob<PriceCheckJob>(opts => opts.WithIdentity("PriceCheckJob").StoreDurably());

});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await schedulerFactory.GetScheduler();
await JobScheduler.ScheduleJob(scheduler);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
