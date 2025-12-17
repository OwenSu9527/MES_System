using MES_System.Application.DTOs;
using MES_System.Application.Interfaces;
using MES_System.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace MES_System.WebAPI.Controllers
{
    [Route("api/[controller]")]// 設定網址路徑，這裡會變成 /api/WorkOrder
    [ApiController] // 標記為 API 控制器
    [Authorize] // [Day 16]需要授權才能存取
    public class WorkOrderController : ControllerBase
    {
        private readonly IWorkOrderService _service;

        // 注入 Service，而不是 Repository (Controller -> Service -> Repository)
        public WorkOrderController(IWorkOrderService service)
        {
            _service = service;
        }

        // 取得所有工單
        // GET: api/workorder
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrder>>> GetWorkOrders()
        {
            var orders = await _service.GetAllWorkOrdersAsync();
            return Ok(orders);// 回傳 HTTP 200 與資料
        }

        // 建立新工單
        // POST: api/workorder
        [HttpPost]
        public async Task<ActionResult<WorkOrder>> CreateWorkOrder(CreateWorkOrderDto dto)
        {
            // 呼叫 Service 執行建立邏輯
            var newOrder = await _service.CreateWorkOrderAsync(dto);
            // CreatedAtAction 會回傳 HTTP 201 Created
            // 並在 Header 的 Location 欄位告訴前端去哪裡查詢這筆新資料 (即 GetWorkOrders)
            return CreatedAtAction(nameof(GetWorkOrders), new { id = newOrder.Id }, newOrder);
        }

        // 開始工單 (開工)
        // PATCH: api/WorkOrder/{id}/start
        // 用 PATCH 代表「部分更新」資源
        [HttpPatch("{id}/start")]
        public async Task<IActionResult> StartWorkOrder(int id)
        {
            var result = await _service.StartWorkOrderAsync(id);
            if (!result) return NotFound();// 如果 Service 回傳 false，代表找不到 ID，回傳 404
            return NoContent();// 成功執行但不需要回傳資料，通常回傳 204 NoContent
        }
    }
}