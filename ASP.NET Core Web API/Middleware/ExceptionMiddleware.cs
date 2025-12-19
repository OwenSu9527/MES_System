using System.Net;
using System.Text.Json;

namespace MES_System.WebAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // 嘗試執行下一個管線 (Controller)
                await _next(context);
            }
            catch (Exception ex)
            {
                // 如果發生錯誤，跳到這裡處理
                _logger.LogError(ex, "發生未處理的異常: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500

            // 在開發模式下回傳詳細錯誤，正式環境只回傳 "Internal Server Error"
            // 修改後 (已修正)：
            // 重點：在第二個物件中補上 StackTrace = (string?)null，確保兩邊結構一致
            var response = _env.IsDevelopment()
                ? new { StatusCode = context.Response.StatusCode, Message = ex.Message, StackTrace = ex.StackTrace?.ToString() }
                : new { StatusCode = context.Response.StatusCode, Message = "系統內部錯誤，請聯絡管理員 (Internal Server Error)", StackTrace = (string?)null };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}