using Microsoft.AspNetCore.Http;
using MES_System.Application.DTOs;
using MES_System.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MES_System.Infrastructure.Repositories;

namespace MES_System.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionController : ControllerBase
    {
        private readonly IWorkOrderService _service;
        private readonly IWipRepository _wipRepo; // [Day 10 新增]

        // (舊有)
        //public ProductionController(IWorkOrderService service)
        //{
        //    _service = service;
        //}

        // (新)修改建構子，注入 IWipRepository
        public ProductionController(IWorkOrderService service, IWipRepository wipRepo)
        {
            _service = service;
            _wipRepo = wipRepo;
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
        // [Day 10 新增] 取得 WIP 概況
        // GET: api/Production/wip
        [HttpGet("wip")]
        public async Task<IActionResult> GetWipOverview()
        {
            var data = await _wipRepo.GetAllAsync();
            return Ok(data);
        }
    }
}