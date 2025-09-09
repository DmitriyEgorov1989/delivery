using DeliveryApp.Api;
using DeliveryApp.Core.Domain.DependencyInjection;
using DeliveryApp.Infrastructure.Adapters.Postgres.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//DI DomainService
builder.Services.AddDomainService();

//DI DataBaseService
builder.Services.AddDataBaseServices(builder.Configuration);

// Health Checks
builder.Services.AddHealthChecks();

// Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin(); // Не делайте так в проде!
        });
});

// Configuration
builder.Services.ConfigureOptions<SettingsSetup>();

var app = builder.Build();

// -----------------------------------
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseRouting();

// Apply Migrations
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     db.Database.Migrate();
// }

app.Run();