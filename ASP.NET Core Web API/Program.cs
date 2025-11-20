using MES_System.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MES_System.Application.Interfaces;
using MES_System.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. 註冊服務 (Service Registration)
// 注意：所有的 builder.Services.Add... 都要寫在 builder.Build() 之前
// ==========================================

// 加入框架基礎服務 (Controller, Swagger)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// [Day 2] 註冊 DbContext (使用 In-Memory Database)
builder.Services.AddDbContext<MesDbContext>(options =>
    options.UseInMemoryDatabase("MesDb"));

// [Day 3] 註冊 Repository (依賴注入)
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();

// ==========================================
// 2. 建置應用程式 (Build)
// 這行指令就像一道牆，牆前面是準備材料，牆後面是開始運作
// ==========================================
var app = builder.Build();

// ==========================================
// 3. 設定 HTTP 請求管線 (Middleware)
// ==========================================

// [Day 3] 確保資料庫已建立 (觸發種子資料)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MesDbContext>();
    context.Database.EnsureCreated();
}

// 設定開發環境下的 Swagger 頁面
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// 啟動程式
app.Run();