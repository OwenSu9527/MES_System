// Day 3
using MES_System.Application.Interfaces;
using MES_System.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MES_System.WebAPI.Controllers
{
    [Route("api/[controller]")] // 設定網址路由為 /api/equipment
    [ApiController] // 標記為 API 控制器
    [Authorize] // [Day 16]需要授權才能存取
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentRepository _repository;

        // 建構子注入：我們不依賴 DbContext，而是依賴介面 IEquipmentRepository
        public EquipmentController(IEquipmentRepository repository)
        {
            _repository = repository;
        }

        // GET: api/equipment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Equipment>>> GetEquipments()
        {
            var equipments = await _repository.GetAllAsync();
            return Ok(equipments); // 回傳 HTTP 200 OK 與資料
        }

        // GET: api/equipment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Equipment>> GetEquipment(int id)
        {
            var equipment = await _repository.GetByIdAsync(id);

            if (equipment == null)
            {
                return NotFound(); // 找不到就回傳 HTTP 404
            }

            return Ok(equipment);
        }

        // [Day 18 新增] 接收 Python 模擬器的遙測數據 (RPM, Temp)
        // PATCH: api/equipment/5/telemetry
        [HttpPatch("{id}/telemetry")]
        public async Task<IActionResult> UpdateTelemetry(int id, [FromBody] Application.DTOs.UpdateTelemetryDto dto)
        {
            // 1. 使用 Repository 查詢機台
            var equipment = await _repository.GetByIdAsync(id);

            if (equipment == null)
            {
                return NotFound();
            }

            // 2. 更新數據
            equipment.RPM = dto.Rpm;
            equipment.Temperature = dto.Temperature;
            equipment.LastUpdated = DateTime.Now;

            // 3. 使用 Repository 存檔 (Day 13 我們有實作 UpdateAsync)
            await _repository.UpdateAsync(equipment);

            return NoContent(); // 回傳 204 No Content (代表成功但不需要回傳內容)
        }
    }
}
