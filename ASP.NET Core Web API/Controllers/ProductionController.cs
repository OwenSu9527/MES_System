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
            var result = await _service.ReportProductionAsync(dto);

            if (!result)
            {
                // 回報失敗 (工單找不到，或狀態不是 Running)
                return BadRequest("回報失敗：工單不存在或尚未開工 (Status must be 'Running')");
            }

            return Ok(new { message = "生產回報成功", data = dto });
        }
    }
}