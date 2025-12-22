using MES_System.Application.DTOs;
using MES_System.Application.Interfaces;
using MES_System.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace MES_System.WebAPI.Controllers
{
    /// <summary>
    /// 工單管理控制器
    /// </summary>
    /// <remarks>
    /// 提供工單的查詢、建立與狀態變更功能。
    /// </remarks>
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

        /// <summary>
        /// 取得所有工單列表
        /// </summary>
        /// <returns>工單物件列表</returns>
        /// <response code="200">成功取得資料</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrder>>> GetWorkOrders()
        {
            var orders = await _service.GetAllWorkOrdersAsync();
            return Ok(orders);// 回傳 HTTP 200 與資料
        }

        /// <summary>
        /// 建立新的生產工單
        /// </summary>
        /// <remarks>
        /// 範例請求 (Request Body):
        /// 
        ///     POST /api/WorkOrder
        ///     {
        ///        "orderNo": "WO-20231222-001",
        ///        "product": "Gaming Mouse",
        ///        "targetQty": 500
        ///     }
        /// 
        /// </remarks>
        /// <param name="dto">工單建立資訊 (DTO)</param>
        /// <returns>新建立的工單物件</returns>
        /// <response code="201">工單建立成功</response>
        /// <response code="400">資料格式錯誤或邏輯驗證失敗</response>
        /// <response code="401">未授權 (Token 無效)</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<WorkOrder>> CreateWorkOrder(CreateWorkOrderDto dto)
        {
            // 呼叫 Service 執行建立邏輯
            var newOrder = await _service.CreateWorkOrderAsync(dto);
            // CreatedAtAction 會回傳 HTTP 201 Created
            // 並在 Header 的 Location 欄位告訴前端去哪裡查詢這筆新資料 (即 GetWorkOrders)
            return CreatedAtAction(nameof(GetWorkOrders), new { id = newOrder.Id }, newOrder);
        }

        /// <summary>
        /// 開始工單 (變更狀態為 Running)
        /// </summary>
        /// <param name="id">工單 ID (Database ID)</param>
        /// <returns>無回傳內容</returns>
        /// <response code="204">狀態變更成功</response>
        /// <response code="404">找不到該工單 ID</response>
        [HttpPatch("{id}/start")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> StartWorkOrder(int id)
        {
            var result = await _service.StartWorkOrderAsync(id);
            if (!result) return NotFound();// 如果 Service 回傳 false，代表找不到 ID，回傳 404
            return NoContent();// 成功執行但不需要回傳資料，通常回傳 204 NoContent
        }
    }
}