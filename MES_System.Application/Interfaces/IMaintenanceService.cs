using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES_System.Application.DTOs;

namespace MES_System.Application.Interfaces
{
    public interface IMaintenanceService
    {
        // 建立維修單，回傳 void (Task) 即可，或回傳建立好的物件
        Task CreateMaintenanceRequestAsync(CreateMaintenanceDto dto);
    }
}