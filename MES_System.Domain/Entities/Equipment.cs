using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES_System.Domain.Enums;

namespace MES_System.Domain.Entities
{
    public class Equipment
    {
        public int Id { get; set; } // 主鍵 (Primary Key)
        public string Name { get; set; } = string.Empty; // 機台名稱 (e.g., SMT-01)
        public string Location { get; set; } = string.Empty; // 位置 (e.g., Line-A)
        public EquipmentStatus Status { get; set; } // 狀態
        public DateTime LastUpdated { get; set; } // 最後更新時間
    }
}
