using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_System.Application.DTOs
{
    public class CreateMaintenanceDto
    {
        public int EquipmentId { get; set; }
        public string RequestUser { get; set; } = string.Empty; // 報修人
        public string ReasonCode { get; set; } = string.Empty;  // 故障代碼
        public string Description { get; set; } = string.Empty;
    }
}
