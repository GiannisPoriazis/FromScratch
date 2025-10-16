using Microsoft.EntityFrameworkCore;
using RetailStoreManagement;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<RetailStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddMaps(typeof(RetailStoreManagement.Mapping.PurchaseProfile).Assembly);
});

IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mappingConfig);
builder.Services.AddSingleton(mapper);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }