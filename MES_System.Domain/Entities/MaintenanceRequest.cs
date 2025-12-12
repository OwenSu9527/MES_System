using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_System.Domain.Entities
{
    /// <summary>
    /// 維修請求單
    /// </summary>
    public class MaintenanceRequest
    {
        public int Id { get; set; }

        public int EquipmentId { get; set; }
        // 這是 EF Core 的導覽屬性 (Navigation Property)，讓我們可以直接存取機台物件
        // [ForeignKey("EquipmentId")] 
        // public Equipment Equipment { get; set; } // 先註解掉，避免 JSON 迴圈參考，除非有設定 DTO

        public string RequestUser { get; set; } = string.Empty;
        public string ReasonCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        /// <summary>
        /// Open, InProgress, Closed
        /// </summary>
        public string Status { get; set; } = "Open";

        public DateTime CreatedAt { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}