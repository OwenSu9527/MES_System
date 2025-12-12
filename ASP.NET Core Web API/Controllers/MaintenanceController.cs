using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MES_System.Application.DTOs;
using MES_System.Application.Interfaces;

namespace MES_System.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceService _service;

        // 注入 Service (注意：不是 Repository)
        public MaintenanceController(IMaintenanceService service)
        {
            _service = service;
        }

        // POST: api/Maintenance/request
        // 建立維修單
        [HttpPost("request")]
        public async Task<IActionResult> CreateRequest(CreateMaintenanceDto dto)
        {
            try
            {
                await _service.CreateMaintenanceRequestAsync(dto);
                return Ok(new { message = "報修成功，機台已標記為故障 (Down)" });
            }
            catch (Exception ex)
            {
                // 這裡捕捉 Service 拋出的 "機台不存在" 等錯誤
                return BadRequest(new { message = ex.Message });
            }
        }
        // PATCH: api/Maintenance/{id}/start
        [HttpPatch("{id}/start")]
        public async Task<IActionResult> StartMaintenance(int id)
        {
            try
            {
                await _service.StartMaintenanceAsync(id);
                return Ok(new { message = "開始維修，機台狀態已變更為 Repair" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/Maintenance/complete
        [HttpPost("complete")]
        public async Task<IActionResult> CompleteMaintenance(CompleteMaintenanceDto dto)
        {
            try
            {
                await _service.CompleteMaintenanceAsync(dto);
                return Ok(new { message = "維修完成，機台已恢復為 Idle" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}