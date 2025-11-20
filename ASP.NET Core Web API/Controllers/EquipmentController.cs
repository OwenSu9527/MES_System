using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
// Day 3
using MES_System.Application.Interfaces;
using MES_System.Domain.Entities;

namespace ASP.NET_Core_Web_API.Controllers
{
    [Route("api/[controller]")] // 設定網址路由為 /api/equipment
    [ApiController]
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
    }
}
