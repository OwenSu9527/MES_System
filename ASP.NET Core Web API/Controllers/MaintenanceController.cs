using MES_System.Application.DTOs;
using MES_System.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MES_System.WebAPI.Controllers
{
    /// <summary>
    /// 設備維修管理控制器
    /// </summary>
    /// <remarks>
    /// 負責處理設備故障報修流程。
    /// 包含：報修 (Request) -> 開始維修 (Start) -> 完修 (Complete) 的完整生命週期。
    /// 此控制器會與機台狀態 (Equipment Status) 進行聯動更新。
    /// </remarks>
    [Route("api/[controller]")] // 設定網址路由為 /api/maintenance
    [ApiController] // 標記為 API 控制器
    [Authorize] // [Day 16]需要授權才能存取
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceService _service;

        // 注入 Service (注意：不是 Repository)
        public MaintenanceController(IMaintenanceService service)
        {
            _service = service;
        }

        /// <summary>
        /// 提交設備報修單 (Request)
        /// </summary>
        /// <remarks>
        /// 產線人員發現異常時呼叫。
        /// 成功後，系統會自動將該機台狀態更新為 **Down (故障)**。
        /// 
        /// 範例請求:
        /// 
        ///     POST /api/Maintenance/request
        ///     {
        ///        "equipmentId": 1,
        ///        "requestUser": "Operator-Kevin",
        ///        "reasonCode": "ERR-500",
        ///        "description": "主軸馬達異音，溫度過高"
        ///     }
        /// 
        /// </remarks>
        /// <param name="dto">報修資訊 DTO</param>
        /// <returns>操作結果訊息</returns>
        /// <response code="200">報修成功</response>
        /// <response code="400">報修失敗 (如：機台不存在)</response>
        [HttpPost("request")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        /// <summary>
        /// 開始維修作業 (Start)
        /// </summary>
        /// <remarks>
        /// 維修工程師到場後呼叫。
        /// 成功後，系統會自動將該機台狀態更新為 **Repair (維修中)**。
        /// </remarks>
        /// <param name="id">維修單 ID (並非機台 ID)</param>
        /// <returns>操作結果訊息</returns>
        /// <response code="200">狀態變更成功</response>
        /// <response code="400">操作失敗 (如：維修單不存在)</response>
        [HttpPatch("{id}/start")] // PATCH: api/Maintenance/{id}/start
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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