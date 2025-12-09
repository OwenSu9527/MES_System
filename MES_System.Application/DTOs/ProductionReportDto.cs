using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_System.Application.DTOs
{
    /// <summary>
    /// 生產回報 DTO
    /// 用途：接收產線傳來的生產數據
    /// </summary>
    public class ProductionReportDto
    {
        /// <summary>
        /// 針對哪張工單？(填寫工單號，例如 "WO-20251202-001")
        /// </summary>
        // 實務上也可以傳 WorkOrderId (Int)，看前端方便，這裡我們示範用單號查
        public string OrderNo { get; set; } = string.Empty;

        /// <summary>
        /// 哪個站點回報的？(例如 "SMT" 或 StationId)
        /// </summary>
        // 為了簡單，傳 StationId
        public int StationId { get; set; }

        /// <summary>
        /// 做好了幾個？
        /// </summary>
        public int Quantity { get; set; }
    }
}
