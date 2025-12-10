using Microsoft.AspNetCore.Http;
using MES_System.Application.DTOs;
using MES_System.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MES_System.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionController : ControllerBase
    {
        private readonly IWorkOrderService _service;

        public ProductionController(IWorkOrderService service)
        {
            _service = service;
        }

        // POST: api/Production/report
        // 生產回報
        [HttpPost("report")]
        public async Task<IActionResult> ReportProduction(ProductionReportDto dto)
        {
            // [Day 9] 加入 Try-Catch 區塊
            try
            {
                var result = await _service.ReportProductionAsync(dto);

                if (!result)
                {
                    return BadRequest("回報失敗：工單不存在、尚未開工，或數量不正確。");
                }

                return Ok(new { message = "生產回報成功", data = dto });
            }
            catch (Exception ex)
            {
                // 這裡可以加入 Log 紀錄 (Day 17 會教)
                // 回傳 500 Internal Server Error
                return StatusCode(500, $"系統內部錯誤: {ex.Message}");
            }
        }
    }
}