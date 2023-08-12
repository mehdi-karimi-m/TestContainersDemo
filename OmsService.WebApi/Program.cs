using OmsService.WebApi.Infrastructure;
using OmsService.WebApi.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("OmsDbConnection");
builder.Services.AddScoped<IOrderRepository>(s => new OrderRepository(connectionString));
builder.Services.AddSingleton(p => new Migration(connectionString, p.GetRequiredService<ILogger<Migration>>()));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var migration = app.Services.GetService<Migration>();
if (migration != null)
    await migration.Migrate();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//}
app.UseSwagger();
app.UseSwaggerUI();


//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
