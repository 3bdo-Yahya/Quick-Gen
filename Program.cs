using Quick_Gen.Extensions;
using Quick_Gen.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddQuickGenPersistence(builder.Configuration);
builder.Services.AddQuickGenJwtAuthentication(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); 
    app.MapScalarApiReference(options =>
    {
        options.Title = "Quick Gen API";
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await IdentitySeeder.SeedAsync(app.Services).ConfigureAwait(false);
await app.RunAsync().ConfigureAwait(false);