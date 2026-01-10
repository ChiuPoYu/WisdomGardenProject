using DBScriptsConvert.Services;
using DBScriptsConvert.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 加入 Swagger 支援
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Service 依賴注入
builder.Services.AddScoped<IConvertService, ConvertService>();
  
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // 啟用 Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
