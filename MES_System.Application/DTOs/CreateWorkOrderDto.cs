using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_System.Application.DTOs
{
    /// <summary>
    /// 建立工單專用的資料傳輸物件
    /// 用途：定義前端呼叫 API 時，允許傳入的欄位，過濾掉不該被修改的欄位 (如 Status, Id)
    /// </summary>
    public class CreateWorkOrderDto
    {
        /// <summary>
        /// 工單號碼
        /// </summary>
        public string OrderNo { get; set; } = string.Empty;

        /// <summary>
        /// 產品名稱
        /// </summary>
        public string Product { get; set; } = string.Empty;

        /// <summary>
        /// 預計生產數量
        /// </summary>
        public int TargetQty { get; set; }
    }
}