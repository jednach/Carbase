using Carbase.Data;
using Carbase.Infrastructure.ModelBinding;
using Carbase.Services;
using Carbase.Validation.Car;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorPages()
    .AddMvcOptions(options =>
    {
        options.ModelBinderProviders.Insert(
            0,
            new FlexibleDecimalModelBinderProvider());
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ngsqlOptions =>
        {
            ngsqlOptions.MigrationsHistoryTable(
                "__EFMigrationsHistory",
                "carbase");
        }));
builder.Services.AddScoped<CarImageService>();
builder.Services.AddValidatorsFromAssemblyContaining<CarCreateRequestValidator>();
builder.Services.AddScoped<CarQueryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
