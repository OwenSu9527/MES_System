using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_System.Application.DTOs
{
    public class CompleteMaintenanceDto
    {
        public int RequestId { get; set; } // 維修單 ID
        public string Solution { get; set; } = string.Empty; // 處置對策 (例如：更換馬達)
        public string Remark { get; set; } = string.Empty;   // 備註
    }
}