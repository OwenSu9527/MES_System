using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_System.Domain.Enums
{
    public enum EquipmentStatus
    {
        Idle = 0,    // 閒置 (黃燈)
        Running = 1, // 運作中 (綠燈)
        Down = 2,    // 故障/停機 (紅燈)
        Repair = 3   // 維修中 (紅燈/維修圖示)
    }
}
