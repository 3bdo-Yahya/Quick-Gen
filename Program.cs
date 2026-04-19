using Quick_Gen.Extensions;
using Quick_Gen.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddQuickGenPersistence(builder.Configuration);
builder.Services.AddQuickGenJwtAuthentication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await IdentitySeeder.SeedAsync(app.Services).ConfigureAwait(false);

await app.RunAsync().ConfigureAwait(false);
