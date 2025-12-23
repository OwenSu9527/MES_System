using MES_System.Application.Interfaces;
using MES_System.Application.Services;
using MES_System.Infrastructure;
using MES_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
// [Day 16]
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

using MES_System.WebAPI; // 引用 SystemTrayManager
using MES_System.WebAPI.Middleware; // 稍後建立
using Serilog; // [Day 17]
using System.Windows.Forms; // [Day 21]

// 手動初始化 UI 樣式
System.Windows.Forms.Application.EnableVisualStyles();
System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

// [Day 17] 1. 在 Builder 建立前，先設定 Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // 確保有檔案 Log，因為 Console 看不到了
    .CreateBootstrapLogger(); // 建立啟動階段的 Logger

//var builder = WebApplication.CreateBuilder(args);

try
{
    Log.Information("系統啟動中 (Starting Web Host)...");

    var builder = WebApplication.CreateBuilder(args);

    // [Day 17] 2. 告訴 Host 使用 Serilog 取代預設 Logger
    // 這行會去讀取 appsettings.json 的設定
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));
    // ==========================================
    // 1. 註冊服務 (Service Registration)
    // 注意：所有的 builder.Services.Add... 都要寫在 builder.Build() 之前
    // ==========================================

    // [Day 4 新增] 註冊 CORS 服務
    // 定義一個政策名稱叫做 "AllowAll"
    builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // 允許任何來源 (e.g. 你的 HTML 檔)
              .AllowAnyMethod()  // 允許任何 HTTP 方法 (GET, POST...)
              .AllowAnyHeader(); // 允許任何標頭
    });
});

    // 加入框架基礎服務 (Controller, Swagger)
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // [Day 2] 註冊 DbContext (使用 In-Memory Database)
    //builder.Services.AddDbContext<MesDbContext>(options =>
    //    options.UseInMemoryDatabase("MesDb"));

    // [Day 6] DB First 連線設定
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<MesDbContext>(options =>
        options.UseSqlServer(connectionString));

    // [Day 3] 註冊 Repository (依賴注入)
    builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();

    // [Day 7] 註冊 WorkOrder 相關服務
    builder.Services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
    builder.Services.AddScoped<IWorkOrderService, WorkOrderService>();

    // [Day 8] 註冊 WIP Repository
    builder.Services.AddScoped<IWipRepository, WipRepository>();

    // [Day 13] 註冊維修管理服務
    builder.Services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
    builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();

    // [Day 16] 1. 設定 JWT 驗證服務
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
        };
    });
    // [Day 16] 2. 設定 Swagger 支援 Bearer Token
    builder.Services.AddSwaggerGen(c =>
    {
        // 原本的設定方式要自行輸入 "Bearer "
        //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        //{
        //    Description = "請在下方輸入: Bearer {your_token}",
        //    Name = "Authorization",
        //    In = ParameterLocation.Header,
        //    Type = SecuritySchemeType.ApiKey,
        //    Scheme = "Bearer"
        //});

        // 優化後只要輸入 Token：改用 Type = Http, Scheme = bearer
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "請直接在下方貼上 JWT Token (無需輸入 Bearer)",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http, // 改成 Http
            Scheme = "bearer",              // 指定 Scheme
            BearerFormat = "JWT"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
        });
        // [Day 19] 加入 XML 註解讀取
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });

    // ==========================================
    // 2. 建置應用程式 (Build)
    // 這行指令就像一道牆，牆前面是準備材料，牆後面是開始運作
    // ==========================================
    var app = builder.Build();

    // [Day 17] 3. 加入 HTTP 請求紀錄 (這會紀錄每個 API 的耗時、回應碼)
    app.UseSerilogRequestLogging();

    // ==========================================
    // 3. 設定 HTTP 請求管線 (Middleware) Code First ：依賴程式碼 (EnsureCreated) 「生出」一個資料庫和種子資料。
    // ==========================================
    // [Day 3] 確保資料庫已建立 (觸發種子資料)
    //using (var scope = app.Services.CreateScope())
    //{
    //    var services = scope.ServiceProvider;
    //    var context = services.GetRequiredService<MesDbContext>();
    //    context.Database.EnsureCreated();
    //}

    // 設定開發環境下的 Swagger 頁面
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // [Day 4 新增] 套用 CORS 政策
    // 重要：這行放在 UseAuthorization 之前
    app.UseCors("AllowAll");
    app.UseMiddleware<ExceptionMiddleware>(); // [Day 17] 註冊全局異常處理 Middleware (稍後實作)

    // [Day 16] 3. 啟用驗證與授權 (順序很重要!)
    app.UseAuthentication(); // 驗證：你是誰？

    app.UseAuthorization(); // 授權：你能進來嗎？

    app.MapControllers(); // 映射控制器路由

    // [Day 21] 關鍵修改：並行執行 WebAPI 與 System Tray

    // 1. 啟動 System Tray 管理器
    // 這裡我們手動建立，因為它依賴 Windows Forms 執行緒
    var trayManager = new SystemTrayManager(app.Lifetime);

    // 2. 在背景 Task 中啟動 WebAPI
    // 我們使用 app.RunAsync() 而不是 app.Run()，避免卡住主執行緒
    var webTask = app.RunAsync();

    // 3. 啟動 Windows Forms 訊息迴圈 (這會卡住主執行緒，直到 Application.Exit 被呼叫)
    System.Windows.Forms.Application.Run();

    // 當 Application.Run() 結束後 (使用者輸入密碼關閉)，等待 WebAPI 正常關閉
    await webTask;
}
catch (Exception ex)
{
    // 捕捉系統啟動過程的致命錯誤 (如 DB 連線字串錯誤)
    Log.Fatal(ex, "系統意外終止 (Host terminated unexpectedly)");
}
finally
{
    // 確保日誌緩衝區寫入硬碟後才關閉
    Log.CloseAndFlush();
}