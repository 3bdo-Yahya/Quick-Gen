using Quick_Gen.Extensions;
using Quick_Gen.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddQuickGenPersistence(builder.Configuration);
builder.Services.AddQuickGenJwtAuthentication(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
});

builder.Services.AddOpenApi();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Quick Gen API";
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
        options.AddHttpAuthentication("Bearer", scheme =>
        {
            scheme.Token = "";
        });
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await IdentitySeeder.SeedAsync(app.Services).ConfigureAwait(false);
await app.RunAsync().ConfigureAwait(false);