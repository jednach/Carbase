using Amazon.Runtime;
using Amazon.S3;
using Carbase.Data;
using Carbase.Infrastructure.ModelBinding;
using Carbase.Services;
using Carbase.Services.Storage;
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


builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var configuration =
        sp.GetRequiredService<IConfiguration>();

    var accountId = configuration["R2:AccountId"]
        ?? throw new InvalidOperationException(
            "R2 AccountId is missing.");

    var accessKeyId = configuration["R2:AccessKeyId"]
        ?? throw new InvalidOperationException(
            "R2 AccessKeyId is missing.");

    var secretAccessKey = configuration["R2:SecretAccessKey"]
        ?? throw new InvalidOperationException(
            "R2 SecretAccessKey is missing.");

    var credentials = new BasicAWSCredentials(
        accessKeyId,
        secretAccessKey);

    var s3Config = new AmazonS3Config
    {
        ServiceURL =
            $"https://{accountId}.r2.cloudflarestorage.com",
        ForcePathStyle = true,
        AuthenticationRegion = "auto"
    };

    return new AmazonS3Client(credentials, s3Config);
});

builder.Services.AddScoped<ICarImageStorage,R2CarImageStorage>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext =
        scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();


app.MapGet("/images/cars/{fileName}", async (
    string fileName,
    ICarImageStorage storage,
    HttpContext context) =>
{
    var extension = Path.GetExtension(fileName).ToLowerInvariant();
    var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

    if (!Guid.TryParseExact(nameWithoutExtension, "D", out _) ||
        extension is not (".jpg" or ".png" or ".webp"))
    {
        return Results.NotFound();
    }

    try
    {
        using var image = await storage.GetAsync(fileName);

        context.Response.ContentType = image.Headers.ContentType
            ?? "application/octet-stream";

        context.Response.Headers.CacheControl =
            "public, max-age=86400";

        await image.ResponseStream.CopyToAsync(
            context.Response.Body,
            context.RequestAborted);

        return Results.Empty;
    }
    catch (Amazon.S3.AmazonS3Exception ex)
        when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
        return Results.NotFound();
    }
});


app.Run();
